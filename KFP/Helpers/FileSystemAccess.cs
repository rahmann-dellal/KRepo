using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Helpers
{
    public class FileSystemAccess
    {
        public FileSystemAccess() { }
        public bool DeleteFile(string? path)
        {
            try {
                File.Delete(path);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public byte[]? ReadFile(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
