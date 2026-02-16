using System.Net;
using ContactManagerApp.Exceptions;

namespace ContactManagerApp.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
			catch (CsvValidationException ex)
			{
				_logger.LogWarning(ex, ex.Message);
				await HandleExceptionAsync(context, ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error");
				await HandleExceptionAsync(context, ex);
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = exception switch
			{
				CsvValidationException => (int)HttpStatusCode.BadRequest,
				_ => (int)HttpStatusCode.InternalServerError
			};

			context.Response.StatusCode = code;
			context.Response.ContentType = "application/json";

			var response = new
			{
				success = false,
				message = code == 400 ? exception.Message : "Internal server error."
			};

			return context.Response.WriteAsJsonAsync(response);
		}
	}
}
