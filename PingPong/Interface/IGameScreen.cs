using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace PingPong.Interface
{
    internal interface IGameScreen
    {
        (int,int) ScreenSize { get; set; }

        void Initialize(ContentManager contentManager);

        void DrawEntities(List<IGameEntity> gameEntities);

        void UpdateEntities(List<IGameEntity> gameEntities);

        void UnloadContent();
    }
}
