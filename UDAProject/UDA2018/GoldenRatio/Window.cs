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
        private long _currentFrame;
        private long _lastFrame = -1;

        private const string WindowTitle = "UDA Project - Coded by Hawk";
        public static readonly List<IDrawable> Drawables = new List<IDrawable>();
        public static Stopwatch Stopwatch = new Stopwatch();
        private const int RectanglesNumber = 10;
        private Camera Camera;

        public Window() : base(ScreenWidth, ScreenHeight, GraphicsMode.Default, WindowTitle, GameWindowFlags.Default)
        {
            GL.Enable(EnableCap.Blend);
            Stopwatch = Stopwatch.StartNew();
            Load += OnLoad;
            Resize += OnResize;
            RenderFrame += OnDraw;
            UpdateFrame += OnUpdate;
        }

        private void OnLoad(object sender, System.EventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.LineWidth(3f);
            WindowBorder = WindowBorder.Fixed;

            Camera = new Camera(position: Vector2.Zero, rotation: 0f, zoom: 1f);

            // Create rectangles
            while (Drawables.Count < RectanglesNumber)
            {
                if (Drawables.LastOrDefault() is GoldenRectangle rectangle)
                    Drawables.Add(rectangle.Next);
                else
                    Drawables.Add(new GoldenRectangle(Side.Right, null, Height - 20));
            }
        }

        private void OnDraw(object sender, FrameEventArgs e)
        {
            _currentFrame = Stopwatch.ElapsedMilliseconds;
            if (_lastFrame < 0)
                _lastFrame = _currentFrame;
            _elapsedTime = (_currentFrame - _lastFrame) / 1000f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Camera.CreateMatrix();

//            GL.PointSize(3f);
//            GL.Begin(PrimitiveType.Points);
//
//            GL.Color3(0, 1, 0);
//            GL.Vertex2(0, 0);
//
//            GL.End();

            foreach (IDrawable rectangle in Drawables)
                rectangle.Draw();

            GL.Flush();
            SwapBuffers();

            _lastFrame = _currentFrame;
        }

        private void OnUpdate(object sender, FrameEventArgs e)
        {
            Title = $"{WindowTitle} - FPS: {1f / e.Time:0.}";
            if (Keyboard[Key.Escape])
                Exit();

            foreach (IDrawable rectangle in Drawables)
                rectangle.Update();
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            ScreenWidth = Width;
            ScreenHeight = Height;
        }
    }
}
