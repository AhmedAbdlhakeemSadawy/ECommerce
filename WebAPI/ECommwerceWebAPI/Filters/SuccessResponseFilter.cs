using ECommwerceWebAPI.Attributes;
using ECommwerceWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommwerceWebAPI.Filters
{
    public class SuccessResponseFilter : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.OfType<SkipSuccessResponseAttribute>().Any())
                return;

            if (context.Result is OkObjectResult okResult && okResult.Value != null)
            {
                var data = okResult.Value;
                var message = GetMessage(context);

                context.Result = new OkObjectResult(new ApiResponse<object>
                {
                    Success = true,
                    Message = message,
                    Data = data
                });
            }
        }


        private string GetMessage(ResultExecutingContext context)
        {
            return context.HttpContext.Request.Method switch
            {
                "POST" => "Created successfully",
                "PUT" => "Updated successfully",
                "DELETE" => "Deleted successfully",
                "GET" => "Retrieved successfully",
                _ => "Success"
            };
        }
    }
}
