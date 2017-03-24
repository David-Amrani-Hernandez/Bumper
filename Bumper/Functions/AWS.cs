using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bumper.Functions
{
    public class AWS
    {

        public static Instance getInstanceInfo(string instanceid)
        {
            var ec2Client = new AmazonEC2Client();

            var instanceRequest = new DescribeInstancesRequest();
            instanceRequest.InstanceIds = new List<string>();
            instanceRequest.InstanceIds.Add(instanceid);

            var response = ec2Client.DescribeInstances(instanceRequest);
            return response.Reservations[0].Instances[0];
        }

        public static SecurityGroup getSecurityGroup(string secGroupName)
        {
            var ec2Client = new AmazonEC2Client();

            var dsgRequest = new DescribeSecurityGroupsRequest();
            var dsgResponse = ec2Client.DescribeSecurityGroups(dsgRequest);
            List<SecurityGroup> mySGs = dsgResponse.SecurityGroups;
            foreach (SecurityGroup item in mySGs)
            {
                Console.WriteLine("Existing security group: " + item.GroupId);
                if (item.GroupName == secGroupName)
                {
                    return item;
                }
            }
            return null;

        }

        public static bool getSnapshotfromVolumeId(string volumeId)
        {
            var ec2Client = new AmazonEC2Client();

            var dsRequest = new DescribeVolumesRequest();
            var dsResponse = ec2Client.DescribeVolumes(dsRequest);
            List<Volume> mySnap = dsResponse.Volumes.Where(x => x.VolumeId == volumeId).ToList();

            if (mySnap.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool changeSecurityGroupToQuarantine(string instanceId)
        {
            try
            {
                var ec2Client = new AmazonEC2Client();

                Instance instancem = getInstanceInfo(instanceId);
                SecurityGroup sg = getSecurityGroup(instancem.SecurityGroups[0].GroupName);

                // Revoke ALL inbound rules
                RevokeSecurityGroupIngressRequest dsreq = new RevokeSecurityGroupIngressRequest();
                dsreq.GroupId = sg.GroupId;
                dsreq.IpPermissions = sg.IpPermissions;
                var dsResponse = ec2Client.RevokeSecurityGroupIngress(dsreq);

                //Authorized rule to myip
                IpPermission quarantineIp = new IpPermission();
                quarantineIp.FromPort = 22;
                quarantineIp.ToPort = 22;
                quarantineIp.IpProtocol = "tcp";
                quarantineIp.IpRanges = new List<string> { "2.139.155.201/32", "2.139.155.203/32" };
                quarantineIp.Ipv6Ranges = new List<Ipv6Range>();
                quarantineIp.PrefixListIds = new List<PrefixListId>();
                quarantineIp.UserIdGroupPairs = new List<UserIdGroupPair>();
                AuthorizeSecurityGroupIngressRequest dsnreq = new AuthorizeSecurityGroupIngressRequest();
                dsnreq.GroupId = sg.GroupId;
                dsnreq.IpPermissions = new List<IpPermission> { quarantineIp };
                var dsnResponse = ec2Client.AuthorizeSecurityGroupIngress(dsnreq);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }

        }

        public static bool CreateSnapshot(string instanceId)
        {
            try
            {
                var ec2Client = new AmazonEC2Client();

                string instanceVolumeId = getInstanceInfo(instanceId).BlockDeviceMappings[0].Ebs.VolumeId;

                var nsreq = new CreateSnapshotRequest();
                nsreq.VolumeId = instanceVolumeId;
                nsreq.Description = String.Format("Bumper_SNAPSHOT_{0}", instanceId);
                var nsresp = ec2Client.CreateSnapshot(nsreq);
                string snapid = nsresp.Snapshot.SnapshotId;

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}