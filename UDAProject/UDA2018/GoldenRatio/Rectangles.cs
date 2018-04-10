using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Rectangles : List<GoldenRectangle>, ITrackable, IDisposable
    {
        public static Rectangles Instance;

        public static Program Program;
        public static CColor GradientStart;
        public static CColor GradientEnd;
        public static Shaders Shader;
        public static Shaders SquareShader;

        private static int _index;
        private static float _zoom = 1f;
        private static float _rotation;
        private static Vector2 _traslation = Vector2.Zero;

        private static Tracker _tracker;
        private const int RectanglesNumber = 12;

        public Rectangles()
        {
            Instance = this;

            Program = new Program(GL.CreateProgram());
            Shader = new Shaders("vertex", "fragment");
            Shader.LinkProgram(Program.ID);
            SquareShader = new Shaders("squareVertex", "squareFragment");

            while (Count < RectanglesNumber)
            {
                if (this.LastOrDefault() is GoldenRectangle rectangle)
                    Add(rectangle.Next);
                else
                    Add(new GoldenRectangle(Side.Right, null, 1.9f));
            }

            _tracker = new Tracker(this);
        }

        public void Update()
        {
            GradientStart.Fade();
            GradientEnd.Fade();

            foreach (GoldenRectangle rectangle in this)
                rectangle.Update();

            _tracker.Update();
        }

        public static void CreateZoomMatrix(out Matrix4 matrix)
        {
            matrix = Matrix4.Identity;
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateTranslation(-_traslation.X, -_traslation.Y, 0f));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateRotationZ(_rotation));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(_zoom, _zoom, 1f));
        }

        public void ResetTracking()
        {
            _index = 0;
            _traslation = Vector2.Zero;
            _zoom = 1f;
            _rotation = 0f;
            _tracker.TrackNext(false);
        }

        #region Tracking

        private GoldenRectangle _tracked => this[Index];

        public float GetNextRotation() => 0f;
        public Vector2 GetNextTraslation() => _traslation + _tracked.OffsetPosition;
        public float GetNextZoom() => GoldenMath.Min(Window.ScreenRatio * 2 / _tracked.SubRectangle.Width, 1.9f / _tracked.SubRectangle.Height);
        public CallbackTrackFinish TrackFinish() => _tracked.TrackFinish();

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public Vector2 Traslation
        {
            get => _traslation;
            set => _traslation = value;
        }

        public float Zoom
        {
            get => _zoom;
            set => _zoom = value;
        }

        public float Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        #endregion

        public void Dispose()
        {
            for (int i = 0; i < Count; i++)
                this[i].Dispose();
            Clear();
        }
    }
}
