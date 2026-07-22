namespace AppUI.Areas.Manager.Models.ViewModels;

public class SaleTimeVm
{
    public required int id { get; set; }
    public required string Publisher { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
