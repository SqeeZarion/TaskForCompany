using DogsHouseService.Domain;

namespace DogsHouseService.Infrastructure.Repositories;

public interface IDogRepository
{
    Task<List<Dog>> GetAllAsync(
        string? sortBy = null, 
        string? order = null, 
        int pageNumber = 1, 
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<Dog?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Dog dog, CancellationToken cancellationToken = default);
}