using System;
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
        private const string WindowTitle = "UDA Project - Coded by Hawk";
        private static float _elapsedTime;
        private Rectangles _rectangles;
        private static int _width = 1280;
        private static int _height = 720;

        public Window() : base(_width, _height, GraphicsMode.Default, WindowTitle, GameWindowFlags.Default)
        {
            GL.Enable(EnableCap.Blend);
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

            _rectangles = new Rectangles();
        }

        private void OnDraw(object sender, FrameEventArgs e)
        {
            _elapsedTime = (float) e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (GoldenRectangle rectangle in _rectangles)
                rectangle.Draw();

            GL.Flush();
            SwapBuffers();
        }

        private void OnUpdate(object sender, FrameEventArgs e)
        {
            if (e.Time < 0.001) return; //TODO: Check if works in less powerfull computers
            Title = $"{WindowTitle} - FPS: {1f / e.Time:0.}";
            if (Keyboard[Key.Escape])
                Exit();

            _rectangles.Update();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _width = base.Width;
            _height = base.Height;

            GL.Viewport(0, 0, _width, _height);
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

        public static float DeltaTime => _elapsedTime;
        public new static int Width => _width;
        public new static int Height => _height;
    }
}
