using UnityEngine;
using UnityEngine.InputSystem;

namespace Abduction.Events
{
    public enum UIEvents { }

    public enum GameEvents 
    { 
        Move, 
        MouseAim, 
        JoystickAim, 
        FireLaser, 
        FireBeam 
    }

    public class InputEvents : MonoBehaviour
    {
        /// <summary>
        /// EventEmitter that dispatches UI specific input events.
        /// </summary>
        public static EventEmitter<UIEvents, InputValue> UI = new EventEmitter<UIEvents, InputValue>();
        /// <summary>
        /// EventEmitter that dispatches Game specific input events.
        /// </summary>
        public static EventEmitter<GameEvents, InputValue> Game = new EventEmitter<GameEvents, InputValue>();

        private void OnMove(InputValue inputValue) => Game.Dispatch(GameEvents.Move, inputValue);
        private void OnMouseAim(InputValue inputValue) => Game.Dispatch(GameEvents.MouseAim, inputValue);
        private void OnJoystickAim(InputValue inputValue) => Game.Dispatch(GameEvents.JoystickAim, inputValue);
        private void OnFireLaser(InputValue inputValue) => Game.Dispatch(GameEvents.FireLaser, inputValue);
        private void OnFireBeam(InputValue inputValue) => Game.Dispatch(GameEvents.FireBeam, inputValue);
    }
}
