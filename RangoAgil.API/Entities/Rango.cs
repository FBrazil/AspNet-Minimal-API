using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RangoAgil.API.Entities;

public class Rango
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; }
    public ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();

    [SetsRequiredMembers]
    public Rango(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public Rango()
    {
    }
}
