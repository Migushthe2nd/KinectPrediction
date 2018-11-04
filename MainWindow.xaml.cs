/////////////////////////////////////////////////////////////////////////
//
// This module contains code to do Kinect NUI initialization and
// processing and also to display NUI streams on screen.
//
// Copyright © Microsoft Corporation.  All rights reserved.  
// This code is licensed under the terms of the 
// Microsoft Kinect for Windows SDK (Beta) from Microsoft Research 
// License Agreement: http://research.microsoft.com/KinectSDK-ToU
//
/////////////////////////////////////////////////////////////////////////
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
using Microsoft.Kinect;
using RedOwlConsulting.JointPrediction;

namespace SkeletalViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        KinectSensor sensor;
        int totalFrames = 0;
        int lastFrames = 0;
        DateTime lastTime = DateTime.MaxValue;

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];
        
        
        Dictionary<JointType,Brush> jointColors = new Dictionary<JointType,Brush>() { 
            {JointType.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointType.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointType.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointType.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointType.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointType.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointType.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointType.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointType.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointType.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointType.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointType.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointType.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointType.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointType.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointType.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointType.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointType.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointType.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointType.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };

        Dictionary<Tuple<int, JointType>, JointPredictor> jointPredictor = new Dictionary<Tuple<int, JointType>, JointPredictor>();

        private void Window_Loaded(object sender, EventArgs e)
        {
            try
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.Start();
                //sensor.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                return;
            }


            try
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.Start();
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);  //Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Failed to open stream. Please make sure to specify a supported image type and resolution.");
                return;
            }

            lastTime = DateTime.Now;

            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady); //sensor.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
        }

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        byte[] convertDepthFrame(byte[] depthFrame16)
        {
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = depthFrame16[i16] & 0x07;
                int realDepth = (depthFrame16[i16+1] << 5) | (depthFrame16[i16] >> 3);
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RED_IDX] = 0;
                depthFrame32[i32 + GREEN_IDX] = 0;
                depthFrame32[i32 + BLUE_IDX] = 0;

                // choose different display colors based on player
                switch (player)
                {
                    case 0:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 2);
                        break;
                    case 1:
                        depthFrame32[i32 + RED_IDX] = intensity;
                        break;
                    case 2:
                        depthFrame32[i32 + GREEN_IDX] = intensity;
                        break;
                    case 3:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 4:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 4);
                        break;
                    case 5:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 6:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 7:
                        depthFrame32[i32 + RED_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(255 - intensity);
                        break;
                }
            }
            return depthFrame32;
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            var Bits = new byte[sensor.DepthStream.FramePixelDataLength];
            using (var depthFrame = e.OpenDepthImageFrame())
                depth.Source = BitmapSource.Create(
                depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, Bits, depthFrame.Width * 4);

            ++totalFrames;
            
            DateTime cur = DateTime.Now;
            if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
                frameRate.Text = frameDiff.ToString() + " fps";
            }
        }

        //private point getdisplayposition(depthimageframe depthframe, joint joint)
        //{
        //    float depthx, depthy;
        //    depthimageformat depthimageformat = sensor.depthstream.format;
        //    depthimagepoint depthpoint = sensor.coordinatemapper.mapskeletonpointtodepthpoint(joint.position, depthimageformat); //.mapskeletonpointtodepth(joint.position, depthimageformat);

        //    depthx = depthpoint.x;
        //    depthy = depthpoint.y;

        //    depthx = math.max(0, math.min(depthx * 320, 320));
        //    depthy = math.max(0, math.min(depthy * 240, 240));

        //    int colorx, colory;
        //    colorimageformat colorimageformat = sensor.colorstream.format;
        //    colorimagepoint colorpoint = //sensor.coordinatemapper.mapdepthpointtocolorpoint(depthpoint.x, depthpoint.y, colorimageformat); 
        //                                 depthframe.maptocolorimagepoint(depthpoint.x, depthpoint.y, sensor.colorstream.format);
        //    colorx = colorpoint.x;
        //    colory = colorpoint.y;

        //    return new point((int)(skeleton.width * colorx / 640.0), (int)(skeleton.height * colory / 480));
        //}

        private Point getDisplayPosition(Joint joint)
        {
            return new Point((int)(joint.Position.X), (int)(joint.Position.Y));
        }

        Polyline getBodySegment(JointCollection joints, Brush brush, params JointType[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; i++)
            {
                points.Add(getDisplayPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }
        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame frame = e.OpenSkeletonFrame();
            int iSkeleton = 0;
            Skeleton[] skeletondata = new Skeleton[5];
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            skeletoncanvas.Children.Clear();
            foreach (Skeleton data in skeletondata)
            {

                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    skeletoncanvas.Children.Add(getBodySegment(data.Joints, brush, JointType.HipCenter, JointType.Spine, JointType.ShoulderCenter, JointType.Head));
                    skeletoncanvas.Children.Add(getBodySegment(data.Joints, brush, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft));
                    skeletoncanvas.Children.Add(getBodySegment(data.Joints, brush, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight));
                    skeletoncanvas.Children.Add(getBodySegment(data.Joints, brush, JointType.HipCenter, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft));
                    skeletoncanvas.Children.Add(getBodySegment(data.Joints, brush, JointType.HipCenter, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight));

                    // Draw joints
                    foreach (Joint joint in data.Joints)
                    {
                        Tuple<int, JointType> index = new Tuple<int, JointType>(data.TrackingId, joint.JointType);
                        Point jointPos = getDisplayPosition(joint);
                        Line jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = jointColors[joint.JointType];
                        jointLine.StrokeThickness = 6;
                        skeletoncanvas.Children.Add(jointLine);

                        // If the jointPredictor entry is not yet initialized, do it
                        if (!jointPredictor.Keys.Contains(index))
                        {
                            jointPredictor.Add(index, new JointPredictor(20F));
                        }

                        // Ingest the new observation using the current position of the joint
                        Observation obs = new Observation
                        {
                            X = data.Joints[joint.JointType].Position.X,
                            Y = data.Joints[joint.JointType].Position.Y,
                            Z = data.Joints[joint.JointType].Position.Z,
                            DateTime = DateTime.Now
                        };
                        jointPredictor[index].Update(obs);

                        // Create a line for the predicted direction of motion
                        Line direction = new Line();
                        Point displayPosition = getDisplayPosition(data.Joints[joint.JointType]);

                        direction.Stroke = jointColors[joint.JointType];
                        direction.StrokeThickness = 10;

                        // These points correspond to the X Y planar projection of the velocity.
                        try
                        {
                            direction.X1 = displayPosition.X;
                            direction.Y1 = displayPosition.Y;
                            direction.X2 = direction.X1 + jointPredictor[index].EwmaVelocity.X * 25;
                            direction.Y2 = direction.Y1 - jointPredictor[index].EwmaVelocity.Y * 25;
                        }
                        catch (Exception)
                        {
                            //jointPredictor[index] = new JointPredictor(50F);
                        }

                        // Add the line to the GUI
                        skeletoncanvas.Children.Add(direction);
                    }
                }

                iSkeleton++;
            } // for each skeleton
        }


        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            var Bits = new byte[sensor.ColorStream.FramePixelDataLength];
            // 32-bit per pixel, RGBA image
            using (var colorFrame = e.OpenColorImageFrame())
                video.Source = BitmapSource.Create(
                colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, Bits, colorFrame.Width * colorFrame.BytesPerPixel);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            sensor.Stop();
            Environment.Exit(0);
        }

    }
}
