using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlingAgentCore;
namespace PlingAgent
{
    class Program
    {
        static int Main(string[] args)
        {
            PlingAgentCore.HttpServer httpServer;
            if (args.GetLength(0) > 0)
            {
                //instanciates Server on specified port
                httpServer = new PlingAgentCore.MyHttpServer(Convert.ToInt16(args[0]));
            }
            else {
                //instanciates Server on default port 8080
                httpServer = new PlingAgentCore.MyHttpServer(8080);
            }
            //Sets up and starts the thread for the HttpServer Listener
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.Start();
            return 0;
        }
        
    }
    
}