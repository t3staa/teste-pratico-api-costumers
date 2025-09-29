using teste_pratico.Models.Entities;

namespace teste_pratico.Repositories.Interfaces
{
    /// <summary>
    /// Interface para o repositório de clientes
    /// Define os métodos de acesso a dados para a entidade Customer
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Obtém todos os clientes cadastrados
        /// </summary>
        /// <returns>Lista de clientes</returns>
        Task<IEnumerable<Customer>> GetAllAsync();

        /// <summary>
        /// Obtém um cliente específico pelo ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Cliente encontrado ou null</returns>
        Task<Customer?> GetByIdAsync(long id);

        /// <summary>
        /// Adiciona um novo cliente
        /// </summary>
        /// <param name="customer">Dados do cliente a ser adicionado</param>
        /// <returns>Cliente adicionado com ID gerado</returns>
        Task<Customer> AddAsync(Customer customer);

        /// <summary>
        /// Atualiza os dados de um cliente existente
        /// </summary>
        /// <param name="customer">Cliente com dados atualizados</param>
        /// <returns>Cliente atualizado</returns>
        Task<Customer> UpdateAsync(Customer customer);

        /// <summary>
        /// Remove um cliente pelo ID
        /// </summary>
        /// <param name="id">ID do cliente a ser removido</param>
        /// <returns>True se removido com sucesso, False caso não encontrado</returns>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Verifica se existe um cliente com o ID especificado
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>True se existe, False caso contrário</returns>
        Task<bool> ExistsAsync(long id);
    }
}