using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumerousControllerTestPlugin
{
    class Controller : NCIController
    {
        private bool _disposed = false;
        public override void Dispose()
        {
            _disposed = true;
        }

        public override bool[] GetButtons()
        {
            return new bool[1];
        }

        public override string GetControllerType()
        {
            return "TestControllerDriver";
        }

        public override string GetName()
        {
            return "TestController";
        }

        public override int[] GetSliders()
        {
            return new int[0];
        }

        public override bool IsDisposed()
        {
            return _disposed;
        }

        public override int GetBreakCount()
        {
            return 9;
        }

        public override int GetBreak()
        {
            return 9;
        }

        public override int GetPowerCount()
        {
            return 5;
        }

        public override int GetPower()
        {
            return 0;
        }
    }
}
