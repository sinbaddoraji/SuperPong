namespace PingPong.Interface
{
    public interface INavigationManager
    {
        void RegisterScreen(string screenName, IGameScreen screen);

        void NavigateTo(string screenName, dynamic parameters);

        void NavigateBackward();

        void NavigateForward();

        IGameScreen CurrentScreen { get; }
    }
}
