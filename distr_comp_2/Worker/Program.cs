using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MWCore;

namespace Worker
{
    public class Program
    {
        public static int Node;

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Node number wasn't specified");
            }
            Node = int.Parse(args[0]);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Node == 0
                    ? Topology.Slave1
                    : Topology.Slave2);
    }
}