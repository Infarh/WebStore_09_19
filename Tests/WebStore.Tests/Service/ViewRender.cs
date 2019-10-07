using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace WebStore.Tests.Service
{
    //https://gist.github.com/ahmad-moussawi/1643d703c11699a6a4046e57247b4d09
    public class ViewRender
    {
        private readonly IRazorViewEngine _ViewEngine;
        private readonly ITempDataProvider _TempDataProvider;
        private readonly IServiceProvider _ServiceProvider;

        public ViewRender(
            IRazorViewEngine ViewEngine,
            ITempDataProvider TempDataProvider,
            IServiceProvider ServiceProvider)
        {
            _ViewEngine = ViewEngine;
            _TempDataProvider = TempDataProvider;
            _ServiceProvider = ServiceProvider;
        }

        public string Render<TModel>(string ViewName, TModel ViewModel)
        {
            var action_context = GetActionContext();

            var view_engine_result = _ViewEngine.FindView(action_context, ViewName, false);

            if (!view_engine_result.Success)
                throw new InvalidOperationException($"Couldn't find view '{ViewName}'");

            var view = view_engine_result.View;

            using var output = new StringWriter();
            var view_context = new ViewContext(
                action_context,
                view,
                new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = ViewModel
                },
                new TempDataDictionary(action_context.HttpContext, _TempDataProvider),
                output,
                new HtmlHelperOptions());

            view.RenderAsync(view_context).GetAwaiter().GetResult();

            return output.ToString();
        }

        private ActionContext GetActionContext()
        {
            var http_context = new DefaultHttpContext();
            http_context.RequestServices = _ServiceProvider;
            return new ActionContext(http_context, new RouteData(), new ActionDescriptor());
        }
    }

    public class ViewRenderService
    {
        private readonly IRazorViewEngine _ViewEngine;
        private readonly IHttpContextAccessor _HttpContextAccessor;

        public ViewRenderService(
            IRazorViewEngine ViewEngine, 
            IHttpContextAccessor HttpContextAccessor)
        {
            _ViewEngine = ViewEngine;
            _HttpContextAccessor = HttpContextAccessor;
        }

        public Task<string> Render(string ViewPath) => RenderAsync(ViewPath, string.Empty);

        public async Task<string> RenderAsync<TModel>(string ViewPath, TModel model)
        {
            var view_engine_result = _ViewEngine.GetView("~/", ViewPath, false);

            if (!view_engine_result.Success)
                throw new InvalidOperationException($"Couldn't find view {ViewPath}");

            var view = view_engine_result.View;

            await using var output = new StringWriter();
            var view_context = new ViewContext
            {
                HttpContext = _HttpContextAccessor.HttpContext,
                ViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {Model = model},
                Writer = output
            };

            await view.RenderAsync(view_context);

            return output.ToString();
        }
    }
}
