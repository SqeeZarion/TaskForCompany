using DogsHouseService.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DogsHouseService.Infrastructure.Repositories;

public class DogRepository : IDogRepository
{
    private readonly DogsDbContext _context;

    public DogRepository(DogsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dog>> GetAllAsync(
        string? sortBy = null, 
        string? order = null, 
        int pageNumber = 1, 
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Dog> query = _context.Dogs.AsQueryable();

        // Sorting
        query = ApplySorting(query, sortBy, order);

        // Pagination
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Dog?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Dogs.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Dogs.AnyAsync(d => d.Name == name, cancellationToken);
    }

    public async Task AddAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        _context.Dogs.Add(dog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Dynamic sorting helper
    private static IQueryable<Dog> ApplySorting(IQueryable<Dog> query, string? sortBy, string? order)
    {
        Expression<Func<Dog, object>> keySelector = sortBy?.ToLower() switch
        {
            "name" => d => d.Name,
            "color" => d => d.Color,
            "tail_length" => d => d.TailLength,
            "weight" => d => d.Weight,
            _ => d => d.Id // default sort by Id
        };

        bool descending = order?.ToLower() == "desc";
        return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }
    
    public async Task DeleteAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        _context.Dogs.Remove(dog);
        await _context.SaveChangesAsync(cancellationToken);
    }

}
