namespace SportStore.Middleware
{
    public class RoleBasedRootRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedRootRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true &&
                    (context.Request.Path == "/" || 
                    context.Request.Path == "/Home/Index" ||
                    context.Request.Path == "/Home"))
            {
                if (context.User.IsInRole("Administrator"))
                {
                    context.Response.Redirect("/Admin/Products");
                    return;
                }
            }

            await _next(context);
        }
    }

}
