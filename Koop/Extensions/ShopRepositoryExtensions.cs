using Koop.Models.RepositoryModels;

namespace Koop.Extensions
{
    public enum ShopRepositoryReturn
    {
        RemoveUserOrderSucess,
        RemoveUserOrderErrEmptyObject,
    }
    
    public static class ShopRepositoryExtensions
    {
        public static ShopRepositoryResponse ToObject(this ShopRepositoryReturn shopRepositoryReturn)
        {
            return shopRepositoryReturn switch
            {
                ShopRepositoryReturn.RemoveUserOrderSucess => new ShopRepositoryResponse()
                {
                    Message = "Order removed successfully.",
                    ErrCode = 200
                },
                ShopRepositoryReturn.RemoveUserOrderErrEmptyObject => new ShopRepositoryResponse()
                {
                    Message = "The order does not exist in the database.",
                    ErrCode = 500
                },
                _ => new ShopRepositoryResponse()
                {
                    Message = "NaN",
                    ErrCode = 200
                }
            };
        }
    }
}