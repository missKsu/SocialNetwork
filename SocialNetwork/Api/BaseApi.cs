using Groups.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Posts.Entities;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Api
{
    public class BaseApi
    {
        protected bool Authorized = false;
        protected string token;

        protected string Authorize(string address, string login, string pass)
        {
            var data = new Models.Auth { Login = login, Pass = pass };
            var response = PostRequest($"{address}/auth", data);
            if (!response.IsSuccessStatusCode)
                return null;
            string token = response.Content.ReadAsStringAsync().Result;
            return token;
        }

        protected HttpResponseMessage PostRequest(string address, object obj)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return client.PostAsync(address,
                        new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).Result;
                }
                catch
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        protected HttpResponseMessage GetRequest(string address)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return client.GetAsync(address).Result;
                }
                catch
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        protected HttpResponseMessage PutRequest(string address, object extadd, Group obj)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return client.PutAsJsonAsync<Group>(address + extadd.ToString(), obj).Result;
                }
                catch
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        protected HttpResponseMessage PutRequest(string address, object extadd, Post obj)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return client.PutAsJsonAsync<Post>(address + extadd.ToString(), obj).Result;
                }
                catch
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        protected HttpResponseMessage PutRequest(string address, object extadd, string obj)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return client.PutAsJsonAsync<string>(address + extadd.ToString(), obj).Result;
                }
                catch
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        protected HttpResponseMessage DeleteRequest(string address, object extadd)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return client.DeleteAsync(address + extadd.ToString()).Result;
                }
                catch
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }
    }
}
