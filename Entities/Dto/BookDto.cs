namespace Entities.Dto
{
    //[Serializable]
    public record BookDto
    {
        public int Id { get; init; }
        public required string Title { get; init; }
        public decimal Price { get; init; }
    };
}
