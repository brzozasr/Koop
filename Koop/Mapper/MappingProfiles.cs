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

            CreateMap<OrderedItemQuantityUpdate, OrderedItem>();
            CreateMap<ProductsQuantityUpdate, Product>();
            CreateMap<ProductSupplierUpdate, Product>();
        }
    }
}