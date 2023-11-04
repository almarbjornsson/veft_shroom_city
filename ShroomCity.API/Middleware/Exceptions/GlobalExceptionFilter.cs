using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShroomCity.Models.Exceptions;

namespace ShroomCity.API.Middleware.Exceptions;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        var status = HttpStatusCode.InternalServerError; 

        // Using switch expression to match the type of the exception
        var problemDetails = exceptionType switch
        {
            // Match for ArgumentOutOfRangeException
            not null when exceptionType == typeof(ArgumentOutOfRangeException) => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Invalid parameters.",
                Detail = context.Exception.Message,
            },
            not null when exceptionType == typeof(UserAlreadyExistsException) => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "User already exists.",
                Detail = context.Exception.Message,
            },
            
            _ => new ProblemDetails
            {
                Status = (int)status,
                Title = "An unexpected error occurred!",
                Detail = context.Exception.Message, 
            }
        };

        // Set the instance path and the response
        problemDetails.Instance = context.HttpContext.Request.Path;

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
        
        context.ExceptionHandled = true;
    }
}
