using System;

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

        public abstract void Dispose();

        public abstract bool IsDisposed();
    }
}
