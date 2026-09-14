namespace GPoseStudio;

public static class ZoneBits
{
    public const int Zones = 0b0000_0111;
    public const int MaskCount = 8;
    public const int Masks = 0b0111_1111_1000;
    public const int MaskModeBits = 0b1_1000_0000_0000;
    public const int MaskModeShift = 11;
    public const int MaskBoth = 0, MaskEither = 1, MaskOnlyOne = 2, MaskMinus = 3;

    public static int ZonePart(int bits) => bits & Zones;
    public static int MaskPart(int bits) => bits & Masks;
    public static int MaskMode(int bits) => (bits & MaskModeBits) >> MaskModeShift;
    public static int WithMaskMode(int bits, int mode) => (bits & ~MaskModeBits) | ((mode & 3) << MaskModeShift);

    public static int ToggleZone(int bits, int zoneBit)
    {
        int nb = bits ^ (zoneBit & Zones);
        return (nb & Zones) == 0 ? ((nb & ~Zones) | (zoneBit & Zones)) : nb;
    }

    public static int ToggleMask(int bits, int maskBit) => bits ^ (maskBit & Masks);

    public static int MaskBit(int index) => 8 << index;
}
