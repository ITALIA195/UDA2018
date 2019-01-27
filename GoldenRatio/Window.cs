using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace GoldenRatio
{
    public class Window : GameWindow
    {
        private const string WindowTitle = "UDA Project - Coded by Hawk";
        private RectangleManager _rectangleManager;
        private static float _screenRatio;
        private float _lineWidth = 3f;

        public Window() : base(WindowSize.Width, WindowSize.Height, GraphicsMode.Default, WindowTitle, GameWindowFlags.Default, DisplayDevice.Default)
        {
            _screenRatio = (float)Width / Height;
            GL.Enable(EnableCap.Blend);
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.Coral);
            GL.LineWidth(_lineWidth);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            WindowBorder = WindowBorder.Fixed;

            _rectangleManager = new RectangleManager();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _rectangleManager.Draw();
            
            GL.Flush();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (e.Time < 0.0005) return; //TODO: Check if works in less powerful computers
            Title = $"{WindowTitle} - FPS: {1f / e.Time:0.}";

            if (_pause) return;
            _rectangleManager.Update();
        }

        protected override void OnResize(EventArgs e)
        {
            _screenRatio = (float)Width / Height;
            GL.Viewport(0, 0, Width, Height);
        }

        private bool _pause;

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Q:
                case Key.Escape:
                    Exit();
                    break;
                
                case Key.P:
                    _pause = !_pause;
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
                            Width = WindowSize.Width;
                            Height = WindowSize.Height;
                            break;
                        
                        default: // Windowed
                            WindowState = WindowState.Fullscreen;
                            Width = (int)Screen.Width;
                            Height = (int)Screen.Height;
                            break;
                    }
                    break;
            }
        }

        public static float DeltaTime => 0.01f;
        public static float OneOverScreenRatio => 1f / _screenRatio;
        public static Rectangle Screen => System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static Size WindowSize
        {
            get
            {
                float width = Screen.Width * 0.66666f;
                return new Size((int) width, (int) (width / GoldenMath.Ratio));
            }
        }
    }
}
