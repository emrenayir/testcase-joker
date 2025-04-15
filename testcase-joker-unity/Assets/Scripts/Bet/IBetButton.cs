namespace Bet
{
    public interface IBetButton
    {
        void OnClick();
        void ShowWinningStatus(bool isWinning);
        BetType GetBetType();
        float GetPayout();
    }
}
