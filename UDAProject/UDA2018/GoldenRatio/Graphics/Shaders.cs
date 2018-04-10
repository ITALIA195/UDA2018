using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace UDA2018.GoldenRatio.Graphics
{
    public struct Shaders
    {
        private readonly int _vertexShader;
        private readonly int _fragmentShader;

        public Shaders(string vertexFileName, string fragmentFileName)
        {
            LoadShader(vertexFileName, ShaderType.VertexShader, out _vertexShader);
            LoadShader(fragmentFileName, ShaderType.FragmentShader, out _fragmentShader);
        }

        public void LinkProgram(int programId)
        {
            GL.AttachShader(programId, _vertexShader);
            GL.AttachShader(programId, _fragmentShader);
        }

        private static string GetShader(string shaderName)
        {
            using (Stream stream = Startup.Assembly.GetManifestResourceStream($@"UDA2018.GoldenRatio.Shaders.{shaderName}.shader"))
                if (stream != null)
                    using (StreamReader reader = new StreamReader(stream))
                        return reader.ReadToEnd();
            return null;
        }

        private static void LoadShader(string fileName, ShaderType type, out int shaderId)
        {
            shaderId = GL.CreateShader(type);
            GL.ShaderSource(shaderId, GetShader(fileName));
            GL.CompileShader(shaderId);
            Console.WriteLine(GL.GetShaderInfoLog(shaderId));
        }

    }
}
