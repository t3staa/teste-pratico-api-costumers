using Microsoft.EntityFrameworkCore;
using teste_pratico.Data;
using teste_pratico.Models.Entities;
using teste_pratico.Repositories.Interfaces;

namespace teste_pratico.Repositories.Implementations
{
    /// <summary>
    /// Implementação do repositório de clientes usando Entity Framework Core
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Customer?> GetByIdAsync(long id)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <inheritdoc />
        public async Task<Customer> AddAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        /// <inheritdoc />
        public async Task<Customer> UpdateAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _context.Entry(customer).State = EntityState.Modified;

            // Não atualizar CreatedAt
            _context.Entry(customer).Property(x => x.CreatedAt).IsModified = false;

            await _context.SaveChangesAsync();

            return customer;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(long id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Customers
                .AnyAsync(c => c.Id == id);
        }
    }
}