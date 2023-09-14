using System;
using RestSharp;
using RestSharp.Authenticators;

namespace ForumApi.Services
{
    public class MailServices
    {
        private static string apikey = "NOOOO key for uuu";
        private static string maaail = "Nooooo";

        public async Task<RestResponse> SendSimpleMessage(string messages, string mail)
        {
            RestClient client = new RestClient("https://api.mailgun.net/v3");

            client.Authenticator = new HttpBasicAuthenticator("api", apikey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", maaail, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"Excited User <mailgun@{maaail}>");
            request.AddParameter("to", mail);
            request.AddParameter("to", $"YOU@{maaail}");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", $"{messages}");
            request.Method = Method.Post;

            return await client.ExecuteAsync(request);
        }
    }
}
