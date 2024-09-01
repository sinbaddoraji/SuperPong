using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PingPong.Inerface
{
    internal interface IGameScreen
    {
        Task Initialize(ContentManager contentManager);

        Task Update(GameTime gameTime);

        Task Draw(GameTime gameTime, SpriteBatch spriteBatch);

        Task UnloadContent();
    }
}
