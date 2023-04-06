using GenshinWish.Common;
using GenshinWish.Models.Api;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Service;
using GenshinWish.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace GenshinWish.Attribute
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        private IHttpContextAccessor httpContextAccessor;
        private AuthorizeService authorizeService;
        private RequestRecordService requestRecordService;
        private ApiLimit ApiLimit = ApiLimit.No;
        private PublicLimit PublicLimit = PublicLimit.No;

        public AuthorizeAttribute(IHttpContextAccessor httpContextAccessor, AuthorizeService authorizeService, RequestRecordService requestRecordService, ApiLimit apiLimit = ApiLimit.No, PublicLimit publicLimit = PublicLimit.No)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.authorizeService = authorizeService;
            this.requestRecordService = requestRecordService;
            ApiLimit = apiLimit;
            PublicLimit = publicLimit;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string authCode = context.HttpContext.Request.Headers["authorzation"];
            if (string.IsNullOrWhiteSpace(authCode))
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(ApiResult.Unauthorized);
                return;
            }

            authCode = authCode.Trim();
            if (PublicLimit == PublicLimit.Yes && authCode == ApiConfig.PublicAuthCode && string.IsNullOrWhiteSpace(ApiConfig.PublicAuthCode) == false)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(ApiResult.PermissionDenied);
                return;
            }

            AuthorizePO authorize = authorizeService.GetAuthorize(authCode);
            if (authorize == null || authorize.IsDisable || authorize.ExpireDate <= DateTime.Now)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(ApiResult.Unauthorized);
                return;
            }

            int apiCalledToday = requestRecordService.GetRequestTimesToday(authorize.Id);
            if (ApiLimit == ApiLimit.Yes && apiCalledToday >= authorize.DailyCall)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(ApiResult.ApiMaximum);
                return;
            }

            requestRecordService.AddRecord(httpContextAccessor, context.HttpContext.Request, authorize.Id);
            context.ActionArguments["authorizeDto"] = new AuthorizeDto(authorize, apiCalledToday);
        }






    }
}
