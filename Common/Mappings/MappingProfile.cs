using Application.Features.Categories.DTOs;
using Application.Features.Contents.DTOs;
using AutoMapper;
using Domain.Entities;
using Application.Features.Categories.DTOs;
using Application.Features.Contents.DTOs;
using Application.Features.ContactMessages.DTOs;
using Application.Features.Users.DTOs;
using Application.Features.Tags.DTOs;
using Application.Features.Products.DTOs;
using Application.Features.PageSettings.Dto;

namespace WEBProje.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Content Mappings
        CreateMap<Content, ContentListDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<Content, ContentDetailDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ContentTags.Select(ct => ct.Tag)));

        // Category Mappings
        CreateMap<Category, CategoryListDto>();
        CreateMap<Category, CategoryDetailDto>()
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));

        // Product Mappings
        CreateMap<Product, ProductListDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProductTags.Select(pt => pt.Tag)));

        // Tag Mappings
        CreateMap<Tag, TagDto>();

        // Contact Message Mappings
        CreateMap<ContactMessage, ContactMessageListDto>();
        CreateMap<ContactMessage, ContactMessageDetailDto>();
        // PageSetting Mappings
        CreateMap<PageSetting, PageSettingDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Group));

        CreateMap<PageSettingDto, PageSetting>()
            .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Category));

        // User Mappings
        CreateMap<User, UserDto>();
    }
}