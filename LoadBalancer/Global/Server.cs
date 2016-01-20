using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoadBalancer.Global
{
    public static class Server
    {
        public static string MBeansServer = "http://localhost:1010/MBeanServer";
        public static string[] listServer = new string[] { "http://localhost:22591/api/", "http://localhost:22592/api/", "http://conenxt2.azureswebsites.net/api/" };
        public static int serverCount = 3;
    }
}