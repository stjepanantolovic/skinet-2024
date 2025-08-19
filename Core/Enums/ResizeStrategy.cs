namespace Core.Enums
{
    public enum ResizeStrategy
    {
        Fit, // keep aspect, fit inside WxH (no crop); final <= WxH
        Pad  // exact WxH with padding (no crop)
    }
}