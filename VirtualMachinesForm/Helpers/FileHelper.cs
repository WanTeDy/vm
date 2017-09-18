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
            SaveString(str, path);
        }

        public static void SaveString(string str, string path)
        {            
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(str);
                sw.Close();
            }
        }

        public static string GetString(string filename = "resource.txt")
        {
            string path = Directory.GetCurrentDirectory() + filename;
            var str = "[]";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    str = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return str;
        }
    }
}
