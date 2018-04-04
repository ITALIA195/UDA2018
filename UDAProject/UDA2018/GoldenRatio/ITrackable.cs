using OpenTK;

namespace UDA2018.GoldenRatio
{
    public interface ITrackable
    {
        TrackInfo TrackInfo { get; }
        CallbackTrackFinish TrackFinish();
    }
}
