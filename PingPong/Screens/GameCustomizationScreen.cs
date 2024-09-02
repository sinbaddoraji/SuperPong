using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PingPong.Inerface;

namespace PingPong.Screens
{

    internal class GameCustomizationScreen : IGameScreen
    {
        public async Task Initialize(ContentManager contentManager)
        {
            throw new NotImplementedException();
        }

        public async Task Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public async Task Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the game customization screen
            // The screen should be split side by side and allow the player to customize the game
            // The player should be able to customize the paddle colour, ball colour and which side the player is on

            // Draw Line in the middle

        }

        public async Task UnloadContent()
        {
            throw new NotImplementedException();
        }
    }
}
