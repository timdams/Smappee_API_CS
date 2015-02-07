using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmappeeAPITD
{
    public class SmappeeAPI
    {
        //Documentation https://smappee.atlassian.net/wiki/display/DEVAPI/Client+Credentials

        static string baseurl = "https://app1pub.smappee.net/dev/v1/";


        private Token _token;
        private string _client_id, _client_secret, _username, _password;
        public SmappeeAPI(string Client_id, string Client_secret, string username, string password) //Todo add usercred
        {
            _token= new Token();
            _client_id = Client_id;
            _client_secret = Client_secret;
            _username = username;
            _password = password;
        }
        #region Authentication
        static string tokenurl = "oauth2/token/";
        public async  Task RetrieveAccessToken()
        {
            HttpClient wc = new HttpClient();
            //Create url
            string url = string.Format(baseurl + tokenurl + "?grant_type={0}&client_id={1}&client_secret={2}&username={3}&password={4}",
               "password",
                _client_id, _client_secret, _username, _password
                );

            string res = await wc.GetStringAsync(url);
            Debug.WriteLine(res);
            //Parse _token
            _token = JsonConvert.DeserializeObject<Token>(res);
            _token.LastRefresh = DateTime.Now;
        }
        public async void RefreshAccessToken()
        {
            HttpClient wc = new HttpClient();
            //Create url
            string url = string.Format(baseurl + tokenurl + "?grant_type={0}&client_id={1}&client_secret={2}&refresh_token={3}",
                "refresh_token",
                _client_id, _client_secret,
                _token.refresh_token
                );

            string res = await wc.GetStringAsync(url);
            Debug.WriteLine(res);
            //Parse _token
            _token = JsonConvert.DeserializeObject<Token>(res);
            _token.LastRefresh = DateTime.Now;


        }
        #endregion

        #region Api methods

        private static string resourceurl = "https://app1pub.smappee.net/dev/v1/";

        public async Task<ServicelocationOverview> GetServiceLocations()
        {
            HttpClient wc = new HttpClient();
            string url = string.Format(resourceurl + "servicelocation");
            //Add access _token to header
            wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.access_token);
            var res = await wc.GetStringAsync(url);
            Debug.WriteLine(res);
            var result = JsonConvert.DeserializeObject<ServicelocationOverview>(res);

            return result;
        }
        public async Task<ServiceLocationInfo> GetServiceLocationInfo(int servicelocationid)
        {
            HttpClient wc = new HttpClient();
            string url = string.Format(resourceurl + "servicelocation/{0}/info", servicelocationid);
            //Add access _token to header
            wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , _token.access_token);
            var res = await wc.GetStringAsync(url);
            Debug.WriteLine(res);
            var result = JsonConvert.DeserializeObject<ServiceLocationInfo>(res);
            return result;
        }
        public async Task<ConsumptionOverview> GetConsumption(int servicelocationid, DateTime from, DateTime to, Aggregation aggregation)
        {
            HttpClient wc = new HttpClient();
            string url = string.Format(resourceurl + "servicelocation/{0}/consumption?aggregation={1}&from={2}&to={3}",
                servicelocationid,
                Convert.ToInt32(aggregation),
                ConvertToUTCTimestamp(from) * 1000,
                 ConvertToUTCTimestamp(to) * 1000
         
                );
            Debug.WriteLine("Consumption url= " + url);
            //Add access _token to header
            wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , _token.access_token);
            var res = await wc.GetStringAsync(url);
            Debug.WriteLine(res);
            var result = JsonConvert.DeserializeObject<ConsumptionOverview>(res);
            return result;
        }
       
        public async Task<List<Event>> GetEvents(int servicelocationid, List<int> applianceid, DateTime from, DateTime to, long maxnumber = 50)
        {
            HttpClient wc = new HttpClient();
            string url = string.Format(resourceurl + "servicelocation/{0}/events?from={1}&to={2}&maxNumber={3}",
                servicelocationid,
               from.ToFileTimeUtc() / 100000,
               to.ToFileTimeUtc() / 100000,
               maxnumber);
            foreach (var applid in applianceid) //Close one, almost typed Apple ID *yugh*
            {
                url += string.Format("&applianceId={0}", applid);
            }
            Debug.WriteLine("Events url= " + url);
            //Add access _token to header
            wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , _token.access_token);
            var res = await wc.GetStringAsync(url);
            Debug.WriteLine(res);
            var result = JsonConvert.DeserializeObject<List<Event>>(res);
            return result;
        }
        public async void SetActuatorOn(int servicelocationid, int actuatorid, bool turnon) //Todo: implement duration
        {
            HttpClient wc = new HttpClient();
            string url = string.Format(resourceurl + "servicelocation/{0}/actuator/{1}/",
                servicelocationid,
              actuatorid);
            url += (turnon) ? "on" : "off";
            Debug.WriteLine("Actuator url= " + url);

            //Add access _token to header
            wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , _token.access_token);
            //Todo: fill in duration
            HttpContent content = new StringContent("{}");
            var res = await wc.PostAsync(url, content);
            Debug.WriteLine(res);

        }
        #endregion

        #region Helperfunctions
        private static long ConvertToUTCTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (long)span.TotalSeconds;
        }
        #endregion
    }

   
   
    public enum Aggregation { FiveMinValues = 1, Hourly, Daily, Monthly, Yearly };
    internal class Token
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }

        public DateTime LastRefresh { get; set; }

        public override string ToString()
        {
            return string.Format("token {0}, expire {1}, refresh {2}, lastrefre {3}", access_token, expires_in, refresh_token, LastRefresh);
        }
    }
}
