using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NATUPNPLib;
using NETCONLib;
using NetFwTypeLib;
using System.Net.Security;
using System.Net;


// block all these mother fuckers dont leave any port open FBI CIA they have no right
// they are more interested to take your code models innovations like test generation script model

namespace CustomSelfFrireWallController
{
    public partial class OnlyForDevelopment : ServiceBase
    {
        // every 0.5 second 
        Timer aTimer = new Timer(500);

        #region "Detect all connected Ips to network block them"


        private static bool AuthorizeApplication(string title, string applicationPath,
             NET_FW_SCOPE_ scope, NET_FW_IP_VERSION_ ipVersion)
        {

            string PROGID_AUTHORIZED_APPLICATION = System.Configuration.ConfigurationManager.AppSettings["PROGID_AUTHORIZED_APPLICATION"];

            // Create the type from prog id
            Type type = Type.GetTypeFromProgID(PROGID_AUTHORIZED_APPLICATION);
            INetFwAuthorizedApplication auth = Activator.CreateInstance(type)
                as INetFwAuthorizedApplication;
            auth.Name = title;
            auth.ProcessImageFileName = applicationPath;
            auth.Scope = scope;
            auth.IpVersion = ipVersion;
            // Unauthorize notepad to connect to internet 
            // Unauthorize wordpad / winword to connect to internet
            // Unauthorize each tempfile to connect to internet.
            auth.Enabled = false;
         
            INetFwMgr manager = GetFirewallManager();
            try
            {
                manager.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(auth);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        #endregion





        public OnlyForDevelopment()
        {
            InitializeComponent();
        }

        private static NetFwTypeLib.INetFwMgr GetFirewallManager()
        {
            string _clsid = System.Configuration.ConfigurationManager.AppSettings["CLSID_FIREWALL"];
            Type objectType = Type.GetTypeFromCLSID(
                  new Guid(_clsid));
            return Activator.CreateInstance(objectType)
                  as NetFwTypeLib.INetFwMgr;


        }

        #region "update the event"
        private static void BlockAllOutgoingConnections()
        {
      
             INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
             Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            firewallRule.Description = "Used to block all internet access.";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = "Block Internet";

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);
        } 






        #endregion

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {

            INetFwMgr manager = GetFirewallManager();

            bool isFirewallEnabled =
                manager.LocalPolicy.CurrentProfile.FirewallEnabled;

            INetFwProfile profileForDeskTop = manager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_CURRENT);
            // Identify what profile group trying to connect 
             profileForDeskTop.FirewallEnabled = true;
           // the above will enable firewall for the connected domain / current profile.
           
            if (isFirewallEnabled == false)
                manager.LocalPolicy.CurrentProfile.FirewallEnabled = true;
            // the above will enable the firewall if its turned off by hacker
            // Disable to access internet make auth disabled if its turned on by hacker

            
            AuthorizeApplication("Notepad", @"C:\Windows\Notepad.exe",
                NET_FW_SCOPE_.NET_FW_SCOPE_ALL,
                NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY);

            AuthorizeApplication("WinWord", @"C:\Program Files\Microsoft Office 15\root\office15\winword.exe",
                NET_FW_SCOPE_.NET_FW_SCOPE_ALL,
                NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY);
            

            // Stop any file to make 
            try
            {
                FileInfo info = null;
                foreach (string path in Directory.EnumerateFiles(@"C:\Users\rqadri\AppData\Local\Temp\"))
                {

                    info = new FileInfo(path);
                    if(info.Extension == "tmp" || info.Extension == "tr0" || info.Extension == "4g3")
                    {
                       AuthorizeApplication("ANY", path, NET_FW_SCOPE_.NET_FW_SCOPE_ALL,
                       NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY);
                        File.Delete(path);

                    }

                }


            }
            catch (Exception excp)
            {


            }

     

            BlockAllOutgoingConnections();
            


        }


        protected override void OnStart(string[] args)
        {
            aTimer = new System.Timers.Timer(500);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;




        }




        protected override void OnStop()
        {
        }
    }
}
