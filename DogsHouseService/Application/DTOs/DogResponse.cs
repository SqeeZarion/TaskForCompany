namespace DogsHouseService.Application.DTOs;

public class DogResponse
{
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public int TailLength { get; set; }
    public int Weight { get; set; }
}