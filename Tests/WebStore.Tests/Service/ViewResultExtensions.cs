using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebStore.Tests.Service
{
    public static class ViewResultExtensions
    {
        public static string ToHtml(this ViewResult Result, HttpContext HttpContext)
        {
            var feature = HttpContext.Features.Get<IRoutingFeature>();
            var route_data = feature.RouteData;
            var view_name = Result.ViewName ?? route_data.Values["action"] as string;
            var action_context = new ActionContext(HttpContext, route_data, new ControllerActionDescriptor());
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<MvcViewOptions>>();
            var value_html_helper_options = options.Value.HtmlHelperOptions;
            var view_engine_result = Result.ViewEngine?.FindView(action_context, view_name, true) ?? options.Value.ViewEngines.Select(x => x.FindView(action_context, view_name, true)).FirstOrDefault(x => x != null);
            var view = view_engine_result.View;
            var builder = new StringBuilder();

            using (var output = new StringWriter(builder))
                view
                   .RenderAsync(new ViewContext(action_context, view, Result.ViewData, Result.TempData, output, value_html_helper_options))
                   .GetAwaiter()
                   .GetResult();

            return builder.ToString();
        }
    }
}