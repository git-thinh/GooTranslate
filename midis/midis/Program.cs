using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace midis
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (se, ev) =>
            {
                Assembly asm = null;
                string comName = ev.Name.Split(',')[0];
                string resourceName = @"dll\" + comName + ".dll";
                var assembly = Assembly.GetExecutingAssembly();
                resourceName = typeof(Program).Namespace + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        //Debug.WriteLine(resourceName);
                    }
                    else
                    {
                        byte[] buffer = new byte[stream.Length];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                ms.Write(buffer, 0, read);
                            buffer = ms.ToArray();
                        }
                        asm = Assembly.Load(buffer);
                    }
                }
                return asm;
            };
        }

        static void Main(string[] args)
        {
            string file = @"C:\GooTranslate\GooTranslate\data\word_key\keyword.txt";
            if (args.Length > 0)
                file = args[0];
            if (File.Exists(file)) {
                string[] a = File.ReadAllText(file)
                    .Split(new char[] { ' ', '.', ',', '_', '-', '\n', '\r' })
                    .Select(x => x.Trim().ToLower())
                    .Select(x => System.Text.RegularExpressions.Regex.Replace(x, @"[^a-z]", string.Empty).Trim())
                    .Where(x => x.Length > 2)
                    .Select(x => (x[x.Length - 1] == 's' && x[x.Length - 2] != 's' && x != "this") ? x.Substring(0, x.Length - 1) : x)
                    .Distinct()
                    .OrderBy(x => x)
                    .Where(x => x.Length > 2)
                    .ToArray();
                string s = string.Join(Environment.NewLine, a);
                File.WriteAllText(file, s);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
