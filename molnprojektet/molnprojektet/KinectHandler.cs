using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
    class KinectHandler
    {
        KinectSensor sensor = null;
        GameWindow game;
        bool running = true;
        Skeleton currentSkeleton;
        public KinectHandler(GameWindow owner)
        {
            game = owner;

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
        }

        public void run()
        {
            while (running)
            {
                if (sensor != null)
                    ProcessSkeletonFrame();
            }
        }

        private void ProcessSkeletonFrame()
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = sensor.SkeletonStream.OpenNextFrame(100))
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    skeletonFrame.Dispose();
                }
            }

            TrackClosestSkeleton(skeletons);

            handleArmAngles();

        }

        private void TrackClosestSkeleton(Skeleton[] skeletons)
        {
            int closestID = 0;

            if (this.sensor != null && this.sensor.SkeletonStream != null)
            {
                if (!this.sensor.SkeletonStream.AppChoosesSkeletons)
                {
                    this.sensor.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
                }

                float closestDistance = 10000f; // Start with a far enough distance
                Skeleton closestSkeleton = null;

                foreach (Skeleton skeleton in skeletons.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                {
                    if (skeleton.Position.Z < closestDistance)
                    {
                        closestID = skeleton.TrackingId;
                        closestDistance = skeleton.Position.Z;
                        closestSkeleton = skeleton;
                    }
                }

                if (closestID > 0)
                {
                    this.currentSkeleton = closestSkeleton;
                    this.sensor.SkeletonStream.ChooseSkeletons(closestID); // Track this skeleton
                }
            }
        }

        private void handleArmAngles()
        {
            float leftHumerusAngle = calculateArmAngle(JointType.ShoulderLeft, JointType.ElbowLeft, Arm.Left);
            float leftUlnaAngle = calculateArmAngle(JointType.ElbowLeft, JointType.WristLeft, Arm.Left);

            float rightHumerusAngle = calculateArmAngle(JointType.ShoulderRight, JointType.ElbowRight, Arm.Right);
            float rightUlnaAngle = calculateArmAngle(JointType.ElbowRight, JointType.WristRight, Arm.Right);

            game.PlayerCloud.setLeftArmRotation(leftHumerusAngle, leftUlnaAngle);
            game.PlayerCloud.setRightArmRotation(rightHumerusAngle, rightUlnaAngle);
        }
        
        private enum Arm {Left, Right};

        private float calculateArmAngle(JointType startJoint1, JointType endJoint1,
                             Arm arm)
        {
            Joint joint1 = currentSkeleton.Joints[startJoint1];
            Joint joint2 = currentSkeleton.Joints[endJoint1];

            Vector3 vector1 = new Vector3(joint2.Position.X - joint1.Position.X, joint2.Position.Y - joint1.Position.Y, joint2.Position.Z - joint1.Position.Z);
            Vector3 vector2;
            if (arm == Arm.Left)
                vector2 = new Vector3(-1, 0, joint2.Position.Z - joint1.Position.Z);
            else
                vector2 = new Vector3(1, 0, joint2.Position.Z - joint1.Position.Z);

            return (float)Utils.CalculateAngle(vector1, vector2);
        }
    }
}
