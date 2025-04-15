using System.Collections;
using Audio;
using UnityEngine;

namespace Chips
{
    /// <summary>
    /// A button that represents a chip value for betting phase.
    /// it has a model that is a 3D chip.
    /// When clicked, it will be animated to flip and rise.
    /// Created by ChipButtonFactory.
    /// Connected to ChipSelectionController.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ChipSelectionButton : MonoBehaviour
    {
        /// <summary>
        /// The value of the chip button.
        /// </summary>
        public ChipValue Value { get; private set; }
    
        [Header("Animation Settings")]
        [SerializeField] private float riseHeight = 0.3f;
        [SerializeField] private float flipDuration = 0.5f;

        [SerializeField] private GameObject selectionIndicator;

        private bool isAnimating;
        private ChipSelectionController chipSelectionController;
        private GameObject chipModel;

        /// <summary>
        /// Initializes the ChipButton with necessary dependencies.
        /// </summary>
        /// <param name="value">The value of the button.</param>
        /// <param name="a_chipSelectionController">The controller that handles the chip selection.</param>
        /// <param name="a_chipModel">The model of the chip.</param>
        public void Initialize(ChipValue value, ChipSelectionController a_chipSelectionController, GameObject a_chipModel)
        {
            Value = value;
            chipSelectionController = a_chipSelectionController;
            chipModel = a_chipModel;
        }

        /// <summary>
        /// Called when the button is clicked.
        /// Notifies the ChipSelectionController that the button was clicked.
        /// Starts the flip animation if not already animating.
        /// </summary>
        public void OnMouseDown()
        {
            chipSelectionController.ChipButtonClick(this);
            if (isAnimating) return;
            StartCoroutine(FlipAnimation());
            SoundManager.Instance.PlaySound("Chip");
        }

        private IEnumerator FlipAnimation()
        {
            isAnimating = true;
            Vector3 startPosition = chipModel.transform.localPosition;
            Vector3 topPosition = startPosition + new Vector3(0, riseHeight, 0);
            Quaternion startRotation = chipModel.transform.localRotation;

            // First half - rise and rotate 180 degrees
            float elapsed = 0f;
            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flipDuration;

                // Using easing function
                float easedT = EaseInOutQuad(t);

                chipModel.transform.localPosition = Vector3.Lerp(startPosition, topPosition, easedT);
                chipModel.transform.localRotation = startRotation * Quaternion.Euler(easedT * 180f, 0, 0);

                yield return null;
            }

            // Second half - fall and rotate another 180 degrees
            elapsed = 0f;
            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flipDuration;

                // Using easing function
                float easedT = EaseInOutQuad(t);

                chipModel.transform.localPosition = Vector3.Lerp(topPosition, startPosition, easedT);
                chipModel.transform.localRotation = startRotation * Quaternion.Euler(180f + easedT * 180f, 0, 0);

                yield return null;
            }

            chipModel.transform.localPosition = startPosition;
            chipModel.transform.localRotation = startRotation;

            isAnimating = false;
        }

        /// <summary>
        /// Easing function for smooth animation transitions.
        /// </summary>
        /// <param name="t">Time value between 0 and 1.</param>
        /// <returns>Eased value.</returns>
        private float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }


        public void SetSelected(bool selected)
        {
            selectionIndicator.SetActive(selected);
        }
    }
}

