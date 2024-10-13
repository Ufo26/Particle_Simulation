using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Particle_Simulation
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CreateResourcesDirIfDoesNotExist();
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch { }
        }

        static void CreateResourcesDirIfDoesNotExist() {
            string directoryPath = "RESOURCES";

            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine("Directory 'RESOURCES' created.");
            }
            else
            {
                Console.WriteLine("Directory 'RESOURCES' already exists.");
            }
        }
    }
}
