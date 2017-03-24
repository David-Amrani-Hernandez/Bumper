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
        /* ------------------------------------- *
         *                ANALYSIS
         * ------------------------------------- */

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
                            incid.vulnerability = "WeakSecurityGroup";
                            incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                            incid.machine = null;
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
                            incid.vulnerability = "WeakSecurityGroup";
                            incid.machine = null;
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

            if (response.Headers["Content-Security-Policy"] == null)
            {
                incidence incid = new incidence();
                incid.description = String.Format("Missing header Content-Security-Policy.");
                incid.evidence = String.Format("{0}", response.Headers);
                incid.vulnerability = "MissingCSPHeader";
                incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                incid.machine = null;
                incidences.Add(incid);
            }
            else
            {
                List<string> weakHeaders = new List<string>();
                weakHeaders.Add("script-src: *;");
                weakHeaders.Add("default-src: *;");
                weakHeaders.Add("frame-src: *;");

                foreach(string header in weakHeaders)
                {
                    if (response.Headers["Content-Security-Policy"].Contains(header))
                    {
                        incidence incid = new incidence();
                        incid.description = String.Format("Too broad condition for Content-Security-Policy header.");
                        incid.evidence = String.Format("{0}", response.Headers["Content-Security-Policy"]);
                        incid.vulnerability = "TooBroadCSPHeader";
                        incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                        incid.machine = null;
                        incidences.Add(incid);
                    }
                }
            }

            if (response.Headers["X-Frame-Options"] == null && environment.Equals("production"))
            {
                incidence incid = new incidence();
                incid.description = String.Format("Missing header X-Frame-Option.");
                incid.evidence = String.Format("{0}", response.Headers);
                incid.vulnerability = "MissingXFOHeader";
                incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                incid.machine = null;
                incidences.Add(incid);
            }


            return incidences;
        }

        public static List<incidence> AnalyzeSnapshot(string instanceid, string environment)
        {
            List<incidence> incidences = new List<incidence>();

            Instance instance = AWS.getInstanceInfo(instanceid);
            string volumeId = instance.BlockDeviceMappings[0].Ebs.VolumeId;
            if (!AWS.getSnapshotfromVolumeId("aaaa"))
            {
                incidence incid = new incidence();
                incid.description = String.Format("There is no snapshot for instance {0}.", instanceid);
                incid.evidence = String.Format("No snapshots asociated to volume {0}", volumeId);
                incid.vulnerability = "MissingSnapshot";
                incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                incid.machine = null;
                incidences.Add(incid);
            }

            return incidences;
        }


        /* ------------------------------------- *
         *                DISASTER
         * ------------------------------------- */

        public static List<incidence> Quarantine(string instanceid, string environment)
        {
            List<incidence> incidences = new List<incidence>();

            AWS.changeSecurityGroupToQuarantine(instanceid);

            return incidences;
        }

        public static List<incidence> CreateSnapshot(string instanceid, string environment)
        {
            List<incidence> incidences = new List<incidence>();

            if (AWS.CreateSnapshot(instanceid))
            {
                incidence incid = new incidence();
                incid.description = String.Format("Snapshot from volume of instance {0} has been created", instanceid);
                incid.evidence = String.Format("AWS.EC2 Snapshot: BUMPER_SNAPSHOT_DISASTER");
                incid.vulnerability = "DisasterSnapshotCreated";
                incid.id_machine = db.machine.First(x => x.instance == instanceid).id;
                incid.machine = null;
                incidences.Add(incid);
            }

            return incidences;
        }
    }
}