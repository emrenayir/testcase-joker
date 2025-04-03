/// <summary>
/// This interface is used to provide the outcome of the roulette wheel.
/// This would make easier to switch between different outcome providers.
/// </summary>
public interface IRouletteOutcomeProvider
{
    void SetSelectedNumber(int number);
    int GetTargetNumber();
}