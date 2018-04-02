using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Window : GameWindow
    {
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static float DeltaTime => _elapsedTime;
        private static float _elapsedTime;

        private const string WindowTitle = "UDA Project - Coded by Hawk";
        public static readonly List<GoldenRectangle> Rectangles = new List<GoldenRectangle>();
        public static Stopwatch Stopwatch = new Stopwatch();
        private Tracker<GoldenRectangle> _tracker;
        private const int RectanglesNumber = 10;

        public Window() : base(ScreenWidth, ScreenHeight, GraphicsMode.Default, WindowTitle, GameWindowFlags.Default)
        {
            GL.Enable(EnableCap.Blend);
            Stopwatch = Stopwatch.StartNew();
            Load += OnLoad;
            Resize += OnResize;
            RenderFrame += OnDraw;
            UpdateFrame += OnUpdate;
            Keyboard.KeyDown += OnKeyDown;
        }

        private void OnLoad(object sender, System.EventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.LineWidth(3f);
            WindowBorder = WindowBorder.Fixed;

            while (Rectangles.Count < RectanglesNumber)
            {
                if (Rectangles.LastOrDefault() is GoldenRectangle rectangle)
                    Rectangles.Add(rectangle.Next);
                else
                    Rectangles.Add(new GoldenRectangle(Side.Right, null, Height - 20));
            }

            _tracker = new Tracker<GoldenRectangle>(Rectangles);
        }

        private void OnDraw(object sender, FrameEventArgs e)
        {
            _elapsedTime = (float) e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (GoldenRectangle rectangle in Rectangles)
                rectangle.Draw();

            GL.Flush();
            SwapBuffers();
        }

        private void OnUpdate(object sender, FrameEventArgs e)
        {
            if (e.Time < 0.001) return;
            Title = $"{WindowTitle} - FPS: {1f / e.Time:0.}";
            if (Keyboard[Key.Escape])
                Exit();

            foreach (GoldenRectangle rectangle in Rectangles)
                rectangle.Update();

            _tracker.Update();
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            ScreenWidth = Width;
            ScreenHeight = Height;
        }

        private float _lineWidth = 1;
        private bool _pause;
        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.P:
                    _pause = !_pause;
                    if (_pause)
                        UpdateFrame -= OnUpdate;
                    else
                        UpdateFrame += OnUpdate;
                    break;
                case Key.X:
                    UpdateFrame -= OnUpdate;
                    break;
                case Key.R:
                    _lineWidth -= 1f;
                    GL.LineWidth(_lineWidth);
                    break;
                case Key.T:
                    _lineWidth += 1f;
                    GL.LineWidth(_lineWidth);
                    break;
            }
        }
    }
}
