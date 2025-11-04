namespace Firmness.Admin.Web.ViewModels.Product;

public class ProductListItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string SKU { get; set; } = "";
    public string PriceFormatted { get; set; } = "";
}