using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        protected HttpResponseMessage PutRequest(string address, object extadd, object obj)
        {
            using (var client = new HttpClient())
            {
                return client.PutAsync(address+extadd.ToString(),
                    new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).Result;
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
