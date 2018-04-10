using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Window : GameWindow
    {
        private const string WindowTitle = "UDA Project - Coded by Hawk";
        private Rectangles _rectangles;
        private static float _screenRatio;
        private float _lineWidth = 3f;

        public Window() : base(WindowSize.Width, WindowSize.Height, GraphicsMode.Default, WindowTitle, GameWindowFlags.Default, DisplayDevice.Default)
        {
            _screenRatio = (float)Width / Height;
            GL.Enable(EnableCap.Blend);
            VSync = VSyncMode.Off;
            Load += OnLoad;
            Resize += OnResize;
            RenderFrame += OnDraw;
            UpdateFrame += OnUpdate;
            Keyboard.KeyDown += OnKeyDown;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.LineWidth(_lineWidth);
            WindowBorder = WindowBorder.Fixed;

            _rectangles = new Rectangles();
        }

        private void OnDraw(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (GoldenRectangle rectangle in _rectangles)
                rectangle.Draw();

            GL.Flush();
            SwapBuffers();
        }

        private void OnUpdate(object sender, FrameEventArgs e)
        {
            if (e.Time < 0.0005) return; //TODO: Check if works in less powerfull computers
            Title = $"{WindowTitle} - FPS: {1f / e.Time:0.}";
            if (Keyboard[Key.Escape])
                Exit();

            if (_pause) return;
            _rectangles.Update();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _screenRatio = (float)Width / Height;
            GL.Viewport(0, 0, Width, Height);
        }

        private bool _pause;
        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
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
        public static float ScreenRatio => _screenRatio;
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
