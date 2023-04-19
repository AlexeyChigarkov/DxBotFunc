using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DxBotFunc
{
    internal class ClusterClient
    {
        private const string uriFull = "http://www.dxsummit.fi/api/v1/spots?dx_calls=\"";
        private const string uriPart = "http://www.dxsummit.fi/api/v1/spots?dx_calls=";
        private const string uriCountry = "http://www.dxsummit.fi/api/v1/spots?dx_countries=";
        private const string uriLast = "http://www.dxsummit.fi/api/v1/spots?limit=";

        public async Task<List<SpotObject>> GetSpots(string call)
        {
            var httpLClient = new HttpClient();
            List<SpotObject> list = new List<SpotObject>();

            var uri = call.EndsWith("*") ? uriPart + call.Substring(0,call.Length-1) + "&limit=10" : uriFull + call + "\"&limit=10";

            var response = await httpLClient.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                list = JsonConvert.DeserializeObject<List<SpotObject>>(result);
            }

            return list;

        }

        public async Task<List<SpotObject>> GetCountrySpots(string country)
        {
            var httpLClient = new HttpClient();
            using (httpLClient)
            {
                List<SpotObject> list = new List<SpotObject>();
                var response = await httpLClient.GetAsync(uriCountry + country + "&limit=10");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    list = JsonConvert.DeserializeObject<List<SpotObject>>(result);
                }

                return list;
            }


        }
        public async Task<List<SpotObject>> GetLastSpots(int top)
        {
          top=  top > 100 ? 100 : top;
            var httpLClient = new HttpClient();
            using (httpLClient)
            {
                List<SpotObject> list = new List<SpotObject>();
                var response = await httpLClient.GetAsync(uriLast +top);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    list = JsonConvert.DeserializeObject<List<SpotObject>>(result);
                }

                return list;
            }


        }
    }
}
