using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CustomSelfFrireWallController
{

    // 24 hours you will keep your laptop with you even in restroom
    // Install police malware software monthly monitoring 
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new OnlyForDevelopment()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
