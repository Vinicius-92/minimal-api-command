using System.ComponentModel.DataAnnotations;

public class CommandCreateDTO
{
    [Required]
    public string? HotTo { get; set; }
    [Required]
    [MaxLength(5)]
    public string? Platform { get; set; }
    [Required]
    public string? CommandLine { get; set; }
}
