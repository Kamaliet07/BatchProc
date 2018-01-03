using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace ControlFileGenerator.Model
{
   public class GlobalApplication
    {
       public const string ProcessName = "ISATI";
        /// <summary>
        /// Finds a named resource folder. (Allows for flexable deployment.)
        /// </summary>
        /// <returns></returns>
        public static string getResourcePath(string resourceName)
        {
            //NOTE: For right now this just searches for a directory with the requested name.
            //NOTE: In the future you could imagine adding more advanced logic here (config file driven, whatever)

            string retVal;
            string appPath = System.Reflection.Assembly.GetAssembly(typeof(GlobalApplication)).Location;
            // Always gets the location of the assembly which contains the PrmApplication object.

            //Check to see if we're running under Visual Studio dev environment:
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;//Application.ExecutablePath;
            exePath = exePath.ToLower();
            if ((exePath.IndexOf("devenv.exe") > -1))
            {
                //If we are running under visual studio we're in design mode & we can't trust the assembly path.
                //	This is because visual studio makes a shadow copy of the assembly for use in design mode.

                //	In this case I use the assembly debug information to find the location of the file which was
                //		compiled to generate this class:
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                appPath = st.GetFrame(0).GetFileName(); // ? Is there a better way to get this file name?? 
            }

            int lastIndex = appPath.LastIndexOf("\\");
            retVal = appPath = appPath.Remove(lastIndex, appPath.Length - lastIndex) + "\\" + resourceName + "\\";
            while (appPath.Contains("\\bin") && (lastIndex = appPath.LastIndexOf("\\")) > 0)
            {
                appPath = appPath.Remove(lastIndex, appPath.Length - lastIndex);
            }

            if (Directory.Exists(appPath + "\\" + resourceName + "\\"))
            {
                retVal = appPath + "\\" + resourceName + "\\";
            }

            return (retVal);
        }

    }
}
