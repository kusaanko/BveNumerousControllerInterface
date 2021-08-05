using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using System.Diagnostics;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class DIJoystick : IController
    {
        private Joystick _stick;
        private string _name;

        public DIJoystick(Joystick stick, string name)
        {
            this._stick = stick;
            this._name = name;
            try
            {
                stick.Acquire();
                foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                {
                    if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                        stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                }
            }
            catch (DirectInputException)
            {
            }
        }

        public string GetName()
        {
            return _name;
        }
        public State Read()
        {
            State state = new State();
            state.IsFailure = false;
            return state;
        }

        public void Dispose()
        {
            _stick.Unacquire();
            _stick.Dispose();
        }

        public bool[] GetButtons()
        {
            int pov = _stick.GetCurrentState().GetPointOfViewControllers()[0];
            bool[] buttons = _stick.GetCurrentState().GetButtons();
            Array.Resize(ref buttons, 132);
            buttons[128] = (pov >= 0 && pov <= 4500) || (pov >= 31500 && pov <= 36000);
            buttons[129] = pov >= 4500 && pov <= 13500;
            buttons[130] = pov >= 13500 && pov <= 22500;
            buttons[131] = pov >= 22500 && pov <= 31500;
            return buttons;
        }

        public int[] GetSliders()
        {
            JoystickState state = _stick.GetCurrentState();
            int[] sliders = new int[24];
            int pos = 0;
            sliders[pos++] = state.AccelerationX;
            sliders[pos++] = state.AccelerationY;
            sliders[pos++] = state.AccelerationZ;
            sliders[pos++] = state.AngularAccelerationX;
            sliders[pos++] = state.AngularAccelerationY;
            sliders[pos++] = state.AngularAccelerationZ;
            sliders[pos++] = state.AngularVelocityX;
            sliders[pos++] = state.AngularVelocityY;
            sliders[pos++] = state.AngularVelocityZ;
            sliders[pos++] = state.ForceX;
            sliders[pos++] = state.ForceY;
            sliders[pos++] = state.ForceZ;
            sliders[pos++] = state.RotationX;
            sliders[pos++] = state.RotationY;
            sliders[pos++] = state.RotationZ;
            sliders[pos++] = state.TorqueX;
            sliders[pos++] = state.TorqueY;
            sliders[pos++] = state.TorqueZ;
            sliders[pos++] = state.VelocityX;
            sliders[pos++] = state.VelocityY;
            sliders[pos++] = state.VelocityZ;
            sliders[pos++] = state.X;
            sliders[pos++] = state.Y;
            sliders[pos++] = state.Z;
            return sliders;
        }

        public string GetControllerType()
        {
            return "DirectInput";
        }
    }
}
