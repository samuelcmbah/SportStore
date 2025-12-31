using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportStore.ViewModels;
using System.Diagnostics;

namespace SportStore.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;
        public string? OriginalPathAndQuery { get; set; }

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult StatusCodeHadler(int statusCode)
        {

            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            string originalPathAndQuery = "unknown";

            if (statusCodeReExecuteFeature is not null)
            {
                originalPathAndQuery = string.Join(
                    statusCodeReExecuteFeature.OriginalPathBase,
                    statusCodeReExecuteFeature.OriginalPath,
                    statusCodeReExecuteFeature.OriginalQueryString);
            }
            // Log the event
            logger.LogWarning("Status Code {StatusCode} generated for path: {Path}",
                statusCode, originalPathAndQuery);

            switch (statusCode)
            {
                case 404:
                    ViewData["ErrorMessage"] = $"The resource you requested at '{originalPathAndQuery}' could not be found.";
                    return View("NotFound");
                // can add more cases here for 401, 403, etc.
                default:
                    ViewData["ErrorMessage"] = "An error occurred while processing your request.";
                    break;
            }


            return View("NotFound"); //return View("GenericError"); Consider a generic error view for other status codes
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature is not null)
            {
                //log the following
                logger.LogError(exceptionHandlerPathFeature.Error, "An unhandled exception occured on path {Path}. TraceId: {TraceId}", exceptionHandlerPathFeature.Path, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
                
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
