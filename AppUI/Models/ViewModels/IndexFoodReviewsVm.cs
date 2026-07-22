namespace AppUI.Models.ViewModels;

public class IndexVideoGameReviewsVm
{
    public string VideoGame { get; set; } = string.Empty;
    public List<CardReviewVm>? ListReviews { get; set; }
}
