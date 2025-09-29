using teste_pratico.Models.DTOs;

namespace teste_pratico.Services.Interfaces
{
    /// <summary>
    /// Interface para o serviço de consulta à API ViaCEP
    /// Responsável por buscar dados de endereço através do CEP
    /// </summary>
    public interface IViaCepService
    {
        /// <summary>
        /// Busca os dados de endereço através do CEP na API ViaCEP
        /// </summary>
        /// <param name="cep">CEP a ser consultado (8 dígitos numéricos)</param>
        /// <returns>Dados do endereço ou null se não encontrado/inválido</returns>
        /// <exception cref="ArgumentException">Quando o CEP está em formato inválido</exception>
        /// <exception cref="HttpRequestException">Quando há erro na comunicação com a API</exception>
        Task<ViaCepResponseDto?> GetAddressByCepAsync(string cep);
    }
}