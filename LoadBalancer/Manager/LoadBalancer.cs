using LoadBalancer.MBeans;
using NetMX;
using NetMX.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace LoadBalancer.Manager
{
    public class LoadBalancer
    {
        public static IMBeanServer server;
        public static INetMXConnectorServer connectorServer;
        public static Counter server1Counter;
        public static Counter server2Counter;
        public static Counter server3Counter;
        public static ObjectName name;
        public static ObjectName name2;
        public static ObjectName name3;
        private static bool initializedServerMBean = false;
        public static bool initializeServerMBean()
        {
            if (!initializedServerMBean)
            {
                server = MBeanServerFactory.CreateMBeanServer();
                server1Counter = new Counter();
                server2Counter = new Counter();
                server3Counter = new Counter();
                name = new ObjectName("server1Counter:");
                name2 = new ObjectName("server2Counter:");
                name3 = new ObjectName("server3Counter:");
                server.RegisterMBean(server1Counter, name);
                server.RegisterMBean(server2Counter, name2);
                server.RegisterMBean(server3Counter, name3);
                Uri serviceUrl = new Uri(Global.Server.MBeansServer);
                connectorServer = NetMXConnectorServerFactory.NewNetMXConnectorServer(serviceUrl, server);
                connectorServer.Start();
                initializedServerMBean = true;
            }
            return true;

        }
        public static bool checkController(string controller)
        {
            if (controller == "agencies" || controller == "actions")
                return true;
            else
                return false;
        }
        public static bool checkMethod(string method)
        {
            if (method == "get")
                return true;
            else
                return false;
        }
        public static HttpResponseMessage makeRequest(int server, string controller, string method = "get")
        {
            if (!checkController(controller))
                throw new Exception("Wrong Controller");
            if (!checkMethod(method))
                throw new Exception("Wrong Method");
            if (server >= Global.Server.serverCount)
                throw new Exception("Wrong Server");
            string targetUri = Global.Server.listServer[server]+ controller;
            WebRequest request = WebRequest.Create(targetUri);

            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.

            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
            //return responseFromServer;
            return new HttpResponseMessage()
            {
                Content = new StringContent(
            responseFromServer,
            Encoding.UTF8,
            "text/html"
            )
            };
        }
        public static HttpResponseMessage balancerRequest(string controller, string method)
        {
            Uri serviceUrl = new Uri("http://localhost:1010/MBeanServer");
            INetMXConnector connector = NetMXConnectorFactory.Connect(serviceUrl, null);
            IMBeanServerConnection remoteServer = connector.MBeanServerConnection;
            object counterServer1 = remoteServer.GetAttribute(name, "Value");
            object counterServer2 = remoteServer.GetAttribute(name2, "Value");
            object counterServer3 = remoteServer.GetAttribute(name3, "Value");


            int serverSelect = getMinBetween3((int)counterServer1,(int)counterServer2,(int)counterServer3);
            try {
                return makeRequest(serverSelect, controller, method);
            }
            catch(WebException e)
            {
                if(e.Status == WebExceptionStatus.Timeout)
                {
                    //Traiter le timeout error
                }
                if(e.Status == WebExceptionStatus.ConnectFailure)
                {
                    //Traiter server not responding
                }
                return null;   
            }
        }

        public static int getMinBetween3(int i, int j, int k)
        {
            if (i <= j && i <= k)
                return 0;
            else if (j <= i && j <= k)
                return 1;
            else
                return 2;
        }
        public static int getMinBetween2(int j, int k)
        {
            if (j <= k)
                return 1;
            else
                return 2;
        }
    }
}