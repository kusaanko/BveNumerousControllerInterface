using System;
using System.Collections.Generic;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public abstract class NCIController : IDisposable
    {
        public enum Reverser
        {
            CENTER,
            FORWARD,
            BACKWARD,
        }
        public static int DEFAULT_SLIDER_MIN_VALUE { get => -1000; }
        public static int DEFAULT_SLIDER_MAX_VALUE { get => 1000; }
        public abstract bool[] GetButtons();
        public abstract int[] GetSliders();
        public abstract string GetName();
        public abstract string GetControllerType();

        public bool[] GetButtonsSafe()
        {
            if(!IsDisposed())
            {
                return GetButtons();
            }
            return new bool[0];
        }

        public int[] GetSlidersSafe()
        {
            if (!IsDisposed())
            {
                return GetSliders();
            }
            return new int[0];
        }

        public virtual int GetSliderMinValue()
        {
            return DEFAULT_SLIDER_MIN_VALUE;
        }
        public virtual int GetSliderMaxValue()
        {
            return DEFAULT_SLIDER_MAX_VALUE;
        }

        public virtual int GetPowerCount()
        {
            return 0;
        }

        public virtual int GetPower()
        {
            return 0;
        }

        public virtual int GetBreakCount()
        {
            return 0;
        }

        public virtual int GetBreak()
        {
            return 0;
        }

        public virtual bool HasReverser()
        {
            return false;
        }

        public virtual Reverser GetReverser()
        {
            return Reverser.CENTER;
        }

        public virtual string[] GetButtonNames()
        {
            return new string[0];
        }
        public virtual bool HasOutputs()
        {
            return false;
        }

        public virtual Dictionary<string, OutputType> GetOutputs()
        {
            return null;
        }

        public virtual Dictionary<string, OutputHint> GetOutputHints()
        {
            return null;
        }

        public virtual void SetOutput(string key, object value)
        {

        }

        public virtual void SendOutput()
        {

        }

        public abstract void Dispose();

        public abstract bool IsDisposed();
    }

    public enum OutputHint
    {
        SpeedMeter,
        PowerNotch,
        BreakNotch,
        ATC,
        DoorLamp,
    }

    public enum OutputType
    {
        Bool,
        Int,
        Double,
        String,
    }
}
