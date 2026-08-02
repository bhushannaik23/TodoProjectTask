namespace ToDoList.Middleware;

using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using ToDoList.Exceptions;
using ToDoList.Models;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            //   _logger.LogError(exception, exception.Message);

            //_logger.LogError(
            //  exception,
            //  "Unhandled exception. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
            //   context.TraceIdentifier,
            //   context.Request.Method,
            //   context.Request.Path);

            switch (exception)
            {
                case ResourceNotFoundException:
                case BadRequestException:
                case ValidationException:
                    _logger.LogWarning(exception, exception.Message);
                    break;

                default:
                    _logger.LogError(
                        exception,
                        "Unhandled exception. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                        context.TraceIdentifier,
                        context.Request.Method,
                        context.Request.Path);
                    break;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var response = exception switch
        {
            ResourceNotFoundException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message
            },

            BadRequestException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message
            },

            ValidationException ex => new ApiErrorResponse
            {

                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed.",
                Errors = ex.Errors.Select(e => e.ErrorMessage)
            },

            _ => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An unexpected error occurred."
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
