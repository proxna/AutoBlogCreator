namespace AutoBlogCreator.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandler(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionHandler>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
            }
        }

        private void HandleException(HttpContext context, Exception ex)
        {
            _logger.LogError($"Problem with {context.Request}. Error: {ex.Message}. StackTrace: {ex.StackTrace}");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
