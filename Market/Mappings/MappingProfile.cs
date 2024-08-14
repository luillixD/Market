﻿using AutoMapper;
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
            CreateMap<User, UserDto>();

            CreateMap<UserDto, User>();

            CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.IsActiveUser, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => new List<UserRole> 
            { 
                new UserRole { RoleId = 2 } 
            }));

            CreateMap<UpdateUserDto, User>();

            CreateMap<Role, RoleDto>();

            CreateMap<RoleDto, UserRole>();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Subcategory, SubcategoryDto>().ReverseMap();

        }
    }
}