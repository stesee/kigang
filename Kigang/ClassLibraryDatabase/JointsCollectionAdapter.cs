using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace ClassLibraryDatabase
{
    public class Conv
    {
        public static IDictionary<JointID,Joint> toDict(JointsCollection joints)
        {
            IDictionary<JointID, Joint> dict = new Dictionary<JointID, Joint>();
            foreach(Joint joint in joints) {
                dict[joint.ID]=joint;
            }
            return dict;
        }

        public static IDictionary<JointID, Joint> toDict(ICollection<SkeletonJointData> joints)
        {
            IDictionary<JointID, Joint> dict = new Dictionary<JointID, Joint>();
            foreach (SkeletonJointData joint in joints)
            {
                Joint j=new Joint() {
                   ID=(JointID)joint.jointId,
                   Position=conv2Vector(joint),
                   TrackingState=JointTrackingState.Tracked                
                };
                dict[j.ID] = j;
            }
            return dict;
        }

        public static Vector conv2Vector(SkeletonJointData skeletonJointData)
        {
            Vector x = new Vector()
            {
                X = (float)skeletonJointData.x,
                Y = (float)skeletonJointData.y,
                Z = (float)skeletonJointData.z,
                W = (float)skeletonJointData.w
            };
            return x;
        }

    }
}
