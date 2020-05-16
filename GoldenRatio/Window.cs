using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Hawk.Framework.Exceptions;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

namespace GoldenRatio
{
    public class Window : GameWindow
    {
        private const string WindowTitle = "Golden Ratio Visualization - FPS: {0:0}";
        private const int BaseWidth = 1280;
        private const int BaseHeight = 720;

        private float _lineWidth = 3f;

        private DebugProc _debugProc = DebugCallback;

        public Window() : base(GameWindowSettings.Default, NativeWindowSettings)
        {
            Load += OnLoad;
            UpdateFrame += OnUpdate;
            Resize += OnResize;
            KeyDown += OnKeyDown;
        }

        private static NativeWindowSettings NativeWindowSettings =>
            new NativeWindowSettings
            {
                Title = WindowTitle,
                Size = new Vector2i(BaseWidth, BaseHeight),
                APIVersion = new Version(4, 6),
                Profile = ContextProfile.Core
            };

        public static float ScreenRatio { get; private set; } = BaseWidth / (float) BaseHeight;

        private new void OnLoad()
        {
            GL.ClearColor(Color.Coral);
            GL.LineWidth(_lineWidth);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.DebugMessageCallback(_debugProc, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            var rectangleManager = new RectangleManager();
            
            RenderFrame += rectangleManager.Render;
            UpdateFrame += rectangleManager.Update;
            Resize += rectangleManager.OnResize;

            // var test = new TestBlink();

            // RenderFrame += test.Render;
            // UpdateFrame += test.Update;
            // Resize += test.OnResize;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            base.OnRenderFrame(e);
            
            SwapBuffers();
            GL.Flush();
        }

        private void OnUpdate(FrameEventArgs e)
        {
            Title = string.Format(WindowTitle, 1 / e.Time);
        }

        private new static void OnResize(ResizeEventArgs e)
        {
            ScreenRatio = e.Width / (float) e.Height;
            GL.Viewport(0, 0, e.Width, e.Height);
        }
        
        private new void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Q:
                case Key.Escape:
                case Key.F4 when e.Alt:
                    Close();
                    break;
                
                case Key.R:
                    _lineWidth -= 1f;
                    GL.LineWidth(_lineWidth);
                    break;
                
                case Key.T:
                    _lineWidth += 1f;
                    GL.LineWidth(_lineWidth);
                    break;
                
                case Key.F11:
                    switch (WindowState)
                    {
                        case WindowState.Fullscreen:
                            WindowState = WindowState.Normal;
                            Size = new Vector2i(BaseWidth, BaseHeight);
                            break;
                        
                        case WindowState.Normal:
                            WindowState = WindowState.Fullscreen;
                            Size = new Vector2i(1920, 1080); //TODO: Support all resolutions
                            break;
                    }
                    break;
            }
        }
        
        private static void DebugCallback(
            DebugSource source,
            DebugType type,
            int id,
            DebugSeverity severity,
            int length,
            IntPtr message,
            IntPtr userParam)
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
