using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;

namespace KinectSDKDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool pause = false;
        Runtime _nui;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _nui = new Runtime();

            _nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(Nui_VideoFrameReady);
            _nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Nui_SkeletonFrameReady);

            try
            {
                _nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
                _nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            System.Windows.Data.CollectionViewSource jointViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource jointViewSource1 = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource1")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource1.Source = [generic data source]
            System.Windows.Data.CollectionViewSource jointViewSource2 = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource2")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource2.Source = [generic data source]
            System.Windows.Data.CollectionViewSource jointViewSource3 = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource3")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource3.Source = [generic data source]
            System.Windows.Data.CollectionViewSource jointViewSource4 = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource4")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource4.Source = [generic data source]
            System.Windows.Data.CollectionViewSource jointViewSource5 = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource5")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource5.Source = [generic data source]
            System.Windows.Data.CollectionViewSource jointViewSource6 = ((System.Windows.Data.CollectionViewSource)(this.FindResource("jointViewSource6")));
            // Load data by setting the CollectionViewSource.Source property:
            // jointViewSource6.Source = [generic data source]
        }

        FramerateCounter framerateCounterVideoFrames = new FramerateCounter();
        void Nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            var image = e.ImageFrame.Image;
            labelVideoFrames.Content = "Videoframes/s: " + framerateCounterVideoFrames.incrementFramerateCounter();
            img.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
        }
        FramerateCounter framerateCounterTrackedSkeletonFrames = new FramerateCounter();
        FramerateCounter framerateCounterSkeletonFrames = new FramerateCounter();
        void Nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
            canvas.Children.Clear();
            labelSkeletonFrames.Content = "Skeletonframes/s: " + framerateCounterSkeletonFrames.incrementFramerateCounter();
            foreach (SkeletonData user in e.SkeletonFrame.Skeletons)
            {
                if (user.TrackingState == SkeletonTrackingState.Tracked)
                {
                    var now = DateTime.Now;
                    if (!pause)
                    {
                        richTextBoxJoints.Document.Blocks.Clear();
                    }
                    labelSkeletonTrackedFrames.Content = "TrackedSkelletonFrames/s: " + framerateCounterTrackedSkeletonFrames.incrementFramerateCounter();
                    foreach (Joint joint in user.Joints)
                    {
                        DrawPoint(joint, Colors.Blue, now);
                    }
                }
            }
        }

        private void DrawPoint(Joint joint, Color color, DateTime now)
        {
            var scaledJoint = ScaleJoint(joint);
            if (!pause)
            {
                string info = "Time: " + now.ToLongTimeString() + DateTime.Now.Millisecond.ToString() + " JointID: " + joint.ID + " X: " + joint.Position.X + " Y: " + joint.Position.Y + " Z: " + joint.Position.Z + " W: " + joint.Position.W;
                info = info.Trim();
       
                richTextBoxJoints.AppendText(info);
                var para = new Paragraph();

                System.IO.File.AppendAllText("test.txt", info);
            }
            Ellipse ellipse = new Ellipse
            {
                Fill = new SolidColorBrush(color),
                Width = 25,
                Height = 25,
                Opacity = 0.5,
                Margin = new Thickness(scaledJoint.Position.X, scaledJoint.Position.Y, 0, 0)
            };

            canvas.Children.Add(ellipse);
        }

        private Joint ScaleJoint(Joint joint)
        {
            return new Joint()
            {
                ID = joint.ID,
                Position = new Microsoft.Research.Kinect.Nui.Vector
                {
                    X = ScalePosition(640, joint.Position.X),
                    Y = ScalePosition(480, -joint.Position.Y),
                    Z = joint.Position.Z,
                    W = joint.Position.W
                },
                TrackingState = joint.TrackingState
            };
        }

        private float ScalePosition(int size, float position)
        {
            float scaledPosition = (((size / 2) * position) + (size / 2));

            if (scaledPosition > size)
            {
                return size;
            }

            if (scaledPosition < 0)
            {
                return 0;
            }
            return scaledPosition;
        }
        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            if (pause)
            {
                buttonPause.Content = "Pause";
            }
            else
            {
                buttonPause.Content = "Resume";
            }
            pause = !pause;
        }
    }
}
