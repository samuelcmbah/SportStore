using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportStore.ViewModels;
using System.Diagnostics;

namespace SportStore.Controllers
{
    public class ErrorController : Controller
    {
        public string? OriginalPathAndQuery { get; set; }

        [Route("Error/{statusCode}")]
        public IActionResult StatusCodeHadler(int statusCode)
        {

            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeReExecuteFeature is not null)
            {
                OriginalPathAndQuery = string.Join(
                    statusCodeReExecuteFeature.OriginalPathBase,
                    statusCodeReExecuteFeature.OriginalPath,
                    statusCodeReExecuteFeature.OriginalQueryString);
            }
            switch (statusCode)
            {
                case 404:
                    ViewData["ErrorMessage"] = $"The resourse you requested \"{ OriginalPathAndQuery}\" could not be found.";
                    break;
            }
           
            return View("NotFound");
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
                //exceptionHandlerPathFeature.Path;
                //exceptionHandlerPathFeature.Error.Message;
                //exceptionHandlerPathFeature.Error.StackTrace;
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
