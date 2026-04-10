using Aethera.Dtos;
using Aethera.Dtos.Order;
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
                    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Category) ? "perfumes" : src.Category));

               
                CreateMap<UpdateProductDto, Product>();

                
                CreateMap<Product, CreateProductDto>().ReverseMap();
                CreateMap<Product, UpdateProductDto>().ReverseMap();


                CreateMap<CartItemDto, CartItem>().ReverseMap();
            
            CreateMap<CartDto, Cart>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ShippingAddressDto, ShippingAddress>().ReverseMap();
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ReverseMap();
        }
    }
    }



