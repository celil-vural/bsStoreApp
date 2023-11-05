using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    [Serializable]
    public record BookDtoForUpdate : BookDtoForManipulation
    {
        [Required]
        public int Id { get; init; }
    }
}
