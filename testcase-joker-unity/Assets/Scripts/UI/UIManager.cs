using EventBus;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This is the main class that controls the UI of the game.
    /// It is responsible for the UI of the game.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button addFreeChipsButton;
        [SerializeField] private Button resetBetButton;

        [SerializeField] private TextMeshProUGUI betAmountText;
        [SerializeField] private TextMeshProUGUI lastRoundEarningsText;
        [SerializeField] private TextMeshProUGUI userMoneyText;


        [SerializeField] private TextMeshProUGUI totalSpinsText;
        [SerializeField] private TextMeshProUGUI totalWinsText;
        [SerializeField] private TextMeshProUGUI totalProfitText;

        private void Awake()
        {
            EventManager.Instance.RegisterEvent<GameStateChangeEvent>(OnGameStateChanged);
            EventManager.Instance.RegisterEvent<UpdateStatsEvent>(UpdateStatsDisplay);
            EventManager.Instance.RegisterEvent<OnMoneyChangedEvent>(OnUserMoneyChanged);
            EventManager.Instance.RegisterEvent<OnBetChangedEvent>(OnBetAmountChanged);
            EventManager.Instance.RegisterEvent<OnPaymentChangedEvent>(OnLastRoundEarningsChanged);
        }

        private void OnGameStateChanged(GameStateChangeEvent @event)
        {
            switch (@event.NewState)
            {
                case GameState.InBet:
                    SetButtonActive(true);
                    SetTotalProfit();
                    break;

                case GameState.Running:
                    SetButtonActive(false);
                    break;

                case GameState.Finish:
                    break;
            }
        }

        private void SetTotalProfit()
        {
            var value = int.Parse(userMoneyText.text) - 1000;

            if (value >= 0)
            {
                totalProfitText.text = value.ToString();
            }
        }

        private void OnLastRoundEarningsChanged(OnPaymentChangedEvent @event)
        {
            lastRoundEarningsText.text = @event.Payment.ToString();
        }

        public void OnAddFreeChipsClicked()
        {
            EventManager.Instance.Raise(new AddFreeChipsButtonClickedEvent());
        }


        private void OnBetAmountChanged(OnBetChangedEvent @event)
        {
            betAmountText.text = @event.Bet.ToString();
        }

        private void OnUserMoneyChanged(OnMoneyChangedEvent @event)
        {
            userMoneyText.text = @event.Money.ToString();
        }

        public void OnBetPlacementConfirmed()
        {
            EventManager.Instance.Raise(new BetPlacementConfirmedButtonEvent());
        }

        public void OnResetBetClicked()
        {
            EventManager.Instance.Raise(new ResetBetButtonEvent());
        }

        // Methods to control button visibility
        public void SetButtonActive(bool isActive)
        {
            if (confirmButton != null)
                confirmButton.gameObject.SetActive(isActive);

            if (resetBetButton != null)
                resetBetButton.gameObject.SetActive(isActive);

            if (addFreeChipsButton != null)
                addFreeChipsButton.gameObject.SetActive(isActive);
        }


        /// <summary>
        /// Update the stats display with the current win/loss tracking information
        /// </summary>
        public void UpdateStatsDisplay(UpdateStatsEvent @event)
        {
            totalSpinsText.text = @event.PlayerStatsData.TotalSpins.ToString();
            totalWinsText.text = @event.PlayerStatsData.TotalWins.ToString();
        }
    }
}