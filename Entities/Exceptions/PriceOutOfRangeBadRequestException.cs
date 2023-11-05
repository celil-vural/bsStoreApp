namespace Entities.Exceptions;

public class PriceOutOfRangeBadRequestException : BadRequestException
{
    public PriceOutOfRangeBadRequestException() : base("Minimum price should be less than maximum price.")
    {
    }
}