using System.Collections.Generic;
using System;
using AutoMapper;
using Koop.Extensions;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.SignalR;

namespace Koop.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserSignUp, User>();

            CreateMap<OrderView, CoopOrder>();
            CreateMap<OrderView, CoopOrderNode>()
                .AfterMap((src, dest) =>
                    dest.FundPrice = dest.CalculateFundPrice())
                .AfterMap((src, dest) =>
                    dest.TotalPrice = dest.CalculateTotalPrice())
                .AfterMap((src, dest) =>
                    dest.TotalFundPrice = dest.CalculateTotalFundPrice());

            CreateMap<Supplier, SupplierProducts>();
            CreateMap<Product, SupplierProductsNode>();

            CreateMap<Product, StockStatus>();
            CreateMap<Supplier, StockStatus>();
            
            // CreateMap<(Supplier supplier, Product product), StockStatus>()
            //     .ForMember(dest => dest.SupplierId,
            //         opt =>
            //         {
            //             opt.Condition(s => s.supplier.SupplierId == s.product.SupplierId);
            //             opt.MapFrom(src => src.supplier.SupplierId);
            //         })
            //     .ForMember(dest => dest.SupplierName,
            //         opt => 
            //         {
            //             opt.Condition(s => s.supplier.SupplierId == s.product.SupplierId);
            //             opt.MapFrom(src => src.supplier.SupplierName);
            //         })
            //     .ForMember(dest => dest.SupplierAbbr,
            //         opt => 
            //         {
            //             opt.Condition(s => s.supplier.SupplierId == s.product.SupplierId);
            //             opt.MapFrom(src => src.supplier.SupplierAbbr);
            //         })
            //     .ForMember(dest => dest.ProductId, 
            //         opt => opt.MapFrom(src => src.product.ProductId))
            //     .ForMember(dest => dest.ProductName,
            //         opt => opt.MapFrom(src => src.product.ProductName))
            //     .ForMember(dest => dest.Price,
            //         opt => opt.MapFrom(src => src.product.Price))
            //     .ForMember(dest => dest.Description,
            //         opt => opt.MapFrom(src => src.product.Description))
            //     .ForMember(dest => dest.AmountInMagazine,
            //         opt => opt.MapFrom(src => src.product.AmountInMagazine))
            //     .ForMember(dest => dest.Magazine,
            //         opt => opt.MapFrom(src => src.product.Magazine))
            //     .ForMember(dest => dest.AmountMax,
            //         opt => opt.MapFrom(src => src.product.AmountMax))
            //     .ForMember(dest => dest.Deposit,
            //         opt => opt.MapFrom(src => src.product.Deposit))
            //     .ForMember(dest => dest.Picture,
            //         opt => opt.MapFrom(src => src.product.Picture))
            //     .ForMember(dest => dest.UnitId,
            //         opt => opt.MapFrom(src => src.product.UnitId))
            //     .ForMember(dest => dest.Available,
            //         opt => opt.MapFrom(src => src.product.Available))
            //     .ForMember(dest => dest.Blocked,
            //         opt => opt.MapFrom(src => src.product.Blocked));

            // CreateMap<(Supplier supplier, Product product), StockStatus>()
            //     .ForMember(dest => dest.SupplierId,
            //         opt => opt.MapFrom(src => src.supplier.SupplierId))
            //     .ForMember(dest => dest.SupplierName,
            //         opt => opt.MapFrom(src => src.supplier.SupplierName))
            //     .ForMember(dest => dest.SupplierAbbr,
            //         opt => opt.MapFrom(src => src.supplier.SupplierAbbr))
            //     .ForMember(dest => dest.ProductId, 
            //         opt => opt.MapFrom(src => src.product.ProductId))
            //     .ForMember(dest => dest.ProductName,
            //         opt => opt.MapFrom(src => src.product.ProductName))
            //     .ForMember(dest => dest.Price,
            //         opt => opt.MapFrom(src => src.product.Price))
            //     .ForMember(dest => dest.Description,
            //         opt => opt.MapFrom(src => src.product.Description))
            //     .ForMember(dest => dest.AmountInMagazine,
            //         opt => opt.MapFrom(src => src.product.AmountInMagazine))
            //     .ForMember(dest => dest.Magazine,
            //         opt => opt.MapFrom(src => src.product.Magazine))
            //     .ForMember(dest => dest.AmountMax,
            //         opt => opt.MapFrom(src => src.product.AmountMax))
            //     .ForMember(dest => dest.Deposit,
            //         opt => opt.MapFrom(src => src.product.Deposit))
            //     .ForMember(dest => dest.Picture,
            //         opt => opt.MapFrom(src => src.product.Picture))
            //     .ForMember(dest => dest.UnitId,
            //         opt => opt.MapFrom(src => src.product.UnitId))
            //     .ForMember(dest => dest.Available,
            //         opt => opt.MapFrom(src => src.product.Available))
            //     .ForMember(dest => dest.Blocked,
            //         opt => opt.MapFrom(src => src.product.Blocked));

            CreateMap<UserEdit, User>();

            CreateMap<ProductsShop, Product>();
            CreateMap<ProductsShop, ProductsShop>();
            CreateMap<ProductsShop, Unit>();
            CreateMap<ProductsShop, AvailableQuantity>();
            CreateMap<ProductsShop, Supplier>();

            CreateMap<ProductCategory, ProductCategoriesCombo>();
            CreateMap<Category, ProductCategoriesCombo>();
            CreateMap<Category, Category>();
            CreateMap<ProductCategoriesCombo, ProductCategory>();
            //CreateMap<ProductCategoriesCombo, ProductCategory>();
            CreateMap<AvailableQuantity, AvailableQuantity>();

            CreateMap<Product, Product>();
        }
    }
}