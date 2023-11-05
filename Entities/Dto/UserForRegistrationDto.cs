using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public record UserForRegistrationDto
    {
        [Required(ErrorMessage = "FirstName is Required.")]
        public string? FirstName { get; init; }
        [Required(ErrorMessage = "LastName is Required.")]
        public string? LastName { get; init; }

        [Required(ErrorMessage = "Username is Required.")]
        public string? Username { get; init; }
        [Required(ErrorMessage = "Password is Required.")]
        public string? Password { get; init; }
        [Required(ErrorMessage = "Email is Required.")]
        public string? Email { get; init; }
        [Required(ErrorMessage = "Phone number is Required.")]
        public string? PhoneNumber { get; init; }

        public ICollection<string>? Roles { get; init; }
    }
}
