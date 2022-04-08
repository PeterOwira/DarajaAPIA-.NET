using DarajaApi.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Linq;
using System.Web.Http;

namespace DarajaApi.Controllers
{
    public class DarajaController : ApiController
    {
      
        //Encode to Base64 function
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);

        }

        //Access token for Authorization Daraja          
        public static string DarajaAuthentication()
        {
            using (mpesa_setupEntities dbContext = new mpesa_setupEntities())
            {


                string client_id = dbContext.DarajaApiCredentials.FirstOrDefault(e => e.ID == 1).ClientId.ToString();
                string client_secret = dbContext.DarajaApiCredentials.FirstOrDefault(e => e.ID == 1).ClientSecret.ToString();
                string grant_type = dbContext.DarajaApiCredentials.FirstOrDefault(e => e.ID == 1).GrantType.ToString();


                string plaincredentials = client_id + ":" + client_secret;
                string base64credentials = Base64Encode(plaincredentials);
                string authorizationheader = "Basic " + base64credentials;

                string url = "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type="+grant_type;

                var client = new RestClient(url);

                var request = new RestRequest(Method.GET);

                request.AddHeader("Authorization",  authorizationheader);
                IRestResponse response = client.Execute(request);


                var jsonobject = JObject.Parse(response.Content);
                string accesstoken = jsonobject["access_token"].ToString();
                return (accesstoken);
               
            }
        }
         
        }
    }
