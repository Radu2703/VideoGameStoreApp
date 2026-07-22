namespace AppUI.Areas.Admin.Models.ViewModels;

public class IndexVideoGamePublisherVm
{
    public string Name { get; set; } = string.Empty;
    public List<VideoGameVm>? ListVideoGames { get; set; }
}
