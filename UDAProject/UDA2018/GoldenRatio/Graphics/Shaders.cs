﻿using System.IO;
using OpenTK.Graphics.OpenGL;

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

        private static void LoadShader(string fileName, ShaderType type, out int shaderId)
        {
            shaderId = GL.CreateShader(type);
            using (StreamReader stream = new StreamReader(Startup.ShaderStream(fileName)))
                GL.ShaderSource(shaderId, stream.ReadToEnd());
            GL.CompileShader(shaderId);
        }

    }
}
