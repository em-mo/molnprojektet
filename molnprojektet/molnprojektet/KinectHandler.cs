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

        private const float SWIPE_INITIALIZE_VALUE = 0F;
        private const float SWIPE_DELTA_QOTIENT = 1.7F;
        private const float SWIPE_THRESHOLD = 0.25F;
        
        //Right hand DOWN
        private float right_DownDeltaBuffer;
        private float right_DownPreviousPosition;
        //Right hand UP
        private float right_UpDeltaBuffer;
        private float right_UpPreviousPosition;
        //RightHand LEFT
        private float right_LeftDeltaBuffer;
        private float right_LeftPreviousPosition;
        //RightHand RIGHT
        private float right_RightDeltaBuffer;
        private float right_RightPreviousPosition;


        //Left hand DOWN
        private float left_DownDeltaBuffer;
        private float left_DownPreviousPosition;
        //Left hand UP
        private float left_UpDeltaBuffer;
        private float left_UpPreviousPosition;
        //LeftHand LEFT
        private float left_LeftDeltaBuffer;
        private float left_LeftPreviousPosition;
        //LeftHand left
        private float left_RightDeltaBuffer;
        private float left_RightPreviousPosition;

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
                System.Threading.Thread.Sleep(10);
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

            if (currentSkeleton != null && currentSkeleton.TrackingState != SkeletonTrackingState.NotTracked)
            {

                handleArmAngles();

                if (CheckForRightHandSwipeUp(currentSkeleton.Joints[JointType.HandRight]))
                    game.SwipeUp(Arm.Right);
                if (CheckForRightHandSwipeDown(currentSkeleton.Joints[JointType.HandRight]))
                    game.SwipeDown(Arm.Right);
                if (CheckForRightHandSwipeToLeft(currentSkeleton.Joints[JointType.HandRight]))
                    game.SwipeLeft(Arm.Right);
                if (CheckForRightHandSwipeToRight(currentSkeleton.Joints[JointType.HandRight]))
                    game.SwipeRight(Arm.Right);


                if (CheckForLeftHandSwipeUp(currentSkeleton.Joints[JointType.HandLeft]))
                    game.SwipeUp(Arm.Left);
                if (CheckForLeftHandSwipeDown(currentSkeleton.Joints[JointType.HandLeft]))
                    game.SwipeDown(Arm.Left);
                if (CheckForLeftHandSwipeToLeft(currentSkeleton.Joints[JointType.HandLeft]))
                    game.SwipeLeft(Arm.Left);
                if (CheckForLeftHandSwipeToRight(currentSkeleton.Joints[JointType.HandLeft]))
                    game.SwipeRight(Arm.Left);
            }
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

        public void handleArmAngles()
        {
            float leftHumerusAngle = calculateArmAngle(JointType.ShoulderLeft, JointType.ElbowLeft, Arm.Left);
            float leftUlnaAngle = calculateArmAngle(JointType.ElbowLeft, JointType.WristLeft, Arm.Left);

            float rightHumerusAngle = calculateArmAngle(JointType.ShoulderRight, JointType.ElbowRight, Arm.Right);
            float rightUlnaAngle = calculateArmAngle(JointType.ElbowRight, JointType.WristRight, Arm.Right);

            game.PlayerCloud.SetLeftArmRotation(leftHumerusAngle, leftUlnaAngle);
            game.PlayerCloud.SetRightArmRotation(rightHumerusAngle, rightUlnaAngle);
        }
        
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

        #region Righthand Swipe checks
        private DateTime rightHandcoolDown = DateTime.Now;
        public bool CheckForRightHandSwipeToLeft(Joint trackedJoint)
        {
            right_LeftDeltaBuffer /= SWIPE_DELTA_QOTIENT; 
            
            right_LeftDeltaBuffer += right_LeftPreviousPosition - trackedJoint.Position.X;

            right_LeftPreviousPosition = trackedJoint.Position.X;

            if (right_LeftDeltaBuffer > SWIPE_THRESHOLD && rightHandcoolDown < DateTime.Now)
            {
                right_LeftDeltaBuffer = 0F;
                rightHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }

        public bool CheckForRightHandSwipeToRight(Joint trackedJoint)
        {
            right_RightDeltaBuffer /= SWIPE_DELTA_QOTIENT;

            right_RightDeltaBuffer += trackedJoint.Position.X - right_RightPreviousPosition;

            right_RightPreviousPosition = trackedJoint.Position.X;

            if (right_RightDeltaBuffer > SWIPE_THRESHOLD && rightHandcoolDown < DateTime.Now)
            {
                right_RightDeltaBuffer = 0F;
                rightHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }

        public bool CheckForRightHandSwipeDown(Joint trackedJoint)
        {
            right_DownDeltaBuffer /= SWIPE_DELTA_QOTIENT;

            right_DownDeltaBuffer += right_DownPreviousPosition - trackedJoint.Position.Y;

            right_DownPreviousPosition = trackedJoint.Position.Y;
            
            if (right_DownDeltaBuffer > SWIPE_THRESHOLD && rightHandcoolDown < DateTime.Now)
            {
                right_DownDeltaBuffer = 0F;
                rightHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }

        public bool CheckForRightHandSwipeUp(Joint trackedJoint)
        {
            right_UpDeltaBuffer /= SWIPE_DELTA_QOTIENT;
            
            right_UpDeltaBuffer += trackedJoint.Position.Y - right_UpPreviousPosition;

            right_UpPreviousPosition = trackedJoint.Position.Y;
            
            if (right_UpDeltaBuffer > SWIPE_THRESHOLD && rightHandcoolDown < DateTime.Now)
            {
                right_UpDeltaBuffer = 0F;
                rightHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }
        #endregion



        #region Lefthand Swipe checks

        private DateTime leftHandcoolDown = DateTime.Now;
        public bool CheckForLeftHandSwipeToLeft(Joint trackedJoint)
        {
            left_LeftDeltaBuffer /= SWIPE_DELTA_QOTIENT;

            left_LeftDeltaBuffer += left_LeftPreviousPosition - trackedJoint.Position.X;

            left_LeftPreviousPosition = trackedJoint.Position.X;

            if (left_LeftDeltaBuffer > SWIPE_THRESHOLD && leftHandcoolDown < DateTime.Now)
            {
                left_LeftDeltaBuffer = 0F;
                rightHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }

        public bool CheckForLeftHandSwipeToRight(Joint trackedJoint)
        {
            left_RightDeltaBuffer /= SWIPE_DELTA_QOTIENT;

            left_RightDeltaBuffer += trackedJoint.Position.X - left_RightPreviousPosition;

            left_RightPreviousPosition = trackedJoint.Position.X;

            if (left_RightDeltaBuffer > SWIPE_THRESHOLD && leftHandcoolDown < DateTime.Now)
            {
                left_RightDeltaBuffer = 0F;
                leftHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }

        public bool CheckForLeftHandSwipeDown(Joint trackedJoint)
        {
            left_DownDeltaBuffer /= SWIPE_DELTA_QOTIENT;

            left_DownDeltaBuffer += left_DownPreviousPosition - trackedJoint.Position.Y;

            left_DownPreviousPosition = trackedJoint.Position.Y;

            if (left_DownDeltaBuffer > SWIPE_THRESHOLD && leftHandcoolDown < DateTime.Now)
            {
                left_DownDeltaBuffer = 0F;
                leftHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }

        public bool CheckForLeftHandSwipeUp(Joint trackedJoint)
        {
            left_UpDeltaBuffer /= SWIPE_DELTA_QOTIENT;

            left_UpDeltaBuffer += trackedJoint.Position.Y - left_UpPreviousPosition;

            left_UpPreviousPosition = trackedJoint.Position.Y;

            if (left_UpDeltaBuffer > SWIPE_THRESHOLD && leftHandcoolDown < DateTime.Now)
            {
                left_UpDeltaBuffer = 0F;
                leftHandcoolDown = DateTime.Now.AddSeconds(0.4);
                return true;
            }
            else
                return false;
        }
        #endregion


    }
}
