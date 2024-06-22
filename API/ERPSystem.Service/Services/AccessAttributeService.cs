using System.IdentityModel.Tokens.Jwt;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataModel.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERPSystem.Service.Services;

public class AccessAttributeService
{
}

public class CheckPermission : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permissionName;

    public CheckPermission(string permissionName)
    {
        _permissionName = permissionName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
       string authorHeader = context.HttpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorHeader) || authorHeader.Length < 7)
        {
            Console.WriteLine($"authHeader data is wrong.");
            Console.WriteLine($"authHeader : {authorHeader}");
            Console.WriteLine($"Route value [Controller] : {context.ActionDescriptor.RouteValues["controller"]}");
            Console.WriteLine($"Route value [API] : {context.ActionDescriptor.RouteValues["action"]}");

            context.Result = new ApiErrorResult(StatusCodes.Status403Forbidden);
            return;
        }

        var token = authorHeader.Remove(0, 7);
        if (token.ToLower() == "token_fake") // FE send fake
        {
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var accountType = tokenS.Claims.First(claim => claim.Type == Constants.ClaimName.Role).Value;
            if (Int32.Parse(accountType) != (short)RoleType.PrimaryManager)
            {
                var accountId = tokenS.Claims.First(claim => claim.Type == Constants.ClaimName.AccountId).Value;

                IRoleService roleService =
                    (IRoleService)context.HttpContext.RequestServices.GetService(typeof(IRoleService));
                var valid = roleService.CheckPermissionEnabled(_permissionName, Int32.Parse(accountId));
                if (valid)
                {
                    return;
                }

                context.Result = new ApiErrorResult(StatusCodes.Status403Forbidden);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"{e.Message} {e.StackTrace} {token}");
        }
    }
}