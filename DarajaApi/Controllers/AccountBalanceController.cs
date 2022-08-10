//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;
//using System.Web.Script.Serialization;

//namespace DarajaApi.Controllers
//{
//    public class AccountBalanceController : ApiController
//    {
//        [HttpGet]
//        [Route("api/daraja/register_url")]
//        public IHttpActionResult AccBal()
//        {
//            var client = new RestClient("https://sandbox.safaricom.co.ke/mpesa/accountbalance/v1/query");
//            var request = new RestRequest(Method.POST);
//            request.AddHeader("Content-Type", "application/json");
//            request.AddHeader("Authorization", "Bearer f4DQuCg0VpxwKu7R40EA0Hf8CLks");
//            var data_object = new{
//                Initiator= "testapi",
//    SecurityCredential= "OO1oXfMRC88dsQK4Z1P1h4owaz5hz5wRKYqo2gzqJffs9qFeoHfEmKHZXMYD3QRebj1j3LHhORYv/dlDSopd5oASapxT6xT37H4jLSYtO7ZBwT/zANYcrMo2aojjNKOdNKKpn2QxvOA2lUFhyWQT0MHwCrH6I+60yj+OCka4apEN+7XFKdYiv33iViOCa6ZTqu/6tNzWT8jufDzN8fOA55R7x774bAUUKbwJlseK8iOledVJTtX5Wsw2TPPQoPSP17cJQDDQqoorGNhOE7oXHqHGnvgKWU9fQLcbxdnB4Qvr+2yRI3ijOf99KAYJva88l1gyu3gSNa6PHtI71DXe4Q==",
//    CommandID= "AccountBalance",
//    PartyA= 600981,
//    IdentifierType= "4",
//    Remarks= "Daraja",
//    QueueTimeOutURL= "https://mydomain.com/AccountBalance/queue/",
//    ResultURL= "https://mydomain.com/AccountBalance/result/",
//  };
//            var data_string = new JavaScriptSerializer().Serialize(data_object);
//            request.AddParameter("application/json", data_string,ParameterType.RequestBody);
//            IRestResponse response = client.Execute(request);
//            return Ok(response.Content);
//        }

//    }
//}