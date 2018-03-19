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
        public static int ScreenWidth;
        public static int ScreenHeight;

        private const string WindowTitle = "UDA Project - Coded by Hawk";
        public static readonly List<GoldenRectangle> Rectangles = new List<GoldenRectangle>();
        public static Stopwatch Stopwatch = new Stopwatch();
        private const int RectanglesNumber = 16;
        private Camera Camera;

        public Window() : base(1280, 720, GraphicsMode.Default, WindowTitle, GameWindowFlags.Default)
        {
            GL.Enable(EnableCap.Blend);
            Stopwatch = Stopwatch.StartNew();
            Load += OnLoad;
            Resize += OnResize;
            RenderFrame += OnDraw;
            UpdateFrame += OnUpdate;
            Mouse.ButtonDown += OnMouseDown;
        }

        private void OnLoad(object sender, System.EventArgs e)
        {
            GL.ClearColor(Color.White);

            Camera = new Camera(position: Vector2.Zero, rotation: 0f, zoom: 1f);
        }


        public static float DeltaTime => _elapsedTime;
        private static float _elapsedTime;
        private long _currentFrame;
        private long _lastFrame = -1;
        private void OnDraw(object sender, FrameEventArgs e)
        {
            _currentFrame = Stopwatch.ElapsedMilliseconds;
            if (_lastFrame < 0)
                _lastFrame = _currentFrame;
            _elapsedTime = (_currentFrame - _lastFrame) / 1000f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GUI.Begin(Width, Height);
            Camera.CreateMatrix();

            foreach (GoldenRectangle drawable in Rectangles)
                drawable.Draw();

            SwapBuffers();

            _lastFrame = _currentFrame;
        }

//        private void RecursiveGoldenRectangle(int layer, GoldenRectangle rect)
//        {
//            while (true)
//            {
//                GUI.DrawGoldenRectangle(rect, Color.Brown);
//                if (layer++ < 10)
//                {
//                    if (rect.Width > rect.Height)
//                        rect = new GoldenRectangle(Side.Right, rect.Position.X + rect.Height, rect.Position.Y, rect.Width - rect.Height, rect.Height);
//                    else
//                        rect = new GoldenRectangle(Side.Right, rect.Position.X, rect.Position.Y + rect.Width, rect.Width, rect.Height - rect.Width);
////                    Camera.MoveTo(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
//                    continue;
//                }
//                break;
//            }
//        }


        private void OnUpdate(object sender, FrameEventArgs e)
        {
            Title = $"{WindowTitle} - FPS: {1f / e.Time:0.} - DeltaTime: {_elapsedTime}";
            if (Keyboard[Key.Escape])
                Exit();

            Camera.Update();

//            if (_drawables.FirstOrDefault() is GoldenRectangle rectangle && !Camera.IsVisible(rectangle))
//                _drawables.RemoveAt(0);

            if (Rectangles.Count >= RectanglesNumber) return;

            if (Rectangles.LastOrDefault() is GoldenRectangle rectangle)
                Rectangles.Add(rectangle.Next);
            else
                Rectangles.Add(new GoldenRectangle(Side.Right, null, Height - 20));
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            ScreenWidth = Width;
            ScreenHeight = Height;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }       
    }
}
