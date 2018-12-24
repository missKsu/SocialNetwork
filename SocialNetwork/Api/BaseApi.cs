using Groups.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Posts.Entities;
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
        protected HttpResponseMessage PostRequest(string address, object obj)
        {
            using (var client = new HttpClient())
            {
                return client.PostAsync(address,
                    new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).Result;
            }
        }

        protected HttpResponseMessage GetRequest(string address)
        {
            using (var client = new HttpClient())
            {
                return client.GetAsync(address).Result;
            }
        }

        protected HttpResponseMessage PutRequest(string address, object extadd, Group obj)
        {
            using (var client = new HttpClient())
            {
                //return client.PutAsync(address + extadd.ToString(), new StringContent(JsonConvert.SerializeObject(obj), UnicodeEncoding.UTF8, "application/json")).Result;
                return client.PutAsJsonAsync<Group>(address+extadd.ToString(), obj).Result;
            }
        }

        protected HttpResponseMessage PutRequest(string address, object extadd, Post obj)
        {
            using (var client = new HttpClient())
            {
                //return client.PutAsync(address + extadd.ToString(), new StringContent(JsonConvert.SerializeObject(obj), UnicodeEncoding.UTF8, "application/json")).Result;
                return client.PutAsJsonAsync<Post>(address + extadd.ToString(), obj).Result;
            }
        }

        protected HttpResponseMessage PutRequest(string address, object extadd, string obj)
        {
            using (var client = new HttpClient())
            {
                //return client.PutAsync(address + extadd.ToString(), new StringContent(JsonConvert.SerializeObject(obj), UnicodeEncoding.UTF8, "application/json")).Result;
                return client.PutAsJsonAsync<string>(address + extadd.ToString(), obj).Result;
            }
        }

        protected HttpResponseMessage DeleteRequest(string address, object extadd)
        {
            using (var client = new HttpClient())
            {
                return client.DeleteAsync(address + extadd.ToString()).Result;
            }
        }
    }
}
