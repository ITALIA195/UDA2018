using System;

namespace GoldenRatio.Graphics
{
    [Flags]
    public enum Side
    {
        Top    = 1 << 0,
        Bottom = 1 << 1,
        Right  = 1 << 2,
        Left   = 1 << 3,
        
        Vertical   = Top | Bottom,
        Horizontal = Left | Right,
    }
}