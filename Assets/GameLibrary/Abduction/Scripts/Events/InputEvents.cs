using UnityEngine;
using UnityEngine.InputSystem;

namespace Abduction.Events
{
    public enum UIInputEvents { }

    public enum GameInputEvents 
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
        public static EventEmitter<UIInputEvents, InputValue> UI = new EventEmitter<UIInputEvents, InputValue>();
        /// <summary>
        /// EventEmitter that dispatches Game specific input events.
        /// </summary>
        public static EventEmitter<GameInputEvents, InputValue> Game = new EventEmitter<GameInputEvents, InputValue>();

        private void OnMove(InputValue inputValue) => Game.Dispatch(GameInputEvents.Move, inputValue);
        private void OnMouseAim(InputValue inputValue) => Game.Dispatch(GameInputEvents.MouseAim, inputValue);
        private void OnJoystickAim(InputValue inputValue) => Game.Dispatch(GameInputEvents.JoystickAim, inputValue);
        private void OnFireLaser(InputValue inputValue) => Game.Dispatch(GameInputEvents.FireLaser, inputValue);
        private void OnFireBeam(InputValue inputValue) => Game.Dispatch(GameInputEvents.FireBeam, inputValue);
    }
}
