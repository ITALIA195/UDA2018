using OpenTK;

namespace GoldenRatio
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

        public void TrackNext(bool animation = true)
        {
            _startPosition = _obj.Translation;
            _endPosition = _obj.GetNextTranslation();
            _startZoom = _obj.Zoom;
            _endZoom = _obj.GetNextZoom();
            if (animation)
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
            _obj.Translation = GoldenMath.Lerp(_startPosition, _endPosition, _time);
            _obj.Zoom = GoldenMath.Lerp(_startZoom, _endZoom, _time);
        }
    }

    public delegate bool CallbackTrackFinish();
}
