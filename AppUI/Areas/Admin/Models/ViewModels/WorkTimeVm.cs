namespace AppUI.Areas.Admin.Models.ViewModels;

public class WorkTimeVm
{
    public required int id { get; set; }
    public List<WorkVm>? ListWorks { get; set; }
}
