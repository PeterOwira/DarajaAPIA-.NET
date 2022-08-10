using System.Web.Http;
using Newtonsoft.Json.Linq;
using RestSharp;
using DarajaApi.Models;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Data;
using Newtonsoft.Json;
using System.Globalization;

namespace DarajaApi.Controllers
{
    public class MpesaExpressController : ApiController
    {
      
        //Getting current TimeStamp
    public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }

        //Lipa na Mpesa Online
        [HttpGet]
        [Route("api/daraja/mpesa_express")]
        public IHttpActionResult MpesaExpress(string phone_no, int amount )
        {
            using (mpesa_setupEntities dbContext = new mpesa_setupEntities())
            {
                //requestparameters
                var shortcode = dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).ShortCode.GetValueOrDefault();
                string passkey= dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).PassKey.ToString();
                string TransDesc= dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).TransactionDescription.ToString();
                string callbackurl = dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).Callbackurl.ToString();
                string AccRef = dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).AccountReference.ToString();

                //Password
                string plainpassword = shortcode + passkey + GetTimestamp(new DateTime());
                string encodepassword = DarajaController.Base64Encode(plainpassword);

                //calback url dynamic because we are using ngrock
                var data_object = new
                {
                    BusinessShortCode = shortcode,
                    Password = encodepassword,
                    Timestamp = GetTimestamp(new DateTime()),
                    TransactionType = "CustomerPayBillOnline",
                    Amount = amount,
                    PartyA = phone_no,
                    PartyB = shortcode,
                    PhoneNumber = phone_no,
                    CallBackURL = "https://9209-41-90-181-55.ngrok.io/api/daraja/callback",
                    AccountReference = AccRef,
                    TransactionDesc = TransDesc
                };

                var data_string = new JavaScriptSerializer().Serialize(data_object);
                var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");

                string token = DarajaController.DarajaAuthentication(1);
                string bearer_token = "Bearer " + token;
                request.AddHeader("Authorization", bearer_token);
                request.AddParameter("application/json", data_string, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return Ok(response.Content);
            }
         
        }


        [HttpPost]
        [Route("api/daraja/callback")]
        public IHttpActionResult StkCallBack ([FromBody] JToken value)
        {
            //write data into a text file
            var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/Callback");
            File.WriteAllText(path + @"/MpesaCallBack" + DateTime.Now.Ticks.ToString() + ".txt", value.ToString());

            //get value data
            var resultcode = value.SelectToken("Body.stkCallback.ResultCode"); 
            string resultcode1 = (string)resultcode;

            //Accepting successful Transaction
            if (resultcode1 == "0")
            {
                var receiptnumber = value.SelectToken("Body.stkCallback.CallbackMetadata.Item[1].Value");
                var Amount = value.SelectToken("Body.stkCallback.CallbackMetadata.Item[0].Value");
                var Phonenumber = value.SelectToken("Body.stkCallback.CallbackMetadata.Item[4].Value"); 
                var T_Date = value.SelectToken("Body.stkCallback.CallbackMetadata.Item[3].Value");

                //Change Transactional Date in DateTime Data
                DateTime theTime = DateTime.ParseExact((string)T_Date,
                                    "yyyyMMddHHmmss",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None);

                //save to the Database
                using (mpesa_setupEntities dbContext = new mpesa_setupEntities())
                {
                    MpesaCallBackData callBackData = new MpesaCallBackData
                    {
                        ReceiptNumber = (string)receiptnumber,
                        Amount = (int)Amount,
                        PhoneNumber =(long)Phonenumber,
                        TransactionDate = theTime
                    };

                    dbContext.MpesaCallBackDatas.Add(callBackData);

                    dbContext.SaveChanges();
                }

            }


       
            return Ok(value);
            

        }


        [HttpGet]
        [Route("api/daraja/expressquery")]
        public IHttpActionResult ExpressQuery(string new_checkout_request_ID)
        {
            using (mpesa_setupEntities dbContext = new mpesa_setupEntities())
            {
                //request parameters
                var shortcode = dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).ShortCode.GetValueOrDefault();
                string passkey = dbContext.MpesaExpressSetups.FirstOrDefault(e => e.ExpressID == 1).PassKey.ToString();

                string plainpassword = shortcode + passkey + GetTimestamp(new DateTime());
                string encodepassword = DarajaController.Base64Encode(plainpassword);

                
                var query_object = new
                {
                    BusinessShortCode = shortcode,
                    Password = encodepassword,
                    Timestamp = GetTimestamp(new DateTime()),
                    CheckoutRequestID = new_checkout_request_ID
                };

                var data_string = new JavaScriptSerializer().Serialize(query_object);


                var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/stkpushquery/v1/query");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string token = DarajaController.DarajaAuthentication(1);
                string bearer_token = "Bearer " + token;
                request.AddHeader("Authorization", bearer_token);
                request.AddParameter("application/json", data_string,ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return Ok(response.Content);

            }
        }

  //      [HttpGet]
  //      [Route("api/daraja/transactionstatus")]
  //      public IHttpActionResult TransactionQuery(string transactionID) {
  //          var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/transactionstatus/v1/query");
  //          var request = new RestRequest(Method.POST);
  //          request.AddHeader("Content-Type", "application/json");
  //          string token = DarajaController.DarajaAuthentication();
  //          string bearer_token = "Bearer "+token;
  //          request.AddHeader("Authorization", bearer_token);
  //          var data_object = new
  //          {
  //  Initiator = "testapi",
  //  SecurityCredential= "fffBKEs0NA+8CVBQPBR0CVSq/bky9CYm4eewq0rJjDwANR962Ne8nbzzAt0QssmGWKNOe7DHEp3iOYs9VrpA8B+LbqMGy2o2gvWrPrKtw9VhVlslB3CvOi6Yc6wyYbND3NfdEWHp3GUAC+GEC0wJS7B2V4prkFJhvsDd5pQz8scfGgzH1CLzUMh5+pX4RABRKGDWPvJUQHBwVq1A+5kPryxvRMMz3Jd6IbrbVelaJ/75KflSpul9exM621Fn41MkGPWAMU6s+POURPaa5GOs+PHGt1MGt9skP/X3Co+14eb5DY0eS5xHuxBJ96G0jJV8HC6EPZP26GAOVQxidDdSww==",
  //  CommandID= "TransactionStatusQuery",
  //  TransactionID= transactionID,
  //  PartyA= 174379,
  //  IdentifierType= "4",
  //  ResultURL= "https://mydomain.com/TransactionStatus/result/",
  //  QueueTimeOutURL= "https://mydomain.com/TransactionStatus/queue/",
  //  Remarks= "Daraja",
  //  Occassion= "daraja",
  //};

  //          var data_string = new JavaScriptSerializer().Serialize(data_object);


  //          request.AddParameter("application/json", data_string, ParameterType.RequestBody);
  //          IRestResponse response = client.Execute(request);
  //          return Ok(response.Content);
  //      }
    }
}
