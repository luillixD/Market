using AutoMapper;
using Market.DTOs.Login;
using Market.DTOs.Product;
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
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsEmailValidated, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRoles.FirstOrDefault().Role.Name));

            CreateMap<UserDto, User>();

            CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.IsEmailValidated, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => new List<UserRole> 
            { 
                new UserRole { RoleId = 2 } 
            }));


            CreateMap<User, RegisterDto>();

            CreateMap<CreateRoleDto, Role>();

            CreateMap<Role, RoleDto>();

            CreateMap<UpdateRoleDto, Role>();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Subcategory, SubcategoryDto>().ReverseMap();

        }
    }
}