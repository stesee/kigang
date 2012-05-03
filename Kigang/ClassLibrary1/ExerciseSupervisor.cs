using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using ClassLibraryDatabase;

namespace SVT
{
    /**
     * In dieser Klasse befindet sich der Code für die Supervision einer Übungsausführung:
     * Vergleich Übungsausführung mit Referenz aus Datenbank.
     * */
    public class ExerciseSupervisor
    {
        const int USER2CHECK = 1;
        Dictionary<JointID, JointID[]> adjacentJoints=new Dictionary<JointID,JointID[]>();

        public ExerciseSupervisor() { 
            //Define adjacent joints
            adjacentJoints.Add(JointID.ElbowRight,new JointID[]{JointID.WristRight,JointID.ShoulderRight});
            adjacentJoints.Add(JointID.ShoulderRight, new JointID[] { JointID.ElbowRight, JointID.HipRight });
            adjacentJoints.Add(JointID.HipRight, new JointID[] { JointID.ShoulderRight, JointID.KneeRight });
            adjacentJoints.Add(JointID.KneeRight, new JointID[] { JointID.HipRight, JointID.AnkleRight });
            adjacentJoints.Add(JointID.ElbowLeft, new JointID[] { JointID.WristLeft, JointID.ShoulderLeft });
            adjacentJoints.Add(JointID.ShoulderLeft, new JointID[] { JointID.ElbowLeft, JointID.HipLeft });
            adjacentJoints.Add(JointID.HipLeft, new JointID[] { JointID.ShoulderLeft, JointID.KneeLeft });
            adjacentJoints.Add(JointID.KneeLeft, new JointID[] { JointID.HipLeft, JointID.AnkleLeft });           
        }

        public IDictionary<JointID,double> compareJoints(SkeletonFrame skeletonFrame, int frameNr, Bewegungsaufzeichnung recording)
        {
            
            IDictionary<JointID, double> metrics = new Dictionary<JointID, double>();
            KigangContext kigangKontext = new KigangContext();
            ///TODO:
            ///Später muss von Bentuzer gewählte Bewegungsaufzeichnung genommen werden, nicht immer die Erste.
            //Bewegungsaufzeichnung recording=kigangKontext.Therapien.First().Trainingseinheiten.First().Bewegungsaufzeichnung.First();
            if (recording == null) return metrics;                    

            ///TODO: 
            ///Die Suche nach dem zugehörigen Frame könnte sehr langsam sein, wenn es viele Frames gibt
            ///--> evt. mit Dictionary beim Laden indizieren
            SpecialSkeletonData skeletonDataDB = recording.skeletonData.FirstOrDefault(o => o.FrameNr == frameNr);
            if (skeletonDataDB == null) return metrics;                    

            //Check difference for each defined joint
            foreach (JointID jointId in adjacentJoints.Keys)
            {
                //Now get corresponding Frame from reference                   
                Vector[] refJointPos = new Vector[] 
                {Conv.conv2Vector(skeletonDataDB.kigangSkelleton.FirstOrDefault(o => o.jointId.Equals(adjacentJoints[jointId][0]))),
                    Conv.conv2Vector(skeletonDataDB.kigangSkelleton.FirstOrDefault(o => o.jointId.Equals(jointId))),
                    Conv.conv2Vector(skeletonDataDB.kigangSkelleton.FirstOrDefault(o => o.jointId.Equals(adjacentJoints[jointId][1])))
                };

                //Fetch data from Kinect
                SkeletonData skeletonData = skeletonFrame.Skeletons.FirstOrDefault(o => o.TrackingState.Equals(SkeletonTrackingState.Tracked));                
                Vector [] thisJointPos=new Vector[]
                {skeletonData.Joints[adjacentJoints[jointId][0]].Position,
                skeletonData.Joints[jointId].Position,
                skeletonData.Joints[adjacentJoints[jointId][1]].Position};

                double deltaPhi = calcAngleError(thisJointPos,refJointPos);
                metrics[jointId] = deltaPhi;
            }
            
            return metrics;
        }

        public double calcAngleError(Vector[] thisJointPos, Vector[] refJointPos) {            
            double a = euclideanLength(subtract(thisJointPos[1], thisJointPos[0]));
            double b = euclideanLength(subtract(thisJointPos[2], thisJointPos[1]));
            double c = euclideanLength(subtract(thisJointPos[2], thisJointPos[0]));

            double refA = euclideanLength(subtract(refJointPos[1], refJointPos[0]));
            double refB = euclideanLength(subtract(refJointPos[2], refJointPos[1]));
            double refC = euclideanLength(subtract(refJointPos[2], refJointPos[0]));

            double thisPhi = calcAngle(a, b, c);
            double refPhi = calcAngle(refA, refB, refC);
            System.Console.WriteLine("thisPhi: {0:F}, refPhi: {1:F}", rad2Degree(thisPhi), rad2Degree(refPhi));

            //relative error of the angle
            double deltaPhi = (refPhi-thisPhi) / refPhi;
            return deltaPhi;
        }

        public static double calcAngle(double a, double b, double c)
        {
            double angle=Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) /
                            (2 * a * b));
            return angle;
        }

        public static Vector subtract(Vector v1, Vector v2)
        {
            Vector v = new Vector();
            v.W = v1.W - v2.W;
            v.X = v1.X - v2.X;
            v.Y = v1.Y - v2.Y;
            v.Z = v1.Z - v2.Z;

            return v;
        }

        public static double euclideanLength(Vector v)
        {
            return Math.Sqrt(Math.Pow(v.X,2)+Math.Pow(v.Y,2)+Math.Pow(v.Z,2));
        }

        public static double rad2Degree(double rad) {
            return (360 * rad / (2 * Math.PI));
        }


    }
}
