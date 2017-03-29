using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VirtualMachinesForm.Helpers
{
    public static class FileHelper
    {
        public static void SaveString(string str)
        {
            string path = Directory.GetCurrentDirectory() + "resource.txt";
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(str);
            }
        }

        public static string GetString()
        {
            string path = Directory.GetCurrentDirectory() + "resource.txt";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
            return "";
        }
    }
}
