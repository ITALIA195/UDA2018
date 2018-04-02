using System;
using System.Collections.Generic;
using OpenTK;

namespace UDA2018.GoldenRatio
{
    public class Tracker<T> where T: ITrackable
    {
        private readonly Queue<T> _objects;
        private T _tracked;

        private CallbackTrackFinish _callback;
        private Vector2 _startPosition, _endPosition;
        private float _startZoom, _endZoom;

        public Tracker(IEnumerable<T> objects)
        {
            _objects = new Queue<T>(objects);
        }

        private void TrackNext()
        {
            if (_objects.Count <= 0) return;
            _tracked = _objects.Dequeue();
            _startPosition = _tracked.TrackPosition;
            _endPosition = _startPosition + _tracked.TrackInfo.OffsetPosition;
            _startZoom = _tracked.TrackZoom;
            _endZoom = _tracked.TrackInfo.ZoomOffset;
            _callback = _tracked.TrackFinish();
        }


        private float _time;
        public void Update()
        {
            if (_callback?.Invoke() == true) return;
            if (_tracked == null || _time >= 1)
            {
                TrackNext();
                _time = 0;
                return;
            }

            _time += Window.DeltaTime / 2;
            _tracked.TrackPosition = GoldenMath.Lerp(_startPosition, _endPosition, _time);
            _tracked.TrackZoom = GoldenMath.Lerp(_startZoom, _endZoom, _time);
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
