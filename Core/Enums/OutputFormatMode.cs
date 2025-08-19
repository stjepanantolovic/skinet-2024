namespace Core.Enums
{
    public enum OutputFormatMode
    {
        Auto,          // choose best automatically
        KeepOriginal,  // re-encode to original format if reasonable
        ForceJpeg,
        ForcePng,
        ForceWebp
    }
}