using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FreePIE.Core.Contracts;
using Leap;
using LeapInternal;


namespace UltraLeapPlugin
{
    public class Extremity
    {
        public bool visible { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float dx { get; set; }
        public float dy { get; set; }
        public float dz { get; set; }
        public float vx { get; set; }
        public float vy { get; set; }
        public float vz { get; set; }
        public float pinch { get; set; }
        public float strength { get; set; }
        public float time { get; set; }
        public float yaw { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }


        //public Digit thumb { get; set; } = new Digit();

        //public Digit index { get; set; } = new Digit();

        public Digit[] fingers { get; set; }

        public Extremity()
        {
            fingers = new Digit[5];
            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i] = new Digit(); // Instantiate each Digit object
            }
        }

    }

    public class Digit
    {
        public int id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float dx { get; set; }
        public float dy { get; set; }
        public float dz { get; set; }
        public bool extended { get; set; }
    }

    [GlobalType(Type = typeof(UltraLeapPluginGlobal))]
    public class UltraLeapPlugin : IPlugin
    {
        public object CreateGlobal()
        {
            return new UltraLeapPluginGlobal(this);
        }
        private LeapMotion lmc { get; set; } = null;

        private bool bDisconnected;

        public int handsCount;

        public Extremity lHand { get; set; } = new Extremity();

        public Extremity rHand { get; set; } = new Extremity();

        public Action Start()
        {
            lmc = new LeapMotion();
            
            
            if (lmc != null)
            {
                // Ask for frames even in the background - this is important!
                lmc.SetPolicy(LeapMotion.PolicyFlag.POLICY_BACKGROUND_FRAMES);
                lmc.SetPolicy(LeapMotion.PolicyFlag.POLICY_ALLOW_PAUSE_RESUME);

                lmc.ClearPolicy(LeapMotion.PolicyFlag.POLICY_OPTIMIZE_HMD); // NOT head mounted
                lmc.ClearPolicy(LeapMotion.PolicyFlag.POLICY_IMAGES);       // NO images, please

                // Subscribe to connected/not messages
                lmc.Connect += Lmc_Connect;
                lmc.Disconnect += Lmc_Disconnect;
                lmc.FrameReady += Lmc_FrameReady;
           

                if (lmc.IsConnected)
                {
                    bDisconnected = false;
                }
                else
                {
                    lmc.StartConnection();
                }
            }
            return null;
        }

        private void Lmc_Disconnect(object sender, ConnectionLostEventArgs e)
        {
            bDisconnected = true;
        }

        private void Lmc_Connect(object sender, ConnectionEventArgs e)
        {
            // Say we are NO LONGER DISCONNECTED woot :)  Double negatives, anyone?
            bDisconnected = false;
        }

        private void Lmc_FrameReady(object sender, FrameEventArgs e)
        {
            if (bDisconnected == true)
            {
                bDisconnected = false;
            }

            lHand.visible = false;
            rHand.visible = false;

            handsCount = e.frame.Hands.Count;

            if (e.frame.Hands.Count > 0)
            {
                for (int i = 0; i < e.frame.Hands.Count; i++)
                {
                    if (e.frame.Hands[i].IsLeft)
                    {
                        lHand.visible = true;
                        lHand.vx = e.frame.Hands[i].PalmVelocity.x;
                        lHand.vy = e.frame.Hands[i].PalmVelocity.y;
                        lHand.vz = e.frame.Hands[i].PalmVelocity.z;
                        lHand.x = e.frame.Hands[i].PalmPosition.x;
                        lHand.y = e.frame.Hands[i].PalmPosition.y;
                        lHand.z = e.frame.Hands[i].PalmPosition.z;
                        lHand.dx = e.frame.Hands[i].Direction.x;
                        lHand.dy = e.frame.Hands[i].Direction.y;
                        lHand.dz = e.frame.Hands[i].Direction.z;
                        lHand.pinch = e.frame.Hands[i].PinchDistance;
                        lHand.roll = e.frame.Hands[i].Direction.Roll;
                        lHand.pitch = e.frame.Hands[i].Direction.Pitch;
                        lHand.yaw = e.frame.Hands[i].Direction.Yaw;
                        lHand.pinch = e.frame.Hands[i].PinchDistance;
                        lHand.strength = e.frame.Hands[i].PinchStrength;
                        lHand.time = e.frame.Hands[i].TimeVisible;

                        for (int j = 0; j < e.frame.Hands[i].Fingers.Count; j++)
                        {
                            lHand.fingers[j].x = e.frame.Hands[i].Fingers[j].TipPosition.x;
                            lHand.fingers[j].y = e.frame.Hands[i].Fingers[j].TipPosition.y;
                            lHand.fingers[j].z = e.frame.Hands[i].Fingers[j].TipPosition.z;
                            lHand.fingers[j].dx = e.frame.Hands[i].Fingers[j].Direction.x;
                            lHand.fingers[j].dy = e.frame.Hands[i].Fingers[j].Direction.y;
                            lHand.fingers[j].dz = e.frame.Hands[i].Fingers[j].Direction.z;
                            lHand.fingers[j].extended = e.frame.Hands[i].Fingers[j].IsExtended;
                        }


                    }
                    else
                    {
                        rHand.visible = true;
                        rHand.vx = e.frame.Hands[i].PalmVelocity.x;
                        rHand.vy = e.frame.Hands[i].PalmVelocity.y;
                        rHand.vz = e.frame.Hands[i].PalmVelocity.z;
                        rHand.x = e.frame.Hands[i].PalmPosition.x;
                        rHand.y = e.frame.Hands[i].PalmPosition.y;
                        rHand.z = e.frame.Hands[i].PalmPosition.z;
                        rHand.dx = e.frame.Hands[i].Direction.x;
                        rHand.dy = e.frame.Hands[i].Direction.y;
                        rHand.dz = e.frame.Hands[i].Direction.z;
                        rHand.pinch = e.frame.Hands[i].PinchDistance;
                        rHand.roll = e.frame.Hands[i].Direction.Roll;
                        rHand.pitch = e.frame.Hands[i].Direction.Pitch;
                        rHand.yaw = e.frame.Hands[i].Direction.Yaw;
                        rHand.pinch = e.frame.Hands[i].PinchDistance;
                        rHand.strength = e.frame.Hands[i].PinchStrength;
                        rHand.time = e.frame.Hands[i].TimeVisible;

                        for (int j = 0; j < e.frame.Hands[i].Fingers.Count; j++)
                        {
                            rHand.fingers[j].x = e.frame.Hands[i].Fingers[j].TipPosition.x;
                            rHand.fingers[j].y = e.frame.Hands[i].Fingers[j].TipPosition.y;
                            rHand.fingers[j].z = e.frame.Hands[i].Fingers[j].TipPosition.z;
                            rHand.fingers[j].dx = e.frame.Hands[i].Fingers[j].Direction.x;
                            rHand.fingers[j].dy = e.frame.Hands[i].Fingers[j].Direction.y;
                            rHand.fingers[j].dz = e.frame.Hands[i].Fingers[j].Direction.z;
                            rHand.fingers[j].extended = e.frame.Hands[i].Fingers[j].IsExtended;
                        }

                    }
                }
                
            }
        }

        public void Stop()
        {
            //Clean up here
        }

        public event EventHandler Started;

        public string FriendlyName
        {
            get { return "UltraLeap plugin"; }
        }

        public bool GetProperty(int index, IPluginProperty property)
        {
            return false;
        }

        public bool SetProperties(Dictionary<string, object> properties)
        {
            return false;
        }

        public void DoBeforeNextExecute()
        {
            //This method will be executed each iteration of the script
        }
    }

    [Global(Name = "ultraleap")]
    public class UltraLeapPluginGlobal
    {
        private readonly UltraLeapPlugin ultraleap;

        public UltraLeapPluginGlobal(UltraLeapPlugin ultraleap)
        {
            this.ultraleap = ultraleap;
        }

        public int HandCount
        {
            get { return ultraleap.handsCount; }
        }
        public Extremity leftHand
        { 
            get { return ultraleap.lHand; }
        }

        public Extremity rightHand
        {
            get { return ultraleap.rHand; }
        }
    }
}
