using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Bumper.Functions
{
    public class Plugins
    {

        public static BumperEntities db = new BumperEntities();

        public static List<incidence> AnalyzeSecurityGroup(string instanceid, string environment)
        {
            List<incidence> incidences = new List<incidence>();
            // Vulnerable configuration for PRODUCTION
            List<string> secProIpRange = new List<string> { "0.0.0.0", "::/0", "0.0.0.0/0", "::/0" };
            List<int> secProPortRange = new List<int> { 0, 23, 3306, 27017, 1433, 3389 };
            // Vulnerable configuration for DEBUG
            List<string> secDebugIpRange = new List<string> { "0.0.0.0", "::/0", "/16" };
            List<int> secDebugPortRange = new List<int> { 0, 23, 80, 443, 8080, 3306, 27017, 1433, 3389 };

            Instance instance = AWS.getInstanceInfo(instanceid);
            SecurityGroup secGroupName = AWS.getSecurityGroup(instance.SecurityGroups[0].GroupName);
            if (environment.Equals("production"))
            {
                foreach (IpPermission rule in secGroupName.IpPermissions)
                {
                    foreach (string ruleip in rule.IpRanges)
                    {
                        if (secProIpRange.Contains(ruleip) && (secProPortRange.Contains(rule.FromPort) || secProPortRange.Contains(rule.ToPort)))
                        {
                            incidence incid = new incidence();
                            incid.evidence = String.Format("Security Group:{0} > IP:{1} FROM:{2} TO:{3}", secGroupName.GroupName, ruleip, rule.FromPort, rule.ToPort);
                            incid.description = String.Format("Too broad permissions on production environment. Ports should be filtered to public addresses.");
                            incid.vulnerability = "SecurityGroup";
                            incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                            incid.machine = db.machine.First(x => x.instance == instanceid);
                            incidences.Add(incid);
                        }
                    }
                }
            }
            else if (environment.Equals("debug"))
            {
                foreach (IpPermission rule in secGroupName.IpPermissions)
                {
                    foreach (string ruleip in rule.IpRanges)
                    {
                        if (secProIpRange.Contains(ruleip) && (secDebugPortRange.Contains(rule.FromPort) || secDebugPortRange.Contains(rule.ToPort)))
                        {
                            incidence incid = new incidence();
                            incid.evidence = String.Format("Security Group:{0} > IP:{1} FROM:{2} TO:{3}", secGroupName.GroupName, ruleip, rule.FromPort, rule.ToPort);
                            incid.description = String.Format("Too broad permissions on debug environment. Ports should be filtered to public addresses.");
                            incid.vulnerability = "SecurityGroup";
                            incid.machine = db.machine.First(x => x.instance == instanceid);
                            incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                            incidences.Add(incid);
                        }
                    }
                }
            }

            return incidences;
        }

        public static List<incidence> AnalyzeHeaders(string instanceid, string environment)
        {
            List<incidence> incidences = new List<incidence>();

            string uri = db.machine.First(x => x.instance == instanceid).domain;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.Headers["Content-Security-Policy"].Count() > 0)
            {

            }

            if (response.Headers["X-Frame-Options"].Count() > 0)
            {

            }


            return incidences;
        }

    }
}