using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CryptoUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            //TO Do
            //if(args.Length != 1)
            //Receive arguments to be able to launch it from Windows explorer Context Menu Item.

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormCryptoUtil());
        }
    }
}
