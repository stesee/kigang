using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Author: stefan.seeland@seeland.biz - 2011
namespace KinectSDKDemo
{
    public class FramerateCounter
    {
        #region frameratecounter
        private  int framerate = 0;
        private  int framerateCoutner = 0;
        private  DateTime framerateStart=DateTime.Now;
        public  int incrementFramerateCounter()
        {
            if (DateTime.Compare( framerateStart.AddSeconds(1),DateTime.Now)==1)
            {
                framerateCoutner++; 
            }
            else
            {
                framerate = framerateCoutner;
                framerateCoutner = 0;
                framerateStart = DateTime.Now;

            }
            return framerate;
        }
        #endregion
    }
}
