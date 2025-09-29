using teste_pratico.Models.DTOs;
using teste_pratico.Models.Entities;
using teste_pratico.Repositories.Interfaces;
using teste_pratico.Services.Interfaces;

namespace teste_pratico.Services.Implementations
{
    /// <summary>
    /// Implementação do serviço de negócio de clientes
    /// Orquestra Repository, ViaCepService e aplica regras de negócio
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IViaCepService _viaCepService;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository customerRepository,
            IViaCepService viaCepService,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
        {
            _logger.LogInformation("Buscando todos os clientes");

            var customers = await _customerRepository.GetAllAsync();
            var result = customers.ToResponseDto();

            _logger.LogInformation("Encontrados {Count} clientes", customers.Count());

            return result;
        }

        /// <inheritdoc />
        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(long id)
        {
            _logger.LogInformation("Buscando cliente com ID: {CustomerId}", id);

            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido: {CustomerId}", id);
                throw new ArgumentException("ID deve ser maior que zero", nameof(id));
            }

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Cliente não encontrado: {CustomerId}", id);
                return null;
            }

            _logger.LogInformation("Cliente encontrado: {CustomerId} - {CustomerName}", id, customer.Name);

            return customer.ToResponseDto();
        }

        /// <inheritdoc />
        public async Task<CustomerResponseDto> CreateCustomerAsync(CustomerRequestDto customerRequest)
        {
            if (customerRequest == null)
                throw new ArgumentNullException(nameof(customerRequest));

            _logger.LogInformation("Criando novo cliente: {CustomerName}, CEP: {Cep}",
                customerRequest.Name, customerRequest.Cep);

            // Consultar ViaCEP antes de salvar
            var viaCepResponse = await GetAndValidateAddressAsync(customerRequest.Cep);

            // Converter DTO para entidade
            var customer = customerRequest.ToEntity();

            // Preencher dados do endereço com ViaCEP
            customer.UpdateAddressFromViaCep(viaCepResponse);

            try
            {
                // Salvar no banco
                var savedCustomer = await _customerRepository.AddAsync(customer);

                _logger.LogInformation("Cliente criado com sucesso: {CustomerId} - {CustomerName}",
                    savedCustomer.Id, savedCustomer.Name);

                return savedCustomer.ToResponseDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cliente: {CustomerName}", customerRequest.Name);
                throw new InvalidOperationException("Erro ao criar cliente", ex);
            }
        }

        /// <inheritdoc />
        public async Task<CustomerResponseDto> UpdateCustomerAsync(long id, CustomerRequestDto customerRequest)
        {
            if (id <= 0)
                throw new ArgumentException("ID deve ser maior que zero", nameof(id));

            if (customerRequest == null)
                throw new ArgumentNullException(nameof(customerRequest));

            _logger.LogInformation("Atualizando cliente: {CustomerId}", id);

            // Verificar se cliente existe
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                _logger.LogWarning("Tentativa de atualizar cliente inexistente: {CustomerId}", id);
                throw new InvalidOperationException($"Cliente com ID {id} não foi encontrado");
            }

            // Consultar ViaCEP antes de salvar
            var viaCepResponse = await GetAndValidateAddressAsync(customerRequest.Cep);

            // Atualizar dados do cliente existente
            existingCustomer.UpdateFromDto(customerRequest);

            // Preencher dados do endereço com ViaCEP
            existingCustomer.UpdateAddressFromViaCep(viaCepResponse);

            try
            {
                // Salvar no banco
                var updatedCustomer = await _customerRepository.UpdateAsync(existingCustomer);

                _logger.LogInformation("Cliente atualizado com sucesso: {CustomerId} - {CustomerName}",
                    id, updatedCustomer.Name);

                return updatedCustomer.ToResponseDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente: {CustomerId}", id);
                throw new InvalidOperationException("Erro ao atualizar cliente", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteCustomerAsync(long id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido para exclusão: {CustomerId}", id);
                throw new ArgumentException("ID deve ser maior que zero", nameof(id));
            }

            _logger.LogInformation("Excluindo cliente: {CustomerId}", id);

            try
            {
                var deleted = await _customerRepository.DeleteAsync(id);

                if (deleted)
                {
                    _logger.LogInformation("Cliente excluído com sucesso: {CustomerId}", id);
                }
                else
                {
                    _logger.LogWarning("Cliente não encontrado para exclusão: {CustomerId}", id);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente: {CustomerId}", id);
                throw new InvalidOperationException("Erro ao excluir cliente", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> CustomerExistsAsync(long id)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _customerRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do cliente: {CustomerId}", id);
                return false;
            }
        }

        /// <summary>
        /// Busca e valida endereço via ViaCEP
        /// </summary>
        /// <param name="cep">CEP a ser consultado</param>
        /// <returns>Resposta válida do ViaCEP</returns>
        /// <exception cref="InvalidOperationException">Quando CEP é inválido ou não encontrado</exception>
        private async Task<ViaCepResponseDto> GetAndValidateAddressAsync(string cep)
        {
            try
            {
                _logger.LogInformation("Consultando ViaCEP para o CEP: {Cep}", cep);

                var viaCepResponse = await _viaCepService.GetAddressByCepAsync(cep);

                if (viaCepResponse == null || !viaCepResponse.IsValid)
                {
                    _logger.LogWarning("CEP inválido ou não encontrado: {Cep}", cep);
                    throw new InvalidOperationException("CEP inválido ou não encontrado. Verifique o CEP informado.");
                }

                if (!viaCepResponse.HasCompleteAddress)
                {
                    _logger.LogWarning("CEP {Cep} retornado pelo ViaCEP possui dados incompletos", cep);
                    throw new InvalidOperationException("CEP encontrado mas com dados de endereço incompletos.");
                }

                _logger.LogInformation("Endereço encontrado via ViaCEP: {Logradouro}, {Localidade}/{Uf}",
                    viaCepResponse.Logradouro, viaCepResponse.Localidade, viaCepResponse.Uf);

                return viaCepResponse;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("CEP em formato inválido: {Cep}", cep);
                throw new InvalidOperationException("CEP deve conter exatamente 8 dígitos numéricos.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de comunicação com ViaCEP para o CEP: {Cep}", cep);
                throw new InvalidOperationException("Erro ao consultar CEP. Tente novamente em alguns instantes.", ex);
            }
            catch (InvalidOperationException)
            {
                // Re-throw InvalidOperationException sem wrapping
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar ViaCEP para o CEP: {Cep}", cep);
                throw new InvalidOperationException("Erro inesperado ao consultar CEP.", ex);
            }
        }
    }
}