using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace GoldenRatio.Graphics
{
    public class Shaders
    {
        private readonly int _programId;

        public Shaders(string shader)
        {
            // ===================
            // == Vertex Shader ==
            // ===================
            
            var source = GetShader(shader + ".vert");

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, source);
            GL.CompileShader(vertexShader);
            
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int code);
            if (code != (int) All.True)
            {
                Console.WriteLine("Error occurred whilst compiling vertex shader {0} ({1}): {2}",
                    vertexShader, (All) code, GL.GetShaderInfoLog(vertexShader));
                throw new Exception("Error compiling shader.");
            }
            
            // =====================
            // == Fragment Shader ==
            // =====================
            
            source = GetShader(shader + ".frag");

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, source);
            GL.CompileShader(fragmentShader);
            
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out code);
            if (code != (int) All.True)
            {
                Console.WriteLine("Error occurred whilst compiling fragment shader {0} ({1}): {2}",
                    fragmentShader, (All) code, GL.GetShaderInfoLog(fragmentShader));
                throw new Exception("Error compiling shader.");
            }
            
            // =============
            // == Program ==
            // =============

            _programId = GL.CreateProgram();
            
            GL.AttachShader(_programId, vertexShader);
            GL.AttachShader(_programId, fragmentShader);
            
            GL.LinkProgram(_programId);
            
            GL.GetProgram(_programId, GetProgramParameterName.LinkStatus, out code);
            if (code != (int) All.True)
            {
                Console.WriteLine("Error occurred whilst linking program {0} ({1}): {2}",
                    _programId, (All) code, GL.GetProgramInfoLog(_programId));
                throw new Exception("Error linking program.");
            }
            
            // Dispose of shaders, they have been compiled into the program
            GL.DetachShader(_programId, vertexShader);
            GL.DetachShader(_programId, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Bind()
        {
            GL.UseProgram(_programId);
        }
        
        public int GetUniformLocation(string uniform)
        {
            int location = GL.GetUniformLocation(_programId, uniform);
            if (location < 0)
                throw new Exception($"Failed to find uniform {uniform}");
            return location;
        }

        public int ProgramId => _programId;

        private static string GetShader(string shaderName)
        {
            using (Stream stream = Startup.Assembly.GetManifestResourceStream($@"GoldenRatio.Graphics.Shaders.{shaderName}"))
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
