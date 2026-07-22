using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;

public class PublisherProfile : Profile
{
    public PublisherProfile()
    {
        CreateMap<NewPublisherDto, Publisher>();
        CreateMap<Publisher, ExistentPublisherDto>();
        CreateMap<Publisher, CardPublisherVm>()
            .ForMember(d => d.PublisherId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));
    }
}