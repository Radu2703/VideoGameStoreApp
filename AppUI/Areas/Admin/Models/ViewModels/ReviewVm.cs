using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Admin.Models.ViewModels;
public class ReviewVm
{
    public int VideoGameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; } = false;
}
