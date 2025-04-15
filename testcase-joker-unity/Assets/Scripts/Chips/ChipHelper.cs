namespace Chips
{
    /// <summary>
    /// A helper class for the chip system.
    /// </summary>
    public static class ChipHelper
    {

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
                    // Throw exception when value doesn't match any chip type
                    throw new System.ArgumentException($"No ChipValue found for {value}");
            }
        }
    }
}