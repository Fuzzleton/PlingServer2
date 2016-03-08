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
        public static ConcurrentDictionary<string, Event> cdActiveEvents = new ConcurrentDictionary<string, Event>();

        //Base method for handling request, switches request to appropriate methods as appropriate
        public static string HandleRequest(String strRequest)
        {
            Request rq = new Request();
            //Splits the Request at the ? to get the request type [0] and the content [1]
            String[] request = strRequest.Split(new char[] { '?' }, 2);
            rq.request_type = request[0];
            rq.content = request[1];
            

            //switch to handle which action to perform
            switch (rq.request_type)
            {
                case "register": return RegisterRequest(rq.content);
                case "host_event": return HostRequest(rq.content);
                case "view_events": return ViewRequest(rq.content);
                case "remove_event": return removeEventRequest(rq.content);
                default: return otherRequest(rq.content);
            }
        }

        //Handles Registering requests
        private static string RegisterRequest(String JSON)
        {
            //returns successful action to confirm connection with client
            return codedReturn("0");
        }

        //Handles hosting events
        private static string HostRequest(String strJSON)
        {
            //Converts JSON content to usable dictionary
            Dictionary<string, string> dctJSON = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJSON);
           
            //Builds Event Object, fillls it with details from JSON
            Event e = new Event();
            e.description = dctJSON["description"];
            e.title = dctJSON["title"];
            e.latlong = dctJSON["latlng"];
            e.owner = dctJSON["name"];

            //adds event to dictionary of active events
            cdActiveEvents.TryAdd(dctJSON["name"], e);

            //returns successful notification
            return codedReturn("0");
        }

        //gathers the list of events and returns them
        private static string ViewRequest(String strJSON)
        {
            //checks to ensure there is atleast one event to display
            if (cdActiveEvents.Count > 0)
            {
                //builds a temporary linkedlist
                LinkedList<Event> events = new LinkedList<Event>();
                
                foreach (Event e in cdActiveEvents.Values)
                {
                    //adds the event to the tempory list
                    events.AddFirst(e);
                }
                //Sends back the list as a JSON Array
                return JsonConvert.SerializeObject(events);
            }
            //returns unsuccessful action
            return codedReturn("-1");
        }
        //handles removal of events
        private static string removeEventRequest(String strJSON)
        {
            Dictionary<string, string> dctJSON = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJSON);
            Event temp;
            //Tries to remove the requested item
            cdActiveEvents.TryRemove(dctJSON["name"], out temp);
            //if the returned item isn't null, actionwas successful
            if (temp != null)
            {
                return codedReturn("0");
            }
            //if it was null, it did not work/could not be found
            return codedReturn("-1");
        }
        //handles additional requests
        private static string otherRequest(string content)
        {
            
            return codedReturn("9");
        }


        //Returns a pre-formatted JSON String
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
