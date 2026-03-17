using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities
{
public class Role
    {
        [Key]
        [MaxLength(16)]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de rol es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre de rol no pede superar los 100 caracteres")]
        public string Name { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //Relaciones con UserRole
        public ICollection<UserRole> UserRole { get; set; } = [];

    }
}
/*
    Roles
    +--------------+--------------+------------------+
    | Id           | Name         | Description      |
    +--------------+--------------+------------------+
    | ADMIN        | Admin        | Administrador    |
    | USER         | User         | Usuario normal   |
    | GUEST        | Guest        | Invitado         |
    +--------------+--------------+------------------+
*/