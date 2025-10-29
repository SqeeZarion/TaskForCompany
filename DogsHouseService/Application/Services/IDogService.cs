using DogsHouseService.Application.DTOs;

namespace DogsHouseService.Application.Services;

public interface IDogService
{
    Task<IEnumerable<DogResponse>> GetAllAsync(
        string? sortBy, string? order, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<DogResponse> CreateAsync(CreateDogRequest request, CancellationToken cancellationToken);
}