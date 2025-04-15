using System.Collections.Generic;
using Bet;
using Game;
using User;

namespace EventBus
{
    public interface IEvent { }

    //Game state events
    public struct GameStateChangeEvent : IEvent
    {
        public GameState NewState;
    }

    public struct RouletteFinishedEvent : IEvent
    {
        public int WinningNumber;
    }

    //Bet events
    public struct BetProcessingFinishedEvent : IEvent
    {
        public bool IsWinner;
    }

    public struct SaveBetsEvent : IEvent
    {
        public List<BetButton> ActiveBets;
    }

    public struct LoadSavedBetsEvent : IEvent
    {
        public List<BetData> SavedBets;
    }

    public struct PlaceBetEvent : IEvent
    {
        public int ChipValue;
    }

    public struct ProcessPaymentEvent : IEvent
    {
        public int Payment;
        public int LostBets;
    }

    public struct ClearSavedBetsEvent : IEvent {}

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

    public struct OnPaymentChangedEvent : IEvent 
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

    public struct OnCurrentRoundProfitChangedEvent : IEvent {}
}