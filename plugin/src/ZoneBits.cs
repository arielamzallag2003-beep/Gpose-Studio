namespace GPoseStudio;

public static class ZoneBits
{
    public const int Zones = 0b0000_0111;
    public const int Masks = 0b0011_1000;

    public static int ZonePart(int bits) => bits & Zones;
    public static int MaskPart(int bits) => bits & Masks;

    public static int ToggleZone(int bits, int zoneBit)
    {
        int nb = bits ^ (zoneBit & Zones);
        return (nb & Zones) == 0 ? ((nb & ~Zones) | (zoneBit & Zones)) : nb;
    }

    public static int ToggleMask(int bits, int maskBit) => bits ^ (maskBit & Masks);

    public static int MaskBit(int index) => 8 << index;
}
