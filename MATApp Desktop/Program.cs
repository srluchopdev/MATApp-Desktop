using MATApp_Desktop;
using System;
using System.Windows.Forms;

namespace MATAppDesktop
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Cambiado a Form1
        }
    }
}