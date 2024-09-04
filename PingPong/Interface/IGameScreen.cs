using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PingPong.Interface
{
    public interface IGameScreen
    {
        (int,int) ScreenSize { get; set; }

        List<IGameEntity> GameEntities { get; set; }

        void Initialize(ContentManager contentManager);

        void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch);

        void UpdateEntities(GameTime gameTime);

        void NavigateTo(INavigationManager navigationManager, string pageName);

        void OnNavigateTo(INavigationManager navigationManager, dynamic parameters);

        void NavigateBackward(INavigationManager navigationManager);
    }
}
