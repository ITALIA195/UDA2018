using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Camera
    {
//        public int Index;
//        private GoldenRectangle _current;
//        private Vector2 _startPosition, _endPosition;
//        private float _startZoom, _endZoom;
//        public float _time;

        private Matrix4 _matrix;

        public Vector2 Position;
        public float Rotation;
        public float Zoom;

//        private Rect Area => new Rect(-Window.ScreenWidth / Zoom + Position.X, -Window.ScreenHeight / Zoom + Position.Y, Window.ScreenWidth / Zoom, Window.ScreenHeight / Zoom);

        public Camera(Vector2 position, float rotation, float zoom)
        {
            Position = position;
            Rotation = rotation;
            Zoom = zoom;
        }

//        private void FollowNext()
//        {
//            if (Window.Rectangles.Count <= 0) return;
//            if (_current != null) _current.Highlight = false;
//            _current = Window.Rectangles[Index++];
//            _startPosition = Position;
//            _startZoom = Zoom;
//            if (_current == null) throw new NullReferenceException($"{nameof(Window.Rectangles)}@{Index} is not an instance of {nameof(GoldenRectangle)}");
//            _endPosition = _current.SubRectangle;
//            _endZoom = GoldenMath.Min(Window.ScreenWidth / _current.Width, Window.ScreenHeight / _current.Height);
//            _current.Highlight = true;
//        }

//        public bool IsVisible(IDrawable obj)
//        {
//            if (obj.Vertices.All(point => !GoldenMath.IsInside(point, Area)))
//                return false;
//            return true;
//        }

        public Vector2 ToWorld(Vector2 input)
        {
            input /= Zoom;
            Vector2 dx = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            Vector2 dy = new Vector2((float)Math.Cos(Rotation + MathHelper.PiOver2), (float)Math.Sin(Rotation + MathHelper.PiOver2));
            return Position + dx * input.X + dy * input.Y;
        }
        
        public void Update()
        {
//            if (_current != null && _current.Highlight) return;
//            if (_current == null && Window.Rectangles.Count <= 0) return;
//            if (_current == null || _time >= 1f)
//            {
//                _time = 0f;
//                if (Index >= 5) // Zooming is not a good way of making it infinite (aka it would so zoomed in that a space unit would be all the monitor)
//                { 
//                    Index = 1;
//                    Position = Window.Rectangles[0].SubRectangle;
//                    Zoom = Window.ScreenWidth / Window.Rectangles[0].Width;
//                }
//                FollowNext();
//                return;
//            }
//
//            _time += Window.DeltaTime * .9f;
//            GoldenMath.Clamp(ref _time, 0f, 1f); 
//            Position = GoldenMath.Lerp(_startPosition, _endPosition, _time);
//            Zoom = GoldenMath.Lerp(_startZoom, _endZoom, _time);
        }

        public void CreateMatrix()
        {
            _matrix = Matrix4.Identity;
            _matrix = Matrix4.Mult(_matrix, Matrix4.CreateTranslation(-Position.X, -Position.Y, 0f));
            _matrix = Matrix4.Mult(_matrix, Matrix4.CreateRotationZ(-Rotation));
            _matrix = Matrix4.Mult(_matrix, Matrix4.CreateScale(Zoom, Zoom, 1f));
            GL.MultMatrix(ref _matrix);
        }

        public Matrix4 ProjectionMatrix => _matrix;
    }
}
