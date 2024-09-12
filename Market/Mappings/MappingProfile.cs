using AutoMapper;
using Market.DTOs.Bill;
using Market.DTOs.Category;
using Market.DTOs.Login;
using Market.DTOs.Product;
using Market.DTOs.Review;
using Market.DTOs.Purchase;
using Market.DTOs.Roles;
using Market.DTOs.Subcategory;
using Market.DTOs.Users;
using Market.Models;

namespace Market.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.IsActiveUser, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => new List<UserRole> 
            { 
                new UserRole { RoleId = 2 } 
            }));

            CreateMap<UpdateUserDto, User>();

            CreateMap<Role, RoleDto>();

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Subcategory.Name))
                .ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();

            CreateMap<CreateSubcategoryDto, Subcategory>();
            CreateMap<UpdateSubcategoryDto, Subcategory>();
            CreateMap<Subcategory, SubcategoryDto>();

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Subcategories, opt => opt.Ignore());
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Category, CategoryDto>();

            CreateMap<CreatePurchaseDto, Purchase>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address 
            { 
                Latitude = src.Latitud, 
                Longitude = src.Longitud, 
                AdditionalData = src.AdditionalData 
            }))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => 0));


            CreateMap<PurchaseDto, Purchase>();

            CreateMap<Purchase, PurchaseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
            
            CreateMap<Review, ReviewDto>();
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}