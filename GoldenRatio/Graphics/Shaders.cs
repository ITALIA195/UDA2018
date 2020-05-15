using System;
using System.IO;
using OpenToolkit.Graphics.OpenGL4;

namespace GoldenRatio.Graphics
{
    public class Shaders
    {
        private readonly int _handle;

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

            _handle = GL.CreateProgram();
            
            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);
            
            GL.LinkProgram(_handle);
            
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out code);
            if (code != (int) All.True)
            {
                Console.WriteLine("Error occurred whilst linking program {0} ({1}): {2}",
                    _handle, (All) code, GL.GetProgramInfoLog(_handle));
                throw new Exception("Error linking program.");
            }
            
            // Dispose of shaders, they have been compiled into the program
            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Bind()
        {
            GL.UseProgram(_handle);
        }
        
        public int GetUniformLocation(string uniform)
        {
            int location = GL.GetUniformLocation(_handle, uniform);
            if (location < 0)
                throw new Exception($"Failed to find uniform {uniform}");
            return location;
        }

        private static string GetShader(string shaderName)
        {
            using var stream = Startup.Assembly.GetManifestResourceStream($@"GoldenRatio.Graphics.Shaders.{shaderName}");
            using var reader = new StreamReader(stream ?? throw new Exception("Embedded Resource missing"));
            
            return reader.ReadToEnd();
        }
    }
}
