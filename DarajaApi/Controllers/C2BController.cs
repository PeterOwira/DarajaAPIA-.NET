using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace DarajaApi.Controllers
{
    public class C2BController : ApiController
    {
        [HttpGet]
        [Route("api/daraja/register_url")]
        public IHttpActionResult Registerurl()
        {

            var data_object = new
            {
                ShortCode = "600980",
                ResponseType = "Completed",
                ConfirmationURL = "https://cb83-41-90-182-80.ngrok.io/api/daraja/confirmation",
                ValidationURL = "https://cb83-41-90-182-80.ngrok.io/api/daraja/validation",
            };

            var data_string = new JavaScriptSerializer().Serialize(data_object);
            var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/c2b/v1/registerurl");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            string token = DarajaController.DarajaAuthentication(3);
            string bearer_token = "Bearer " + token;
            request.AddHeader("Authorization", bearer_token);
            request.AddParameter("application/json", data_string, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return Ok(response.Content);
        }

        [HttpPost]
        [Route("api/daraja/confirmation")]
        public IHttpActionResult ConfimationURL([FromBody] JToken value)
        {
            var response = new
            {
                ResultCode = 0,
                ResultDesc = "Confirmation Received Successfully"
            };

            var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/Confirmation");
            File.WriteAllText(path + @"/MpesaConfirmationResponse" + DateTime.Now.Ticks.ToString() + ".txt", value.ToString());

            var data_string = new JavaScriptSerializer().Serialize(response);

            return Ok(data_string);
        }


        [HttpPost]
        [Route("api/daraja/validation")]
        public IHttpActionResult ValidationURL([FromBody] JToken value)
        {
            var response = new
            {
                ResultCode = 0,
                ResultDesc = "Confirmation Received Successfully"
            };

            var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/Validation");
            File.WriteAllText(path + @"/MpesaValidationResponse" + DateTime.Now.Ticks.ToString() + ".txt", value.ToString());

            var data_string = new JavaScriptSerializer().Serialize(response);

            return Ok(data_string);
        }
    }
}
