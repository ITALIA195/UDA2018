using OpenTK;

namespace UDA2018.GoldenRatio
{
    public unsafe interface ITrackable
    {
        TrackInfo TrackInfo { get; }
        Vector2 TrackPosition { get; set; }
        float TrackZoom { get; set; }

        void TrackFinish(out bool* ptr);
    }
}
