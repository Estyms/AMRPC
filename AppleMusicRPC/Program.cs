using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppleMusicRPC
{
    internal class Program
    {


        [STAThread]
        private static void Main()
        {

            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly()?.Location)).Length > 1) return;

            _ = new Provider();

            Application.Run(new ServiceApplicationContext());
        }

    }
}
