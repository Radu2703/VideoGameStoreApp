using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Admin.Models.ViewModels;

public class SaleTimeItemVm
{
    public int id { get; set; }
    public int VideoGameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Display(Name = "Total Sales")]
    public decimal TotalSales { get; set; }
}
