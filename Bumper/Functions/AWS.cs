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
        /* ------------------------------------- *
         *                ANALYSIS
         * ------------------------------------- */

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

    }
}