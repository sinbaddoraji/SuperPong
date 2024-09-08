using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PingPong.Interface;

namespace PingPong.Implementation.Controller
{
    public class GameScreenControllerManager : IGameScreenControllerManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _currentGamePadStatePlayerOne;
        private GamePadState _previousGamePadStatePlayerOne;
        private GamePadState _currentGamePadStatePlayerTwo;
        private GamePadState _previousGamePadStatePlayerTwo;
        private GameTime _gameTime;

        // Cooldown settings
        public double CooldownPeriod { get; set; }
        private readonly Dictionary<Keys, double> _keyCooldowns;
        private readonly Dictionary<Buttons, double> _buttonCooldowns;
        private readonly Dictionary<string, double> _thumbstickCooldowns; // For Thumbstick and D-pad cooldown

        public GameScreenControllerManager(int cooldown = 200)
        {
            CooldownPeriod = cooldown;
            _currentKeyboardState = Keyboard.GetState();
            _keyCooldowns = new Dictionary<Keys, double>();
            _buttonCooldowns = new Dictionary<Buttons, double>();
            _thumbstickCooldowns = new Dictionary<string, double>();
        }

        // Call this in your game's Update method, passing in the GameTime instance
        public bool AnyPlayerKeyAction()
        {
            // Key press: enter, space or A button on gamepad
            return IsKeyPressedWithCooldown(Keys.Enter) ||
                   IsKeyPressedWithCooldown(Keys.Space) ||
                   IsButtonPressedWithCooldown(Buttons.A, _currentGamePadStatePlayerOne, _previousGamePadStatePlayerOne) ||
                   IsButtonPressedWithCooldown(Buttons.A, _currentGamePadStatePlayerTwo, _previousGamePadStatePlayerTwo);
        }

        public void Update(GameTime gameTime)
        {
            // Update keyboard and gamepad states
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousGamePadStatePlayerOne = _currentGamePadStatePlayerOne;
            _currentGamePadStatePlayerOne = GamePad.GetState(PlayerIndex.One);

            _previousGamePadStatePlayerTwo = _currentGamePadStatePlayerTwo;
            _currentGamePadStatePlayerTwo = GamePad.GetState(PlayerIndex.Two);

            _gameTime = gameTime;
        }

        // General Press with Cooldown method for keyboard or gamepad buttons
        public bool PressWithCooldown(Buttons button, PlayerIndex playerIndex)
        {
            GamePadState currentGamePadState = (playerIndex == PlayerIndex.One) ? _currentGamePadStatePlayerOne : _currentGamePadStatePlayerTwo;
            GamePadState previousGamePadState = (playerIndex == PlayerIndex.One) ? _previousGamePadStatePlayerOne : _previousGamePadStatePlayerTwo;

            return IsButtonPressedWithCooldown(button, currentGamePadState, previousGamePadState);
        }

        // Helper function to detect key press with cooldown for keyboard
        private bool IsKeyPressedWithCooldown(Keys key)
        {
            if (_currentKeyboardState.IsKeyDown(key))
            {
                // Check if cooldown is applied
                if (!_keyCooldowns.ContainsKey(key) || _gameTime.TotalGameTime.TotalMilliseconds - _keyCooldowns[key] > CooldownPeriod)
                {
                    _keyCooldowns[key] = _gameTime.TotalGameTime.TotalMilliseconds;
                    return true;
                }
            }
            return false;
        }

        // Helper function to detect button press with cooldown for gamepad buttons
        private bool IsButtonPressedWithCooldown(Buttons button, GamePadState currentGamePadState, GamePadState previousGamePadState)
        {
            if (currentGamePadState.IsButtonDown(button) && !previousGamePadState.IsButtonDown(button))
            {
                // Check if cooldown is applied
                if (!_buttonCooldowns.ContainsKey(button) || _gameTime.TotalGameTime.TotalMilliseconds - _buttonCooldowns[button] > CooldownPeriod)
                {
                    _buttonCooldowns[button] = _gameTime.TotalGameTime.TotalMilliseconds;
                    return true;
                }
            }
            return false;
        }

        // Helper function to detect D-pad and thumbstick movement with cooldown
        private bool IsThumbstickOrDPadWithCooldown(string movementKey, bool isPressed)
        {
            if (isPressed)
            {
                if (!_thumbstickCooldowns.ContainsKey(movementKey) || _gameTime.TotalGameTime.TotalMilliseconds - _thumbstickCooldowns[movementKey] > CooldownPeriod)
                {
                    _thumbstickCooldowns[movementKey] = _gameTime.TotalGameTime.TotalMilliseconds;
                    return true;
                }
            }
            return false;
        }

        // Player 1 Controls (X, Y, A, B, Select, Options, R1, R2, L1, L2)
        public bool PlayerOneButtonX() => PressWithCooldown(Buttons.X, PlayerIndex.One);
        public bool PlayerOneButtonY() => PressWithCooldown(Buttons.Y, PlayerIndex.One);
        public bool PlayerOneButtonA() => PressWithCooldown(Buttons.A, PlayerIndex.One);
        public bool PlayerOneButtonB() => PressWithCooldown(Buttons.B, PlayerIndex.One);
        public bool PlayerOneSelect() => PressWithCooldown(Buttons.Back, PlayerIndex.One);
        public bool PlayerOneOptions() => PressWithCooldown(Buttons.Start, PlayerIndex.One);
        public bool PlayerOneR1() => PressWithCooldown(Buttons.RightShoulder, PlayerIndex.One);
        public bool PlayerOneR2() => PressWithCooldown(Buttons.RightTrigger, PlayerIndex.One);
        public bool PlayerOneL1() => PressWithCooldown(Buttons.LeftShoulder, PlayerIndex.One);
        public bool PlayerOneL2() => PressWithCooldown(Buttons.LeftTrigger, PlayerIndex.One);

