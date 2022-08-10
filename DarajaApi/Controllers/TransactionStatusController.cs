﻿using System.Web.Http;
using RestSharp;
using System.Web.Script.Serialization;
using System.Web;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DarajaApi.Controllers
{
    public class TransactionStatusController : ApiController
    {
        [HttpGet]
        [Route("api/daraja/transactionstatus")]
        public IHttpActionResult TransactionQuery(string transactionID)
        {
            var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/transactionstatus/v1/query");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");


            string token = DarajaController.DarajaAuthentication(3);
            string bearer_token = "Bearer " + token;
            request.AddHeader("Authorization", bearer_token);

            var data_object = new
            {
                Initiator = "testapi",
                SecurityCredential = "gXEwe7pDAqD0Z9VcAL2WJ2YxnM1fz0R8mQKHuY//jxyRZacf7FAp1SCzo9nSiiCEPfGteRW/EEpMHCvcVROWWdCc94jLxGf//Y2mh3PthmBi+XBGeEda3Afcy70Khdn+zr92FPu1dj9rzSKsPu2pfLsjV5K8UUyXJVv9SYsR3Q4DRR4znyMetgrZres7gyXBDC0Tc+jyWaVsg0dU2l7nuthZ92XH/qYyOFwSHMZg3CNCkxmftr3AozeWD/2cr9F+aINzHRiSVijxIVaBdwJcLDZkddktj6ppZpse3OFX57nEjwuFLxw6viXSGRfmralm9JstdAqSHrCo6y8WRh/01w==",
                CommandID = "TransactionStatusQuery",
                TransactionID = transactionID,
                PartyA = 600997,
                IdentifierType = "4",
                ResultURL = "https://ba0c-41-90-176-0.ngrok.io/api/daraja/transactionstatus/result",
                QueueTimeOutURL = "https://ba0c-41-90-176-0.ngrok.io/api/daraja/transactionstatus/queue",
                Remarks = "Daraja",
                Occassion = "daraja",
            };

            var data_string = new JavaScriptSerializer().Serialize(data_object);


            request.AddParameter("application/json", data_string, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return Ok(response.Content);
        }

        [HttpPost]
        [Route("api/daraja/transactionstatus/queue")]
        public IHttpActionResult Queuetimeout(JToken value)
        {
            var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/QueueTimeout");
            File.WriteAllText(path + @"/QueueTime" + DateTime.Now.Ticks.ToString() + ".txt", value.ToString());

            return Ok(value);
        }


        [HttpPost]
        [Route("api/daraja/transactionstatus/result")]
        public IHttpActionResult TransactionStatusResult(JToken value)
        {
            var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/TransactionResult");
            File.WriteAllText(path + @"/Result" + DateTime.Now.Ticks.ToString() + ".txt", value.ToString());

            return Ok(value);          
        }
    }

}