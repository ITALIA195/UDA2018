using System.Collections.Generic;
using OpenTK;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Tracker<T> : List<T> where T: ITrackable // I'd like to derive from Queue<T>
    {
        private T _tracked;
        private int _index;

        private CallbackTrackFinish _callback;
        private Vector2 _startPosition, _endPosition;
        private float _startZoom, _endZoom;

        public Tracker(IEnumerable<T> objects) : base(objects)
        {
            
        }

        private void TrackNext()
        {
            _tracked = this[_index++];
            _startPosition = GoldenRectangle.TrackPosition;
            _endPosition = _startPosition + _tracked.TrackInfo.OffsetPosition;
            _startZoom = GoldenRectangle.TrackZoom;
            _endZoom = _tracked.TrackInfo.ZoomOffset;
            _callback = _tracked.TrackFinish();
        }

        public void Reset()
        {
            _index = 0;
            _tracked = this[_index++];
            _startPosition = Vector2.Zero;
            _endPosition = _startPosition + _tracked.TrackInfo.OffsetPosition;
            _startZoom = 1f;
            _endZoom = _tracked.TrackInfo.ZoomOffset;
        }

        private float _time;
        public void Update()
        {
            if (_callback?.Invoke() == true) return;
            _callback = null;
            if (_tracked == null || _time >= 1)
            {
                TrackNext();
                _time = 0;
                return;
            }

            _time += Window.DeltaTime / 2;
            GoldenRectangle.TrackPosition = GoldenMath.Lerp(_startPosition, _endPosition, _time);
            GoldenRectangle.TrackZoom = GoldenMath.Lerp(_startZoom, _endZoom, _time);
        }
    }

    public delegate bool CallbackTrackFinish();

    public struct TrackInfo
    {
        public CallbackTrackFinish Callback { get; set; }
        public float ZoomOffset { get; set; }
        public Vector2 OffsetPosition { get; set; }
    }
}
