using Aethera.Dtos;
using Aethera.Dtos.Product;
using Aethera.Models;
using AutoMapper;

namespace Aethera.MappingProfiles
{
 

        public class MappingProfiles : Profile
        {
            public MappingProfiles()
            {
                
                CreateMap<CreateProductDto, Product>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                    .ForMember(dest => dest.Category, opt => opt.MapFrom(_ => "perfumes"));

               
                CreateMap<UpdateProductDto, Product>();

                
                CreateMap<Product, CreateProductDto>().ReverseMap();
                CreateMap<Product, UpdateProductDto>().ReverseMap();


                CreateMap<CartItemDto, CartItem>().ReverseMap();
            
            CreateMap<CartDto, Cart>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ReverseMap();

        }
    }
    }



