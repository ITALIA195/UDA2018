using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public struct Shaders
    {
        private readonly int _programId;

        public int ProgramID => _programId;

        public Shaders(string vertexFileName, string fragmentFileName)
        {
            _programId = GL.CreateProgram();
            LoadShader(vertexFileName, ShaderType.VertexShader, _programId);
            LoadShader(fragmentFileName, ShaderType.FragmentShader, _programId);
        }

        private static void LoadShader(string fileName, ShaderType type, int programId)
        {
            int id = GL.CreateShader(type);
            using (StreamReader stream = new StreamReader($"UDA2018\\GoldenRatio\\Shaders\\{fileName}.shader"))
                GL.ShaderSource(id, stream.ReadToEnd());
            GL.CompileShader(id);
            GL.AttachShader(programId, id);
            Console.Write(GL.GetShaderInfoLog(id));
        }
    }
}
