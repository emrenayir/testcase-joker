using Audio;
using EventBus;
using UnityEngine;
using User;

namespace Game
{
    /// <summary>
    /// This is the main class that controls the game loop.
    /// It is responsible for the game phases and the logic of the game.
    /// </summary>
    public class GameLoop : MonoBehaviour
    {
        [SerializeField] private ParticleSystem winParticles;

        private GameState currentPhase;

        void Awake()
        {
            EventManager.Instance.RegisterEvent<BetPlacementConfirmedButtonEvent>(OnBetPlacementConfirmed);
            EventManager.Instance.RegisterEvent<BetProcessingFinishedEvent>(OnBetProcessingFinished);
        }

        void Start()
        {
            var playerSave = PlayerSave.Instance; //TODO: this is probably not a good way to do this

            SetPhase(GameState.InBet);
        }

        private void SetPhase(GameState newPhase, bool isWinner = false)
        {
            currentPhase = newPhase;

            // Raise event for state change
            EventManager.Instance.Raise(new GameStateChangeEvent { NewState = newPhase });

            switch (currentPhase)
            {
                case GameState.Running:
                    EnterRunningPhase();
                    break;
                case GameState.Finish:
                    EnterFinishPhase(isWinner);
                    break;
            }
        }
        private void EnterRunningPhase()
        {
            EventManager.Instance.Raise(new OnTotalSpinsChangedEvent());
        }

        private void EnterFinishPhase(bool isWinner)
        {
            if (isWinner)
            {
                EventManager.Instance.Raise(new OnTotalWinsChangedEvent { TotalWinsChangeAmount = 1 });
                winParticles.Play();
                SoundManager.Instance.PlaySound("Win");
            }
            else
            {
                SoundManager.Instance.PlaySound("Error");
            }
        
            SetPhase(GameState.InBet);
        }

        private void OnBetProcessingFinished(BetProcessingFinishedEvent @event)
        {
            SetPhase(GameState.Finish, @event.IsWinner);
        }
        private void OnBetPlacementConfirmed()
        {
            // Player has confirmed their chip selection, move to next phase
            SetPhase(GameState.Running);
        }
    }
}