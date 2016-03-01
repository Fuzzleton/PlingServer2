using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using System.Net;

namespace PlingAgentCore
{
    class RequestHandler
    {
        public static ConcurrentDictionary<string, Account> cdAccounts = new ConcurrentDictionary<string, Account>();
        public static ConcurrentDictionary<string, ActiveSession> cdActiveSessions = new ConcurrentDictionary<string, ActiveSession>();
        public static ConcurrentDictionary<string, Event> cdActiveEvents = new ConcurrentDictionary<string, Event>();
        public static string JSONHandle(String JSON)
        {


            Request rq = new Request();
            String[] request = JSON.Split(new char[] { '?' }, 2);
            rq.request_type = request[0];
            rq.content = request[1];
            Console.WriteLine("Got the Following request type:" + rq.request_type);
            switch (rq.request_type)
            {
                case "register": return RegisterRequest(rq.content);
                case "host_event": return HostRequest(rq.content);
                case "view_events": return ViewRequest(rq.content);
                case "remove_event": return removeEventRequest(rq.content);
                default: return otherRequest(rq.content);
            }

            string reply = "";
            return reply;
        }





        private static string RegisterRequest(String JSON)
        {
            Console.WriteLine("Received Registier reqeust");

            return codedReturn("0");
        }

        private static string HostRequest(String JSON)
        {
            //handle host request

            Dictionary<string, string> doc = JsonConvert.DeserializeObject<Dictionary<string, string>>(JSON);



            Event e = new Event();
            e.description = doc["description"];
            e.title = doc["title"];
            e.latlong = doc["latlng"];
            e.owner = doc["name"];

            cdActiveEvents.TryAdd(doc["name"], e);

            return codedReturn("0");
        }

        private static string ViewRequest(String JSON)
        {
            if (cdActiveEvents.Count > 0)
            {
                LinkedList<Event> events = new LinkedList<Event>();
                foreach (Event e in cdActiveEvents.Values)
                {
                    events.AddFirst(e);
                }
                return JsonConvert.SerializeObject(events);
            }
            return codedReturn("-1");
        }

        private static string removeEventRequest(String JSON)
        {
            Dictionary<string, string> doc = JsonConvert.DeserializeObject<Dictionary<string, string>>(JSON);
            Event temp;
            cdActiveEvents.TryRemove(doc["name"], out temp);
            if (temp != null)
            {
                return codedReturn("0");
            }
            return codedReturn("-1");
        }

        private static string otherRequest(string content)
        {
            return codedReturn("9");
        }

        private static string codedReturn(string code)
        {
            Dictionary<string, string> reply = new Dictionary<string, string>()
                    { 
                            //This should NOT be possible
                            {"reply",code}
                    };

            return JsonConvert.SerializeObject(reply, Formatting.None);

        }


        private class Request
        {
            public string request_type;
            public string content;
        }
    }
}
