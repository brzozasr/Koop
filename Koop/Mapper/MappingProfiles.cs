using System;
using AutoMapper;
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

            CreateMap<UserEdit, User>();
        }
    }
}