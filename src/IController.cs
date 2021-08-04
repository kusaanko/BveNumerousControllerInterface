using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public interface IController: IDisposable
    {
        bool[] GetButtons();
        int[] GetSliders();
        State Read();
        string GetName();
        string GetControllerType();
    }

    public class State
    {
        public bool IsFailure;
    }
}
