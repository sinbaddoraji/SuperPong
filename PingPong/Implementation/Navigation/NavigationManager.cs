using System;
using System.Collections.Generic;
using PingPong.Interface;

namespace PingPong.Implementation.Navigation
{
    internal class NavigationManager : INavigationManager
    {
        // Stack to track navigation history for back navigation.
        private Stack<IGameScreen> _backwardStack = new Stack<IGameScreen>();
        // Stack to track forward navigation.
        private Stack<IGameScreen> _forwardStack = new Stack<IGameScreen>();
        // Dictionary to store registered screens by their names.
        private Dictionary<string, IGameScreen> _screens = new Dictionary<string, IGameScreen>();

        // Property for the current screen.
        public IGameScreen CurrentScreen { get; private set; }

        // Register a screen with a name.
        public void RegisterScreen(string screenName, IGameScreen screen)
        {
            if (string.IsNullOrEmpty(screenName) || screen == null)
            {
                throw new ArgumentException("Screen name or screen cannot be null");
            }

            _screens.TryAdd(screenName, screen);

            // Set first screen to be current screen
            if (_screens.Count == 1)
            {
                CurrentScreen = screen;
            }
        }

        // Navigate to a specific screen by its name and pass parameters if needed.
        public void NavigateTo(string screenName, dynamic parameters = null)
        {
            if (!_screens.ContainsKey(screenName))
            {
                throw new ArgumentException($"Screen {screenName} is not registered.");
            }

            var screen = _screens[screenName];

            if (CurrentScreen != null)
            {
                _backwardStack.Push(CurrentScreen);  // Push the current screen to the back stack
            }

            // Clear the forward stack whenever a new navigation occurs
            _forwardStack.Clear();

            CurrentScreen = screen;

            // Pass parameters to the screen
            CurrentScreen.OnNavigateTo(this, parameters);
        }

        // Navigate back to the previous screen.
        public void NavigateBackward()
        {
            if (_backwardStack.Count > 0)
            {
                var previousScreen = _backwardStack.Pop();
                if (CurrentScreen != null)
                {
                    _forwardStack.Push(CurrentScreen);  // Push the current screen to the forward stack
                }
                CurrentScreen = previousScreen;
            }
            else
            {
                //throw new InvalidOperationException("No previous screens to navigate back to.");
            }
        }

        // Navigate forward to the screen that was navigated away from (if available).
        public void NavigateForward()
        {
            if (_forwardStack.Count > 0)
            {
                var nextScreen = _forwardStack.Pop();
                if (CurrentScreen != null)
                {
                    _backwardStack.Push(CurrentScreen);  // Push the current screen to the back stack
                }
                CurrentScreen = nextScreen;
            }
            else
            {
                throw new InvalidOperationException("No forward screens to navigate to.");
            }
        }
    }
}
