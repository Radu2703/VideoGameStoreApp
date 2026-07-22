namespace AppUI.Areas.Manager.Models.ViewModels;

public class IndexSalesVm
{
    public required string Publisher { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
