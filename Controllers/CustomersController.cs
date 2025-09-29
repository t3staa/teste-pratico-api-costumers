using Microsoft.AspNetCore.Mvc;
using teste_pratico.Models.DTOs;
using teste_pratico.Services.Interfaces;

namespace teste_pratico.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de clientes
    /// Implementa os 5 endpoints CRUD conforme especificação
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerService customerService,
            ILogger<CustomersController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lista todos os clientes cadastrados
        /// </summary>
        /// <returns>Lista de clientes</returns>
        /// <response code="200">Lista de clientes retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAllCustomers()
        {
            _logger.LogInformation("Requisição para listar todos os clientes");

            var customers = await _customerService.GetAllCustomersAsync();

            _logger.LogInformation("Retornando {Count} clientes", customers.Count());

            return Ok(customers);
        }

        /// <summary>
        /// Obtém um cliente específico pelo ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Dados do cliente</returns>
        /// <response code="200">Cliente encontrado</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerById(long id)
        {
            _logger.LogInformation("Requisição para buscar cliente com ID: {CustomerId}", id);

            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Cliente não encontrado: {CustomerId}", id);
                return NotFound(new { error = "Cliente não encontrado", message = $"Cliente com ID {id} não foi encontrado." });
            }

            _logger.LogInformation("Cliente encontrado: {CustomerId}", id);

            return Ok(customer);
        }

        /// <summary>
        /// Cadastra um novo cliente
        /// Antes de salvar, consulta o ViaCEP para preencher o endereço
        /// </summary>
        /// <param name="customerRequest">Dados do cliente a ser criado</param>
        /// <returns>Cliente criado com endereço preenchido</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos ou CEP não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerResponseDto>> CreateCustomer([FromBody] CustomerRequestDto customerRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados de entrada inválidos para criação de cliente");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Requisição para criar cliente: {CustomerName}", customerRequest.Name);

            var createdCustomer = await _customerService.CreateCustomerAsync(customerRequest);

            _logger.LogInformation("Cliente criado com sucesso: {CustomerId}", createdCustomer.Id);

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = createdCustomer.Id },
                createdCustomer);
        }

        /// <summary>
        /// Atualiza os dados de um cliente existente
        /// Antes de salvar, consulta o ViaCEP para atualizar o endereço
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="customerRequest">Novos dados do cliente</param>
        /// <returns>Cliente atualizado</returns>
        /// <response code="200">Cliente atualizado com sucesso</response>
        /// <response code="400">Dados inválidos ou CEP não encontrado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(long id, [FromBody] CustomerRequestDto customerRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados de entrada inválidos para atualização do cliente: {CustomerId}", id);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Requisição para atualizar cliente: {CustomerId}", id);

            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerRequest);

            _logger.LogInformation("Cliente atualizado com sucesso: {CustomerId}", id);

            return Ok(updatedCustomer);
        }

        /// <summary>
        /// Remove um cliente pelo ID
        /// </summary>
        /// <param name="id">ID do cliente a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="204">Cliente removido com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            _logger.LogInformation("Requisição para excluir cliente: {CustomerId}", id);

            var deleted = await _customerService.DeleteCustomerAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Tentativa de excluir cliente inexistente: {CustomerId}", id);
                return NotFound(new { error = "Cliente não encontrado", message = $"Cliente com ID {id} não foi encontrado." });
            }

            _logger.LogInformation("Cliente excluído com sucesso: {CustomerId}", id);

            return NoContent();
        }
    }
}