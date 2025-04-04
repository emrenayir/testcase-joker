
/// <summary>
/// A helper class for the chip system.
/// </summary>
public static class ChipHelper
{
    /// <summary>
    /// Contains possible chip values in the game.
    /// </summary>
    public enum ChipValue
    {
        One = 1,
        Five = 5,
        TwentyFive = 25,
        Hundred = 100,
        FiveHundred = 500,
        Thousand = 1000
    }

    /// <summary>
    /// Returns the value of the chip.
    /// </summary>
    public static int GetChipValue(ChipValue chipValue)
    {
        return (int)chipValue;
    }


}