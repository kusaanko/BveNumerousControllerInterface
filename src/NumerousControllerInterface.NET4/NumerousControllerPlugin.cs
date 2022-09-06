using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public interface NumerousControllerPlugin : IDisposable
    {
        string GetName();
        string GetVersion();
        void Load();
        List<NCIController> GetAllControllers();
    }
}
