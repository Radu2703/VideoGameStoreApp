using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Manager.Models.ViewModels;
public class SaleVm
{
    public int VideoGameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Display(Name = "Total Sales")]
    public decimal TotalSales { get; set; }
}
