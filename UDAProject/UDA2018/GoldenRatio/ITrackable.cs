using OpenTK;

namespace UDA2018.GoldenRatio
{
    public interface ITrackable
    {
        TrackInfo TrackInfo { get; }
        Vector2 TrackPosition { get; set; }
        float TrackZoom { get; set; }

        CallbackTrackFinish TrackFinish();
    }
}
