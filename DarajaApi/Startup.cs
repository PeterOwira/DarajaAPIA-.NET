//using System;
//using Microsoft.Owin;
//using Owin;
//using Microsoft.Owin.Security.OAuth;
//using System.Web.Http;

//[assembly: OwinStartup(typeof(DarajaApi.App_Start.Startup))]

//namespace DarajaApi.App_Start
//{

//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

//            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
//            {
//                AllowInsecureHttp = true,


//                TokenEndpointPath = new PathString("/token"),

//                //Setting the Token Expired Time (30 minutes)
//                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),

//                //MyAuthorizationServerProvider class will validate the user credentials
//                //Provider = new MyAuthorizationServerProvider()
//            };

//            //Token Generations
//            app.UseOAuthAuthorizationServer(options);
//            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

//            HttpConfiguration config = new HttpConfiguration();
//            WebApiConfig.Register(config);
//        }
//    }
//}
