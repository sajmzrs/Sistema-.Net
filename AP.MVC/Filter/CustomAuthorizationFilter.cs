using System;
using AP.Repositories;
using System.Web.Mvc;

namespace AP.MVC.Filter
{
    public class CustomAuthorizationFilter : FilterAttribute, System.Web.Mvc.IAuthorizationFilter
    {
        private readonly string _requiredAction;

        public CustomAuthorizationFilter()
        {
        }

        public CustomAuthorizationFilter(string requiredAction)
        {
            _requiredAction = requiredAction;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            if (string.IsNullOrWhiteSpace(_requiredAction))
                return;

            string userEmail = filterContext.HttpContext.User.Identity.Name;
            IPermissionsRepository repository = new PermissionsRepository();

            if (!repository.HasPermission(userEmail, _requiredAction))
            {
                filterContext.Result = new HttpUnauthorizedResult("No tiene permiso para realizar esta accion.");
            }
        }
    }
}
