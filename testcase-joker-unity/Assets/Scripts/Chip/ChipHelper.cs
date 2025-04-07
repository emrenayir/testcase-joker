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
    
    /// <summary>
    /// Returns the ChipValue enum from the integer value
    /// </summary>
    public static ChipValue GetChipTypeFromValue(int value)
    {
        switch (value)
        {
            case 1:
                return ChipValue.One;
            case 5:
                return ChipValue.Five;
            case 25:
                return ChipValue.TwentyFive;
            case 100:
                return ChipValue.Hundred;
            case 500:
                return ChipValue.FiveHundred;
            case 1000:
                return ChipValue.Thousand;
            default:
                // Default to the smallest chip if value doesn't match
                UnityEngine.Debug.LogWarning($"No ChipValue found for {value}, defaulting to One");
                return ChipValue.One;
        }
    }
}