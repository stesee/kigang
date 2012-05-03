using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace ClassLibraryDatabase
{
    [Serializable]
    public class ImportExportData
    {
        public int Id { get; set; }
        public string Zeit { get; set; }
        //  public Ueabungsaufgabe Ueabungsaufgabe { get; set; }
        public List<SpecialSkeletonDataImportExport> skeletonData { get; set; }
        [Serializable]
        public class SpecialSkeletonDataImportExport
        {
            public int Id { get; set; }
            public int FrameNr { get; set; }
            public List<SkeletonJointDataImportExport> kigangSkelleton { get; set; }
            public string Timestamp { get; set; }
        }
        [Serializable]
        public class SkeletonJointDataImportExport
        {
            public int Id { get; set; }
            public JointID jointId { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
            public double w { get; set; }
        }
    }
}
