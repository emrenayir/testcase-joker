using UnityEngine;

namespace Roulette
{
    /// <summary>
    /// This script is responsible for managing the slots on the roulette wheel.
    /// It is responsible for storing the slot number and the position of the slot on the wheel.
    /// </summary>
    public class RouletteSlot : MonoBehaviour
    {
        [SerializeField] private int slotNumber;
    
        public int SlotNumber => slotNumber;
    }
}
