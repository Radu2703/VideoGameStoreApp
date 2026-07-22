using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Admin.Models.ViewModels;
public class VideoGameVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    [Price]
    public decimal Price { get; set; }
    public decimal NewPrice { get; set; }
    [Stock]
    public int Stock { get; set; }
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; } = false;
}
