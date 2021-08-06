using System;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public abstract class Controller : IDisposable
    {
        public abstract bool[] GetButtons();
        public abstract int[] GetSliders();
        public abstract string GetName();
        public abstract string GetControllerType();

        public abstract void Dispose();
    }
}
