using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Framework.Exceptions;
using OpenToolkit.Graphics.OpenGL4;

namespace GoldenRatio
{
    public static class Startup
    {
        private static readonly DebugProc DebugProcCallback = DebugCallback;
        
        public static void Main()
        {
            using var window = new Window();
            window.Run();
        }

        [Conditional("DEBUG")]
        public static void EnableGLLogging()
        {
            GL.DebugMessageCallback(DebugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }
        
        [DebuggerStepThrough]
        private static void DebugCallback
        (
            DebugSource source,
            DebugType type,
            int id,
            DebugSeverity severity,
            int length,
            IntPtr message,
            IntPtr userParam
        )
        {
            var messageString = Marshal.PtrToStringAnsi(message, length);

            Console.WriteLine($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                throw new GLException(messageString);
            }
        }
    }
}
