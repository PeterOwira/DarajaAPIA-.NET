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
    public class B2CController : ApiController
    {
        [HttpGet]
        [Route("api/daraja/b2c")]
        public IHttpActionResult B2c()
        {
            var data_object = new {
                InitiatorName = "testapi",
                SecurityCredential = "",
                CommandID = "SalaryPayment",
                Amount = 1000,
                PartyA = 600998,
                PartyB = 0,
                Remarks = "Test remarks",
                QueueTimeOutURL = "https://cb83-41-90-182-80.ngrok.io/api/daraja/transactionstatus/queue",
                ResultURL = "https://cb83-41-90-182-80.ngrok.io/api/daraja/b2cconfirmation",
                Occassion = "Salary April 2022",
            };

            var data_string = new JavaScriptSerializer().Serialize(data_object);
            var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/b2c/v1/paymentrequest");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            string token = DarajaController.DarajaAuthentication(3);
            string bearer_token = "Bearer " + token;
            request.AddHeader("Authorization", bearer_token);
            request.AddParameter("application/json", data_string,  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return Ok(response.Content);
        }


        [HttpPost]
        [Route("api/daraja/b2cconfirmation")]
        public IHttpActionResult B2CConfimationURL([FromBody] JToken value)
        {
            var response = new
            {
                ResultCode = 0,
                ResultDesc = "Confirmation Received Successfully"
            };

            var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/Confirmation/B2C result");
            File.WriteAllText(path + @"/MpesaB2CConfirmationResponse" + DateTime.Now.Ticks.ToString() + ".txt", value.ToString());

            var data_string = new JavaScriptSerializer().Serialize(response);

            return Ok(data_string);
        }

    }
}
