using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace UDA2018.GoldenRatio
{
    public static class Startup
    {
        public static Assembly Assembly => Assembly.GetExecutingAssembly();

        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            using (Window window = new Window())
                window.Run(60.0);
        }

        private static void OnLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine("LOADED: "+ args.LoadedAssembly.FullName);
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            MessageBox.Show("RESOLVING", args.Name);
            if (args.Name == "OpenTK, Version=3.0.0.0, Culture=neutral, PublicKeyToken=bad199fe84eb3df4")
            {
                using (Stream stream = Assembly.GetManifestResourceStream(@"UDA2018.GoldenRatio.Assembly.OpenTK.dll"))
                {
                    if (stream != null)
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        return Assembly.Load(data);
                    }
                }
            }
            return null;
        }

        public static Stream ShaderStream(string shaderName)
        {
            return Assembly.GetManifestResourceStream($@"UDA2018.GoldenRatio.Shaders.{shaderName}.shader");
        }
    }
}
