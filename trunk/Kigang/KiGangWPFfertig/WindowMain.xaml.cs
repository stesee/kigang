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
using System.Windows.Shapes;
using System.IO;
using ClassLibraryDatabase;
using Microsoft.Research.Kinect.Nui;

using KinectSDKDemo;
using System.Runtime.Serialization.Formatters.Binary;
using SVT;

namespace KiGangWPFfertig
{
    /// <summary>
    /// Interaktionslogik für WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        // Variablen
        bool pause = false;
        Runtime _nui;
        bool bAufnahme = false;
        ICollection<SpecialSkeletonData> recording = null;
        private const int USERINDEX = 0;

        ExerciseSupervisor exerciseSupervisor = null;
        bool bPlay = false;

        //store last framenr
        int startFrameNr;

        //treshold of relative error of angle
        //if any relative error of any joint compared to a reference movement is higher than this treshold, the corresponding joint
        //should be highlighted
        private double relErrorTreshold = 0.1;

        public WindowMain()
        {
            InitializeComponent();
        }
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void aufnahme_Click(object sender, RoutedEventArgs e)
        {
            bAufnahme = true;
            recording = new List<SpecialSkeletonData>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect_Initialize();
            db_Initialize();
            //Test ExerciseSupervisor
            ExerciseSupervisor superVisor = new ExerciseSupervisor();
            Microsoft.Research.Kinect.Nui.Vector[] refJointPos = new Microsoft.Research.Kinect.Nui.Vector[] 
                {new Microsoft.Research.Kinect.Nui.Vector() { W = 0.0f, X = 1.0f, Y = 0.0f, Z = 0.0f },
                new Microsoft.Research.Kinect.Nui.Vector(){ W=0.0f, X=0f, Y=0f, Z=0.0f },
                new Microsoft.Research.Kinect.Nui.Vector() { W = 0.0f, X = 0f, Y = 1f, Z = 0.0f }};

            Microsoft.Research.Kinect.Nui.Vector[] thisJointPos = new Microsoft.Research.Kinect.Nui.Vector[] 
                {new Microsoft.Research.Kinect.Nui.Vector() { W = 0.0f, X = 1f, Y = 0f, Z = 0.0f },
                new Microsoft.Research.Kinect.Nui.Vector(){ W=0.0f, X=0f, Y=0f, Z=0f },
                new Microsoft.Research.Kinect.Nui.Vector() { W = 0.0f, X = 0.5f, Y = 0.5f, Z = 0.0f }};
            double deltaPhi = superVisor.calcAngleError(thisJointPos, refJointPos);

            System.Console.WriteLine("deltaPhi: {0:F}", deltaPhi);
            System.Windows.Data.CollectionViewSource bewegungsaufzeichnungViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("bewegungsaufzeichnungViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // bewegungsaufzeichnungViewSource.Source = [generic data source]
            // Load data by setting the CollectionViewSource.Source property:
            using (KigangContext kigangKontext = new KigangContext())
            {
                kigangKontext.Therapien.FirstOrDefault().Trainingseinheiten.FirstOrDefault().Bewegungsaufzeichnung.ToList();
                bewegungsaufzeichnungViewSource.Source = kigangKontext.Therapien.FirstOrDefault().Trainingseinheiten.FirstOrDefault().Bewegungsaufzeichnung;
            }
        }

        private void db_Initialize()
        {
            System.Data.Entity.Database.SetInitializer<KigangContext>(new System.Data.Entity.DropCreateDatabaseIfModelChanges<KigangContext>());
        }
        private void kinect_Initialize()
        {
            try
            {
 
            _nui = new Runtime();

            _nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(Nui_VideoFrameReady);
            _nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Nui_SkeletonFrameReady);

                _nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
                _nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }

        FramerateCounter framerateCounterVideoFrames = new FramerateCounter();
        void Nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            var image = e.ImageFrame.Image;
            labelVideoFrames.Content = "Videoframes/s: " + framerateCounterVideoFrames.incrementFramerateCounter();

            if (((TabItem)tabControl1.SelectedItem).Header.Equals("Therapeut"))
                anzeige.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
            else if (((TabItem)tabControl1.SelectedItem).Header.Equals("Patient"))
                anzeigepatient.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
        }
        FramerateCounter framerateCounterTrackedSkeletonFrames = new FramerateCounter();
        FramerateCounter framerateCounterSkeletonFrames = new FramerateCounter();
        //void Nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        //{
        //    canvas.Children.Clear();
        //    labelSkeletonFrames.Content = "Skeletonframes/s: " + framerateCounterSkeletonFrames.incrementFramerateCounter();
        //    foreach (SkeletonData user in e.SkeletonFrame.Skeletons)
        //    {
        //        if (user.TrackingState == SkeletonTrackingState.Tracked)
        //        {
        //            if (!pause)
        //            {
        //                //richTextBoxJoints.Document.Blocks.Clear();
        //            }
        //            labelSkeletonTrackedFrames.Content = "TrackedSkelletonFrames/s: " + framerateCounterTrackedSkeletonFrames.incrementFramerateCounter();
        //            foreach (Joint joint in user.Joints)
        //            {
        //                DrawPoint(joint, Colors.Blue);
        //            }
        //        }
        //    }
        //}
        #region skeletonviewer
        Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };
        private Point getDisplayPosition(Joint joint, Canvas _canvas)
        {
            float depthX, depthY;
            _nui.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = depthX * 320; //convert to 320, 240 space
            depthY = depthY * 240; //convert to 320, 240 space
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            _nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)(_canvas.Width * colorX / 640.0), (int)(_canvas.Height * colorY / 480));
        }

        Polyline getBodySegment(Canvas _canvas, IDictionary<JointID, Joint> joints, Brush brush, params JointID[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i)
            {
                points.Add(getDisplayPosition(joints[ids[i]], _canvas));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        void Nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            int iSkeleton = 0;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            Canvas liveViewCanvas = getLiveViewCanvas();
            Canvas referenceCanvas = getReferenceCanvas();

            if (bPlay == false && bAufnahme == false)
            {
                startFrameNr = skeletonFrame.FrameNumber;
            }
            System.Console.WriteLine("skeletonFrame: {0}, resetFrame: {1}",skeletonFrame.FrameNumber,resetFrameNr(skeletonFrame.FrameNumber));

            //Now calculate relative errors of joint angles to reference movements (only in Patienten tab)
            IDictionary<JointID, double> relErrors = new Dictionary<JointID, double>();
            if (bPlay && exerciseSupervisor != null)
            {
                Bewegungsaufzeichnung refMovement = getReferenceMovement();
                relErrors = exerciseSupervisor.compareJoints(skeletonFrame,resetFrameNr(skeletonFrame.FrameNumber),refMovement);
            }

            liveViewCanvas.Children.Clear();
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {

                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    DrawSkeletonToCanvas(Conv.toDict(data.Joints), brush, liveViewCanvas, relErrors);
                }
                iSkeleton++;
            } // for each skeleton

            //Now draw reference if we are in patienten tab
            if (bPlay && referenceCanvas != null)
            {
                Bewegungsaufzeichnung refMovement = getReferenceMovement();

                if (refMovement != null)
                {
                    //Find corresponding framenr.
                    SpecialSkeletonData skeletonDataDB = refMovement.skeletonData.FirstOrDefault(o => o.FrameNr == resetFrameNr(e.SkeletonFrame.FrameNumber));
                    if (skeletonDataDB != null)
                    {
                        DrawSkeletonToCanvas(Conv.toDict(skeletonDataDB.kigangSkelleton), brushes[0], referenceCanvas, null);
                    }
                }
            }

            if (bAufnahme == true)
            {
                SkeletonData userKinect = e.SkeletonFrame.Skeletons.FirstOrDefault(o => o.TrackingState.Equals(SkeletonTrackingState.Tracked));
                if (userKinect != null)
                {

                    // Umkopieren der Framedaten (Kinect) in das DB Objekt
                    SpecialSkeletonData kinectData = new ClassLibraryDatabase.SpecialSkeletonData();

                    kinectData.Timestamp = e.SkeletonFrame.TimeStamp.ToString();
                    kinectData.FrameNr = resetFrameNr(e.SkeletonFrame.FrameNumber);
                    kinectData.kigangSkelleton = new List<SkeletonJointData>();
                    foreach (Joint joint in userKinect.Joints)
                    {
                        SkeletonJointData dbSkeletonData = new SkeletonJointData();

                        // ID
                        dbSkeletonData.jointId = (int)joint.ID;
                        // Position
                        dbSkeletonData.x = joint.Position.X;
                        dbSkeletonData.y = joint.Position.Y;
                        dbSkeletonData.z = joint.Position.Z;
                        dbSkeletonData.w = joint.Position.W;
                        // zur Liste hinzufügen...

                        kinectData.kigangSkelleton.Add(dbSkeletonData);
                    }
                    recording.Add(kinectData);
                }

            }

        }
        int resetFrameNr(int currentFrameNr)
        {
            return currentFrameNr - startFrameNr;
        }

        void DrawSkeletonToCanvas(IDictionary<JointID, Joint> joints, Brush brush, Canvas _canvas, IDictionary<JointID, double> relErrors)
        {

            _canvas.Children.Add(getBodySegment(_canvas, joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
            _canvas.Children.Add(getBodySegment(_canvas, joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
            _canvas.Children.Add(getBodySegment(_canvas, joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
            _canvas.Children.Add(getBodySegment(_canvas, joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
            _canvas.Children.Add(getBodySegment(_canvas, joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

            // Draw joints
            foreach (Joint joint in joints.Values)
            {
                Point jointPos = getDisplayPosition(joint, _canvas);
                Line jointLine = new Line();
                jointLine.X1 = jointPos.X - 3;
                jointLine.X2 = jointLine.X1 + 6;
                jointLine.Y1 = jointLine.Y2 = jointPos.Y;

                //Now check if this joint's angle has a relatve error > than the treshold
                if (relErrors != null && relErrors.ContainsKey(joint.ID) && relErrors[joint.ID] > relErrorTreshold)
                {
                    jointLine.Stroke = new SolidColorBrush(Colors.Red);
                    jointLine.StrokeThickness = 12;
                }
                else
                {
                    jointLine.Stroke = jointColors[joint.ID];
                    jointLine.StrokeThickness = 6;
                }
                _canvas.Children.Add(jointLine);
            }
        }

        Canvas getLiveViewCanvas()
        {
            if (((TabItem)tabControl1.SelectedItem).Header.Equals("Therapeut"))
                return canvas;

            return patientenCanvas;
        }

        Canvas getReferenceCanvas()
        {
            //For Therapeut tab there is no canvas that shows reference movement
            if (((TabItem)tabControl1.SelectedItem).Header.Equals("Therapeut"))
                return null;

            return canvas1;
        }

        Bewegungsaufzeichnung getReferenceMovement()
        {
            KigangContext kigangKontext = new KigangContext();
            System.Windows.Data.CollectionViewSource bewegungsaufzeichnungViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("bewegungsaufzeichnungViewSource")));
            string currentSelection = ((Bewegungsaufzeichnung)bewegungsaufzeichnungViewSource.View.CurrentItem).Zeit;
            Bewegungsaufzeichnung recording = kigangKontext.Therapien.First().Trainingseinheiten.First().Bewegungsaufzeichnung.FirstOrDefault(o=>o.Zeit.Equals(currentSelection));
            return recording;
        }

        void nui_ColorFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // 32-bit per pixel, RGBA image
            PlanarImage Image = e.ImageFrame.Image;
            anzeige.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
        }

        #endregion
        private void DrawPoint(Joint joint, Color color)
        {
            var scaledJoint = ScaleJoint(joint);
            Canvas liveViewCanvas = getLiveViewCanvas();
            Canvas referenceCanvas = getReferenceCanvas();

            if (!pause)
            {
                string info = "Time: " + DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond.ToString() + " JointID: " + joint.ID + " X: " + joint.Position.X + " Y: " + joint.Position.Y + " Z: " + joint.Position.Z + " W: " + joint.Position.W;
                info = info.Trim();

                // richTextBoxJoints.AppendText(info);
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

            liveViewCanvas.Children.Add(ellipse);
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
        #region ImportExport
        #region exporthelpers
        /// <summary> 
        /// Creates a semicolon delimeted string of all the objects property values. 
        /// </summary> 
        /// <param name="obj">object.</param> 
        /// <returns>string.</returns> 
        public static string ObjectNameToCsvData(object obj)
        {
            if (obj == null)
            {
                //  throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            System.Reflection.PropertyInfo[] pi = t.GetProperties();
            for (int index = 0; index < pi.Length; index++)
            {
                sb.Append(pi[index].Name);
                if (index < pi.Length - 1)
                {
                    sb.Append(";");
                }
            }
            return sb.ToString();
        }
        /// <summary> 
        /// Creates a semicolon delimeted string of all the objects values. 
        /// </summary> 
        /// <param name="obj">object.</param> 
        /// <returns>string.</returns> 
        public static string ObjectToCsvData(object obj)
        {
            StringBuilder sb = new StringBuilder();
            if (obj == null)
            {
                sb.Append("Keine Einträge");
            }
            else
            {
                Type t = obj.GetType();
                System.Reflection.PropertyInfo[] pi = t.GetProperties();

                for (int index = 0; index < pi.Length; index++)
                {
                    sb.Append(pi[index].GetValue(obj, null));

                    if (index < pi.Length - 1)
                    {
                        sb.Append(";");
                    }
                }
            }
            return sb.ToString();
        }
        #endregion
        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Kigang"; // Default file name
            dlg.DefaultExt = ".kigang"; // Default file extension
            dlg.Filter = "Text documents (.kigang)|*.kigang"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                string strValue = string.Empty;
                // http://stackoverflow.com/questions/5041172/serialize-entity-framework-object-save-to-file-read-and-deserialize
                if (File.Exists(dlg.FileName))
                {
                    File.Delete(dlg.FileName);
                }
                List<ImportExportData> export = new List<ImportExportData>();
                using (KigangContext kigangKontext = new KigangContext())
                {
                    var export1 = new ImportExportData();
                    foreach (var item in kigangKontext.Therapien.First().Trainingseinheiten.First().Bewegungsaufzeichnung.AsQueryable())
                    {
                        var skeletondata = new ImportExportData.SpecialSkeletonDataImportExport();
                        export1.skeletonData = new List<ImportExportData.SpecialSkeletonDataImportExport>();
                        foreach (var item1 in item.skeletonData.ToList())
                        {
                            skeletondata.kigangSkelleton = new List<ImportExportData.SkeletonJointDataImportExport>();
                            skeletondata.FrameNr = item1.FrameNr;
                            skeletondata.Timestamp = item1.Timestamp;
                            if (item1.kigangSkelleton != null)
                            {
                                foreach (var item2 in item1.kigangSkelleton)
                                {

                                    var kigangSkelleton = new ImportExportData.SkeletonJointDataImportExport();
                                    kigangSkelleton.jointId = (JointID)item2.jointId;
                                    kigangSkelleton.w = item2.w;
                                    kigangSkelleton.x = item2.x;
                                    kigangSkelleton.y = item2.y;
                                    kigangSkelleton.z = item2.z;
                                    skeletondata.kigangSkelleton.Add(kigangSkelleton);
                                }
                            }
                            export1.skeletonData.Add(skeletondata);
                        }
                        export1.Zeit = item.Zeit;
                        export.Add(export1);
                    }
                }
                using (var file = File.Create(dlg.FileName))
                {
                    BinaryFormatter ser = new BinaryFormatter();
                    ser.Serialize(file, export);
                    file.Close();
                }
            }
        }
        private void buttonInit_Click(object sender, RoutedEventArgs e)
        {
            System.Data.Entity.Database.SetInitializer<KigangContext>(new System.Data.Entity.DropCreateDatabaseAlways<KigangContext>());
            try
            {
                using (KigangContext kigangKontext = new KigangContext())
                {
                    //Therapeut
                    kigangKontext.Therapeuten.Add(new Therapeut { Email = "kigang@technikum-wien.at", Vorname = "Stefan", Nachname = "Therapeut", Telefon = "+4367686422648", Kommentar = "Wien", Plz = 1010, Strasse = "Stefansplatz 1", Ort = "Wien", SvNr = 1234140299, Type = "Typ1" });
                    kigangKontext.SaveChanges();
                    richTextBoxLog.AppendText("Therapeuten initialisiert");
                    //Patient
                    kigangKontext.Patients.Add(new Patient { Kommentar = "TestUser", LoginName = "TestUser" });
                    kigangKontext.SaveChanges();
                    richTextBoxLog.AppendText("Patienten initiealisiert");
                    //Therapie
                    var thearpie = new Therapie { Beginn = DateTime.Now.ToString(), };
                    kigangKontext.Therapien.Add(thearpie);
                    kigangKontext.SaveChanges();
                    //Übung
                    var uebung = new Uebung();
                    uebung.UebungsName = "test";
                    //Übungsgeräte
                    var uebungsgraete = new List<Uebungsgeraete>();
                    uebungsgraete.Add(new Uebungsgeraete());
                    //Übungsaufgabe
                    var uebungsaufgabe = new Ueabungsaufgabe { AnzWiederhlg = 2, Uebung = uebung, Uebungsgeraete = uebungsgraete };
                    var uebungsaufgaben = new List<Ueabungsaufgabe>();
                    uebungsaufgaben.Add(uebungsaufgabe);
                    //Übungsplan
                    var uebungsplan = new Uebungsplan { Ueabungsaufgabe = uebungsaufgaben, ZuUebenBis = DateTime.Now.ToString() };
                    //Bewegungsaufzeichnung

                    var bewegungsaufzeichnung = new Bewegungsaufzeichnung { Zeit = DateTime.Now.ToString() };

                    SkeletonJointData skeletonData = new SkeletonJointData();

                    skeletonData.x = 123;
                    // und andere Properties übersetzen... vermutlich in einem Foreach so wie im kinecdsdkdemo
                    SpecialSkeletonData specialSkelletonData = new ClassLibraryDatabase.SpecialSkeletonData();
                    specialSkelletonData.kigangSkelleton = new List<SkeletonJointData>();
                    specialSkelletonData.kigangSkelleton.Add(skeletonData);
                    bewegungsaufzeichnung.skeletonData = new List<SpecialSkeletonData>();
                    bewegungsaufzeichnung.skeletonData.Add(specialSkelletonData);
                    bewegungsaufzeichnung.Ueabungsaufgabe = uebungsaufgabe;
                    thearpie.Trainingseinheiten = new List<Trainingseinheit>();
                    thearpie.Trainingseinheiten.Add(new Trainingseinheit { Timestamp = DateTime.Now.ToString() });
                    kigangKontext.SaveChanges();
                    thearpie.Trainingseinheiten = new List<Trainingseinheit>();
                    thearpie.Trainingseinheiten.Add(new Trainingseinheit());
                    var trainingseinheit = thearpie.Trainingseinheiten.First();
                    trainingseinheit.Bewegungsaufzeichnung = new List<Bewegungsaufzeichnung>();
                    trainingseinheit.Bewegungsaufzeichnung.Add(bewegungsaufzeichnung);
                    kigangKontext.SaveChanges();

                }
            }
            catch (Exception ex)
            {

                richTextBoxLog.AppendText(ex.Message);
            }
        }
        private void buttonImport_Click(object sender, RoutedEventArgs e)
        {
            List<ImportExportData> import = new List<ImportExportData>();
            // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Kigang"; // Default file name
            dlg.DefaultExt = ".kigang"; // Default file extension
            dlg.Filter = "Text documents (.kigang)|*.kigang"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                using (KigangContext kigangKontext = new KigangContext())
                {
                    //    kigangKontext.Configuration.ProxyCreationEnabled = false;
                    using (var file = File.OpenRead(dlg.FileName))
                    {
                        BinaryFormatter ser = new BinaryFormatter();
                        import = (List<ImportExportData>)ser.Deserialize(file);
                    }
                    try
                    {
                        foreach (var item in import)
                        {
                            var bewegungsaufzeichnung = new Bewegungsaufzeichnung { Zeit = item.Zeit };
                            bewegungsaufzeichnung.skeletonData = new List<SpecialSkeletonData>();
                            //   bewegungsaufzeichnung.Ueabungsaufgabe = null;
                            foreach (var item1 in item.skeletonData)
                            {
                                SpecialSkeletonData bla = new SpecialSkeletonData();
                                bla.FrameNr = item1.FrameNr;

                                bla.kigangSkelleton = new List<SkeletonJointData>();
                                foreach (var item2 in item1.kigangSkelleton)
                                {
                                    var bla2 = new SkeletonJointData();
                                    bla2.jointId = (int)item2.jointId;
                                    bla2.w = item2.w;
                                    bla2.x = item2.x;
                                    bla2.y = item2.y;
                                    bla2.z = item2.z;
                                    bla.kigangSkelleton.Add(bla2);
                                }
                                bla.Timestamp = item1.Timestamp;
                                bewegungsaufzeichnung.skeletonData.Add(bla);
                            }
                            kigangKontext.Therapien.First().Trainingseinheiten.First().Bewegungsaufzeichnung.Add(bewegungsaufzeichnung);
                            kigangKontext.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        richTextBoxLog.AppendText(ex.Message);
                    }


                }
            }
        }
        #endregion
        private void speichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (KigangContext kigangKontext = new KigangContext())
                {
                    var bewegungsaufzeichnung = new Bewegungsaufzeichnung { Zeit = DateTime.Now.ToString() };
                    bewegungsaufzeichnung.Ueabungsaufgabe = null;
                    bewegungsaufzeichnung.skeletonData = recording;
                    var uebung = new Uebung();
                    //temporärer Übungsname: kann später vom User geändert werden.
                    uebung.UebungsName = bewegungsaufzeichnung.Zeit;
                    uebung.Bewegungsaufzeichnung = bewegungsaufzeichnung;

                    kigangKontext.Therapien.First().Trainingseinheiten.First().Bewegungsaufzeichnung.Add(bewegungsaufzeichnung);
                    kigangKontext.SaveChanges();
                    recording.Clear();
                    recording = null;
                }
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendText(ex.Message);
            }

        }
        private void stop_Click(object sender, RoutedEventArgs e)
        {
            bAufnahme = false;
            bPlay = false;
            exerciseSupervisor = null;
        }

        private void p_wiedergabe_Click(object sender, RoutedEventArgs e)
        {
            bPlay = true;
            exerciseSupervisor = new ExerciseSupervisor();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void bewegungsaufzeichnungDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            label1.Content = "test";
        }
    }
}
