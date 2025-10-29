using System.ComponentModel.DataAnnotations;

namespace DogsHouseService.Domain;

public class Dog
{
    [Key]
    public int Id { get; set; } 
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public int TailLength { get; set; }
    public int Weight { get; set; }
}