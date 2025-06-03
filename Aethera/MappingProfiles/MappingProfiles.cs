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
                // From CreateProductDto to Product (when creating)
                CreateMap<CreateProductDto, Product>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                    .ForMember(dest => dest.Category, opt => opt.MapFrom(_ => "perfumes"));

                // From UpdateProductDto to Product (when updating)
                CreateMap<UpdateProductDto, Product>();

                // Optional: back to DTOs if needed
                CreateMap<Product, CreateProductDto>().ReverseMap();
                CreateMap<Product, UpdateProductDto>().ReverseMap();


                CreateMap<CartItemDto, CartItem>().ReverseMap();
                CreateMap<CartDto, Cart>().ReverseMap();
        }
        }
    }



