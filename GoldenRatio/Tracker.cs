﻿using OpenToolkit.Mathematics;

namespace GoldenRatio
{
    public class Tracker
    {
        private readonly ITrackable _obj;
        private CallbackTrackFinish _callback;
        private Vector2 _startPosition, _endPosition;
        private float _startZoom, _endZoom;
        private float _time;

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

            _time += 0.001f / 2;
            _obj.Translation = Vector2.Lerp(_startPosition, _endPosition, _time);
            _obj.Zoom = FloatLerp(_startZoom, _endZoom, _time);
        }

        private static float FloatLerp(float a, float b, float blend)
        {
            return a + blend * (b - a);
        }
    }

    public delegate bool CallbackTrackFinish();
}
