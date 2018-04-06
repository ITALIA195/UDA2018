using OpenTK;

namespace UDA2018.GoldenRatio
{
    public class Tracker
    {
        private readonly ITrackable _obj;
        private CallbackTrackFinish _callback;
        private Vector2 _startPosition, _endPosition;
        private float _startZoom, _endZoom;

        public Tracker(ITrackable obj)
        {
            _obj = obj;
            TrackNext();
        }

        private void TrackNext()
        {
            _startPosition = _obj.Traslation;
            _endPosition = _obj.GetNextTraslation();
            _startZoom = _obj.Zoom;
            _endZoom = _obj.GetNextZoom();
            _callback = _obj.TrackFinish();
            _obj.Index++;
        }

        private float _time;
        public void Update()
        {
            if (_callback?.Invoke() == true) return;
            _callback = null;
            if (_time >= 1)
            {
                TrackNext();
                _time = 0;
                return;
            }

            _time += Window.DeltaTime / 2;
            _obj.Traslation = GoldenMath.Lerp(_startPosition, _endPosition, _time);
            _obj.Zoom = GoldenMath.Lerp(_startZoom, _endZoom, _time);
        }
    }

    public delegate bool CallbackTrackFinish();
}
