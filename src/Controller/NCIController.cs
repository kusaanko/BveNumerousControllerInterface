using System;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public abstract class NCIController : IDisposable
    {
        public abstract bool[] GetButtons();
        public abstract int[] GetSliders();
        public abstract string GetName();
        public abstract string GetControllerType();

        public int GetPowerCount()
        {
            return 0;
        }

        public int GetPower()
        {
            return 0;
        }

        public int GetBreakCount()
        {
            return 0;
        }

        public int GetBreak()
        {
            return 0;
        }

        public abstract void Dispose();
    }
}