        // Player 2 Controls (X, Y, A, B, Select, Options, R1, R2, L1, L2)
        public bool PlayerTwoButtonX() => PressWithCooldown(Buttons.X, PlayerIndex.Two);
        public bool PlayerTwoButtonY() => PressWithCooldown(Buttons.Y, PlayerIndex.Two);
        public bool PlayerTwoButtonA() => PressWithCooldown(Buttons.A, PlayerIndex.Two);
        public bool PlayerTwoButtonB() => PressWithCooldown(Buttons.B, PlayerIndex.Two);
        public bool PlayerTwoSelect() => PressWithCooldown(Buttons.Back, PlayerIndex.Two);
        public bool PlayerTwoOptions() => PressWithCooldown(Buttons.Start, PlayerIndex.Two);
        public bool PlayerTwoR1() => PressWithCooldown(Buttons.RightShoulder, PlayerIndex.Two);
        public bool PlayerTwoR2() => PressWithCooldown(Buttons.RightTrigger, PlayerIndex.Two);
        public bool PlayerTwoL1() => PressWithCooldown(Buttons.LeftShoulder, PlayerIndex.Two);
        public bool PlayerTwoL2() => PressWithCooldown(Buttons.LeftTrigger, PlayerIndex.Two);

        // Thumbstick and D-Pad controls for movement (unchanged from previous implementation)
        public bool PlayerOneKeyUp()
        {
            return IsKeyPressedWithCooldown(Keys.W) ||
                   IsThumbstickOrDPadWithCooldown("P1_Up", _currentGamePadStatePlayerOne.ThumbSticks.Left.Y > 0.5f || _currentGamePadStatePlayerOne.DPad.Up == ButtonState.Pressed);
        }

        public bool PlayerOneKeyDown()
        {
            return IsKeyPressedWithCooldown(Keys.S) ||
                   IsThumbstickOrDPadWithCooldown("P1_Down", _currentGamePadStatePlayerOne.ThumbSticks.Left.Y < -0.5f || _currentGamePadStatePlayerOne.DPad.Down == ButtonState.Pressed);
        }

        public bool PlayerOneKeyLeft()
        {
            return IsKeyPressedWithCooldown(Keys.A) ||
                   IsThumbstickOrDPadWithCooldown("P1_Left", _currentGamePadStatePlayerOne.ThumbSticks.Left.X < -0.5f || _currentGamePadStatePlayerOne.DPad.Left == ButtonState.Pressed);
        }

        public bool PlayerOneKeyRight()
        {

            return IsKeyPressedWithCooldown(Keys.D) ||
                   IsThumbstickOrDPadWithCooldown("P1_Right", _currentGamePadStatePlayerOne.ThumbSticks.Left.X > 0.5f || _currentGamePadStatePlayerOne.DPad.Right == ButtonState.Pressed);
        }

        public bool PlayerOneKeyAction()
        {
            return IsKeyPressedWithCooldown(Keys.Space) ||
                   IsButtonPressedWithCooldown(Buttons.A, _currentGamePadStatePlayerOne, _previousGamePadStatePlayerOne);
        }

        public bool PlayerTwoKeyUp()
        {
            return IsKeyPressedWithCooldown(Keys.Up) ||
                   IsThumbstickOrDPadWithCooldown("P2_Up", _currentGamePadStatePlayerTwo.ThumbSticks.Left.Y > 0.5f || _currentGamePadStatePlayerTwo.DPad.Up == ButtonState.Pressed);
        }

        public bool PlayerTwoKeyDown()
        {
            return IsKeyPressedWithCooldown(Keys.Down) ||
                   IsThumbstickOrDPadWithCooldown("P2_Down", _currentGamePadStatePlayerTwo.ThumbSticks.Left.Y < -0.5f || _currentGamePadStatePlayerTwo.DPad.Down == ButtonState.Pressed);
        }

        public bool PlayerTwoKeyLeft()
        {
            return IsKeyPressedWithCooldown(Keys.Left) ||
                   IsThumbstickOrDPadWithCooldown("P2_Left", _currentGamePadStatePlayerTwo.ThumbSticks.Left.X < -0.5f || _currentGamePadStatePlayerTwo.DPad.Left == ButtonState.Pressed);
        }

        public bool PlayerTwoKeyRight()
        {
            return IsKeyPressedWithCooldown(Keys.Right) ||
                   IsThumbstickOrDPadWithCooldown("P2_Right", _currentGamePadStatePlayerTwo.ThumbSticks.Left.X > 0.5f || _currentGamePadStatePlayerTwo.DPad.Right == ButtonState.Pressed);
        }

        public bool PlayerTwoKeyAction()
        {
            return IsKeyPressedWithCooldown(Keys.Enter) ||
                   IsButtonPressedWithCooldown(Buttons.A, _currentGamePadStatePlayerTwo, _previousGamePadStatePlayerTwo);
        }

        public bool AnyPlayerKeyUp()
        {
            return PlayerOneKeyUp() || PlayerTwoKeyUp();
        }

        public bool AnyPlayerKeyDown()
        {
            return PlayerOneKeyDown() || PlayerTwoKeyDown();
        }

        public bool AnyPlayerKeyLeft()
        {
            return PlayerOneKeyLeft() || PlayerTwoKeyLeft();
        }

        public bool AnyPlayerKeyRight()
        {
            return PlayerOneKeyRight() || PlayerTwoKeyRight();
        }
    }
}
