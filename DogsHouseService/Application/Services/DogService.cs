using DogsHouseService.Application.DTOs;
using DogsHouseService.Domain;
using DogsHouseService.Infrastructure.Repositories;

namespace DogsHouseService.Application.Services;

public class DogService : IDogService
{
    private readonly IDogRepository _repository;

    public DogService(IDogRepository repository)
    {
        _repository = repository;
    }

    // Get all dogs
    public async Task<IEnumerable<DogResponse>> GetAllAsync(
        string? sortBy, string? order, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0 || pageSize > 50) pageSize = 10;

        var dogs = await _repository.GetAllAsync(sortBy, order, pageNumber, pageSize, cancellationToken);

        return dogs.Select(d => new DogResponse
        {
            Name = d.Name,
            Color = d.Color,
            TailLength = d.TailLength,
            Weight = d.Weight
        });
    }

    // Create dog
    public async Task<DogResponse> CreateAsync(CreateDogRequest request, CancellationToken cancellationToken)
    {
        
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name cannot be empty.");
        if (request.TailLength < 0)
            throw new ArgumentException("Tail length must be non-negative.");
        if (request.Weight < 0)
            throw new ArgumentException("Weight must be non-negative.");

        
        if (await _repository.ExistsByNameAsync(request.Name, cancellationToken))
            throw new InvalidOperationException($"Dog with name '{request.Name}' already exists.");

        
        var dog = new Dog
        {
            Name = request.Name,
            Color = request.Color,
            TailLength = request.TailLength,
            Weight = request.Weight
        };

        await _repository.AddAsync(dog, cancellationToken);
        
        return new DogResponse
        {
            Name = dog.Name,
            Color = dog.Color,
            TailLength = dog.TailLength,
            Weight = dog.Weight
        };
    }
}
