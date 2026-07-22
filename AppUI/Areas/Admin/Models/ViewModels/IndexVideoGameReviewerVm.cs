namespace AppUI.Areas.Admin.Models.ViewModels;

public class IndexVideoGameReviewerVm
{
    public string Name { get; set; } = string.Empty;
    public List<ReviewVm>? ListReviews { get; set; }
}
