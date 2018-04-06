using OpenTK;

namespace UDA2018.GoldenRatio
{
    public interface ITrackable
    {
        int Index { get; set; }

        Vector2 Traslation { get; set; }
        float Zoom { get; set; }
        float Rotation { get; set; }

        float GetNextRotation();
        Vector2 GetNextTraslation();
        float GetNextZoom();
        CallbackTrackFinish TrackFinish();
    }
}
