using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Responses;
using System;
using Nancy.Responses.Negotiation;

namespace Nancy.Demo.Hosting.Self
{
    using Nancy;
    using Nancy.Diagnostics;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        /* protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            System.Console.WriteLine ("ConfigureApplicationContainer");
            container.Register<JsonSerializer, CustomJsonSerializer>();
        } */

        protected override sealed Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration { 
            // let's hack response processor to return XML by default and when asked for text/html (avoid creating views).
            get {
                return NancyInternalConfiguration.WithOverrides ((c) => {
                    c.ResponseProcessors.Clear ();
                    c.ResponseProcessors.Add (typeof(JsonProcessor));
                });
            }
        }

        public override void Configure(Nancy.Configuration.INancyEnvironment environment)
        {
            environment.Tracing (true, true);
            environment.Diagnostics(
                enabled: true,
                password: "password");
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {

            pipelines.OnError.AddItemToEndOfPipeline ((ctx, e) => {
                System.Console.WriteLine ("AddItemToEndOfPipeline");
                return new JsonResponse<Id> (new Id (), new DefaultJsonSerializer (ctx.Environment), ctx.Environment);//.WithStatusCode(ce.StatusCode);
            });
            base.RequestStartup(requestContainer, pipelines, context);
        }
    }
}
