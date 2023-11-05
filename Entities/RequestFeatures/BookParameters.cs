namespace Entities.RequestFeatures;
public class BookParameters : RequestParameters
{
    public uint MinPrice { get; set; } = 0;
    public uint MaxPrice { get; set; } = uint.MaxValue;
    public String? SearchTerm { get; set; }
    public bool ValidPriceRange => MaxPrice > MinPrice;

    public BookParameters()
    {
        OrderBy = "id";
    }
}