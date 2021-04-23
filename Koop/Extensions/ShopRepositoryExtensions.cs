using System;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Extensions
{
    public enum ShopRepositoryReturn
    {
        RemoveUserOrderSuccess,
        RemoveUserOrderErrEmptyObject,
        UpdateUserOrderQuantitySuccess,
        UpdateUserOrderQuantityErrTooMuch,
        UpdateUserOrderQuantityErrOther,
        MakeOrderSuccess,
        MakeOrderErrTooMuch,
        MakeOrderErrOther,
        UpdateUserOrderStatusSuccess,
        RemoveUnitsSuccess,
        UpdateUnitsSuccess,
        AddUnitsSuccess,
        UpdateUnitsErrNone,
        RemoveCategoriesSuccess,
        UpdateCategoriesSuccess,
        AddCategoriesSuccess,
        UpdateCategoriesNone,
        RemoveProductCategoriesSuccess,
        UpdateProductCategoriesSuccess,
        AddProductCategoriesSuccess,
        UpdateProductCategoriesNone,
        UpdateAvailableQuantitiesSuccess,
        AddAvailableQuantitiesSuccess,
        UpdateAvailableQuantitiesNone,
        RemoveAvailableQuantitiesSuccess,
        RemoveProductSuccess,
        UpdateProductSuccess,
        AddProductSuccess,
        UpdateSupplierSuccess
    }
    
    public static class ShopRepositoryExtensions
    {
        public static ShopRepositoryResponse ToObject(this ShopRepositoryReturn shopRepositoryReturn)
        {
            return shopRepositoryReturn switch
            {
                ShopRepositoryReturn.RemoveUserOrderSuccess => new ShopRepositoryResponse()
                {
                    Message = "Order removed successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.RemoveUserOrderErrEmptyObject => new ShopRepositoryResponse()
                {
                    Message = "The order does not exist in the database.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.UpdateUserOrderQuantitySuccess => new ShopRepositoryResponse()
                {
                    Message = "Order's quantity updated successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateUserOrderQuantityErrTooMuch => new ShopRepositoryResponse()
                {
                    Message = "Provided quantity is higher than available amount.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.UpdateUserOrderQuantityErrOther => new ShopRepositoryResponse()
                {
                    Message = "You are not allowed to change the order's quantity.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.MakeOrderSuccess => new ShopRepositoryResponse()
                {
                    Message = "Your order was accepted.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.MakeOrderErrTooMuch => new ShopRepositoryResponse()
                {
                    Message = "The inventory is insufficient to fulfill the order.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.MakeOrderErrOther => new ShopRepositoryResponse()
                {
                    Message = "You cannot make an order.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.UpdateUserOrderStatusSuccess => new ShopRepositoryResponse()
                {
                    Message = "Order status updated successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.RemoveUnitsSuccess => new ShopRepositoryResponse()
                {
                    Message = "Entries of Unit were removed successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateUnitsSuccess => new ShopRepositoryResponse()
                {
                    Message = "Entries of Unit were updated successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.AddUnitsSuccess => new ShopRepositoryResponse()
                {
                    Message = "New unit added successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateUnitsErrNone => new ShopRepositoryResponse()
                {
                    Message = "No units to change.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.RemoveCategoriesSuccess => new ShopRepositoryResponse()
                {
                    Message = "Entries of Categories were removed successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateCategoriesSuccess => new ShopRepositoryResponse()
                {
                    Message = "Table ProductCategories updated successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.AddCategoriesSuccess => new ShopRepositoryResponse()
                {
                    Message = "New category added successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateCategoriesNone => new ShopRepositoryResponse()
                {
                    Message = "No categories to change.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.RemoveProductCategoriesSuccess => new ShopRepositoryResponse()
                {
                    Message = "Entries of ProductCategories were removed successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateProductCategoriesSuccess => new ShopRepositoryResponse()
                {
                    Message = "Product categories updated successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.AddProductCategoriesSuccess => new ShopRepositoryResponse()
                {
                    Message = "New category was added to the product.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.RemoveAvailableQuantitiesSuccess => new ShopRepositoryResponse()
                {
                    Message = "Entries of AvailableQuantities were removed successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateProductCategoriesNone => new ShopRepositoryResponse()
                {
                    Message = "No categories to change.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.UpdateAvailableQuantitiesSuccess => new ShopRepositoryResponse()
                {
                    Message = "Table AvailableQuantities updated successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.AddAvailableQuantitiesSuccess => new ShopRepositoryResponse()
                {
                    Message = "New Available Quantities added to the product.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateAvailableQuantitiesNone => new ShopRepositoryResponse()
                {
                    Message = "Nothing to change.",
                    StatusCode = 500
                },
                ShopRepositoryReturn.RemoveProductSuccess => new ShopRepositoryResponse()
                {
                    Message = "Entries of Product were removed successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.AddProductSuccess => new ShopRepositoryResponse()
                {
                    Message = "New product added successfully.",
                    StatusCode = 200
                },
                ShopRepositoryReturn.UpdateSupplierSuccess => new ShopRepositoryResponse()
                {
                    Message = "Supplier updated successfully.",
                    StatusCode = 200
                },
                _ => new ShopRepositoryResponse()
                {
                    Message = "NaN",
                    StatusCode = 200
                }
            };
        }
    }
}