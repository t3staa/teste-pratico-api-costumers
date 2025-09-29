using System.Text.Json;
using teste_pratico.Models.DTOs;
using teste_pratico.Services.Interfaces;

namespace teste_pratico.Services.Implementations
{
    /// <summary>
    /// Implementação do serviço de consulta à API ViaCEP
    /// </summary>
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViaCepService> _logger;
        private const string ViaCepBaseUrl = "https://viacep.com.br/ws";

        public ViaCepService(HttpClient httpClient, ILogger<ViaCepService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configurar timeout padrão
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <inheritdoc />
        public async Task<ViaCepResponseDto?> GetAddressByCepAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                _logger.LogWarning("CEP fornecido está vazio ou nulo");
                throw new ArgumentException("CEP não pode ser vazio ou nulo", nameof(cep));
            }

            // Normalizar CEP (remover caracteres não numéricos)
            var normalizedCep = System.Text.RegularExpressions.Regex.Replace(cep, @"\D", "");

            if (normalizedCep.Length != 8)
            {
                _logger.LogWarning("CEP {Cep} possui formato inválido após normalização: {NormalizedCep}", cep, normalizedCep);
                throw new ArgumentException("CEP deve conter exatamente 8 dígitos numéricos", nameof(cep));
            }

            var url = $"{ViaCepBaseUrl}/{normalizedCep}/json/";

            try
            {
                _logger.LogInformation("Consultando ViaCEP para o CEP: {Cep}", normalizedCep);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Erro ao consultar ViaCEP. Status: {StatusCode}, CEP: {Cep}",
                        response.StatusCode, normalizedCep);
                    throw new HttpRequestException($"Erro na consulta ViaCEP: {response.StatusCode}");
                }

                var jsonContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("Resposta vazia do ViaCEP para o CEP: {Cep}", normalizedCep);
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponseDto>(jsonContent, options);

                if (viaCepResponse == null)
                {
                    _logger.LogError("Falha ao deserializar resposta do ViaCEP para o CEP: {Cep}", normalizedCep);
                    return null;
                }

                // Verificar se o ViaCEP retornou erro
                if (viaCepResponse.Erro.GetValueOrDefault(false))
                {
                    _logger.LogWarning("ViaCEP retornou erro para o CEP: {Cep}", normalizedCep);
                    return null;
                }

                // Verificar se tem dados mínimos válidos
                if (!viaCepResponse.IsValid)
                {
                    _logger.LogWarning("Dados inválidos retornados pelo ViaCEP para o CEP: {Cep}", normalizedCep);
                    return null;
                }

                _logger.LogInformation("CEP {Cep} consultado com sucesso: {Localidade}, {Uf}",
                    normalizedCep, viaCepResponse.Localidade, viaCepResponse.Uf);

                return viaCepResponse;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Timeout ao consultar ViaCEP para o CEP: {Cep}", normalizedCep);
                throw new HttpRequestException("Timeout na consulta ao ViaCEP", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro HTTP ao consultar ViaCEP para o CEP: {Cep}", normalizedCep);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro ao deserializar resposta do ViaCEP para o CEP: {Cep}", normalizedCep);
                throw new HttpRequestException("Erro ao processar resposta do ViaCEP", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar ViaCEP para o CEP: {Cep}", normalizedCep);
                throw new HttpRequestException("Erro inesperado na consulta ao ViaCEP", ex);
            }
        }
    }
}