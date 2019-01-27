using OpenTK;

namespace GoldenRatio
{
    public interface ITrackable
    {
        int Index { get; set; }

        Vector2 Translation { get; set; }
        float Zoom { get; set; }
        float Rotation { get; set; }

        float GetNextRotation();
        Vector2 GetNextTranslation();
        float GetNextZoom();
        CallbackTrackFinish TrackFinish();
    }
}
