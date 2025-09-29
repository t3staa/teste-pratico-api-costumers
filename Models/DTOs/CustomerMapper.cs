using teste_pratico.Models.Entities;

namespace teste_pratico.Models.DTOs
{
    /// <summary>
    /// Classe auxiliar para mapear entre Customer entity e DTOs
    /// </summary>
    public static class CustomerMapper
    {
        /// <summary>
        /// Converte uma entidade Customer para CustomerResponseDto
        /// </summary>
        public static CustomerResponseDto ToResponseDto(this Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Cep = customer.Cep,
                Street = customer.Street,
                City = customer.City,
                State = customer.State,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }

        /// <summary>
        /// Converte uma lista de Customer entities para CustomerResponseDto
        /// </summary>
        public static IEnumerable<CustomerResponseDto> ToResponseDto(this IEnumerable<Customer> customers)
        {
            return customers?.Select(c => c.ToResponseDto()) ?? Enumerable.Empty<CustomerResponseDto>();
        }

        /// <summary>
        /// Cria uma nova entidade Customer a partir de um CustomerRequestDto
        /// </summary>
        public static Customer ToEntity(this CustomerRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new Customer
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.ToLower().Trim(),
                Cep = dto.GetNormalizedCep(),
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Atualiza uma entidade Customer existente com dados de um CustomerRequestDto
        /// </summary>
        public static void UpdateFromDto(this Customer customer, CustomerRequestDto dto)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            customer.Name = dto.Name.Trim();
            customer.Email = dto.Email.ToLower().Trim();
            customer.Cep = dto.GetNormalizedCep();
            // Street, City e State serão atualizados pelo serviço após consultar ViaCEP
        }

        /// <summary>
        /// Atualiza os dados de endereço do Customer com base na resposta do ViaCEP
        /// </summary>
        public static void UpdateAddressFromViaCep(this Customer customer, ViaCepResponseDto viaCepResponse)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));
            if (viaCepResponse == null || !viaCepResponse.IsValid)
                return;

            customer.Street = viaCepResponse.Logradouro ?? string.Empty;
            customer.City = viaCepResponse.Localidade ?? string.Empty;
            customer.State = viaCepResponse.Uf ?? string.Empty;
        }
    }
}