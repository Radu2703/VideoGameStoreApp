using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Areas.Critic.Models.ViewModels;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class ReviewerProfile : Profile
{
    public ReviewerProfile()
    {
        CreateMap<NewReviewerDto, Reviewer>();
        CreateMap<Reviewer, ExistentReviewerDto>();
        CreateMap<Reviewer, CardReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));
        CreateMap<Reviewer, ShortReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.Id));
    }
}