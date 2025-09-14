using Adminpanel.api.Domain.Entities; // your entities namespace
using AutoMapper;
using static Adminpanel.api.Domain.Enum.ContentEnum;
using static Adminpanel.api.DTOs.Blogs;
using static Adminpanel.api.DTOs.Pages;
using static Adminpanel.api.DTOs.Placements;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // BLOGS
        CreateMap<BlogPost, BlogListDto>()
            .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.ToString()));
        CreateMap<BlogPost, BlogDetailDto>()
            .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.ToString()));
        CreateMap<BlogCreateDto, BlogPost>()
            .ForMember(d => d.Status, m => m.MapFrom(_ => PostStatus.Draft))
            .ForMember(d => d.CreatedAt, m => m.MapFrom(_ => DateTime.UtcNow));
        CreateMap<BlogUpdateDto, BlogPost>()
            .ForMember(d => d.Status, m => m.MapFrom(s => Enum.Parse<PostStatus>(s.Status, true)));

        // PAGES
        CreateMap<Page, PageListDto>();
        CreateMap<Page, PageDetailDto>()
            .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.ToString()));
        CreateMap<PageCreateDto, Page>()
            .ForMember(d => d.Status, m => m.MapFrom(_ => PageStatus.Draft))
            .ForMember(d => d.CreatedAt, m => m.MapFrom(_ => DateTime.UtcNow));
        CreateMap<PageUpdateDto, Page>()
            .ForMember(d => d.Status, m => m.MapFrom(s => Enum.Parse<PageStatus>(s.Status, true)))
            .ForMember(d => d.UpdatedAt, m => m.MapFrom(_ => DateTime.UtcNow));

        // PLACEMENTS
        CreateMap<ContentPlacement, PlacementItemDto>()
            .ForMember(d => d.Position, m => m.MapFrom(s => s.Position.ToString()))
            .ForMember(d => d.ContentType, m => m.MapFrom(s => s.ContentType.ToString()));
        CreateMap<PlacementCreateDto, ContentPlacement>()
            .ForMember(d => d.Position, m => m.MapFrom(s => Enum.Parse<PlacementPosition>(s.Position, true)))
            .ForMember(d => d.ContentType, m => m.MapFrom(s => Enum.Parse<PlacementContentType>(s.ContentType, true)))
            .ForMember(d => d.CreatedAt, m => m.MapFrom(_ => DateTime.UtcNow));
        CreateMap<PlacementUpdateDto, ContentPlacement>()
            .ForMember(d => d.Position, m => m.MapFrom(s => Enum.Parse<PlacementPosition>(s.Position, true)))
            .ForMember(d => d.ContentType, m => m.MapFrom(s => Enum.Parse<PlacementContentType>(s.ContentType, true)));
    }
}
