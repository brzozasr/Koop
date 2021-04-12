using System;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetTopologySuite.Operation.Overlay.Validate;

namespace Koop.Models.Util
{
    public enum OrderStatuses
    {
        Zaplanowane,
        Otwarte,
        Zamknięte,
        Anulowane,
        Archiwalne
    }

    /*public class AuthorizeRefresh : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they user is authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // Otherwise redirect to 
                filterContext.Result = new RedirectResult("user/refreshToken");
            }
        }
    }*/
}