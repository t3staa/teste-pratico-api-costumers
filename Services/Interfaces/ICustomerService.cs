using teste_pratico.Models.DTOs;

namespace teste_pratico.Services.Interfaces
{
    /// <summary>
    /// Interface para o serviço de negócio de clientes
    /// Responsável pela lógica de negócio e orquestração entre Repository e ViaCEP
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Obtém todos os clientes cadastrados
        /// </summary>
        /// <returns>Lista de clientes</returns>
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();

        /// <summary>
        /// Obtém um cliente específico pelo ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Cliente encontrado ou null se não existir</returns>
        Task<CustomerResponseDto?> GetCustomerByIdAsync(long id);

        /// <summary>
        /// Cria um novo cliente
        /// Antes de salvar, consulta o ViaCEP para preencher o endereço
        /// </summary>
        /// <param name="customerRequest">Dados do cliente a ser criado</param>
        /// <returns>Cliente criado com endereço preenchido</returns>
        /// <exception cref="ArgumentException">Quando os dados são inválidos</exception>
        /// <exception cref="InvalidOperationException">Quando o CEP é inválido ou não encontrado</exception>
        Task<CustomerResponseDto> CreateCustomerAsync(CustomerRequestDto customerRequest);

        /// <summary>
        /// Atualiza os dados de um cliente existente
        /// Antes de salvar, consulta o ViaCEP para atualizar o endereço
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="customerRequest">Novos dados do cliente</param>
        /// <returns>Cliente atualizado com endereço atualizado</returns>
        /// <exception cref="ArgumentException">Quando os dados são inválidos</exception>
        /// <exception cref="InvalidOperationException">Quando o cliente não existe ou CEP é inválido</exception>
        Task<CustomerResponseDto> UpdateCustomerAsync(long id, CustomerRequestDto customerRequest);

        /// <summary>
        /// Remove um cliente pelo ID
        /// </summary>
        /// <param name="id">ID do cliente a ser removido</param>
        /// <returns>True se removido com sucesso, False se não encontrado</returns>
        Task<bool> DeleteCustomerAsync(long id);

        /// <summary>
        /// Verifica se existe um cliente com o ID especificado
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>True se existe, False caso contrário</returns>
        Task<bool> CustomerExistsAsync(long id);
    }
}