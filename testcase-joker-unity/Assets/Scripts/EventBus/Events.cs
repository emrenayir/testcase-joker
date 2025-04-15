using Game;

public interface IEvent { }

//Game state events
public struct GameStateChangeEvent : IEvent
{
    public GameState NewState;
}

//Event for UI button clicks
public struct BetPlacementConfirmedButtonEvent : IEvent{}

public struct ResetBetButtonEvent : IEvent{}

public struct AddFreeChipsButtonClickedEvent : IEvent{}


//Player money events
public struct OnMoneyChangedEvent : IEvent
{
    public int Money;
}

public struct OnBetChangedEvent : IEvent
{
    public int Bet;
}

public struct OnPaymentChangedEvent : IEvent //I might not need this event
{
    public int Payment;
}

//Data events
public struct UpdateStatsEvent : IEvent
{
    public PlayerStatsData PlayerStatsData;
}
public struct OnTotalSpinsChangedEvent : IEvent{}

public struct OnTotalWinsChangedEvent : IEvent
{
    public int TotalWinsChangeAmount;
}

public struct OnTotalProfitChangedEvent : IEvent
{
    public int ProfitChangeAmount;
}

public struct OnCurrentRoundProfitChangedEvent : IEvent //I might not need this event
{
    public int CurrentRoundProfitChangeAmount;
}








