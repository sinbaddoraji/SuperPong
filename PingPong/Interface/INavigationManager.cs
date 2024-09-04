using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Interface
{
    internal interface INavigationManager
    {
        void RegisterScreen(string screenName, IGameScreen screen);

        void NavigateTo(string screenName, dynamic parameters);

        void NavigateBackward();

        void NavigateForward();

        IGameScreen CurrentScreen { get; }
    }
}
