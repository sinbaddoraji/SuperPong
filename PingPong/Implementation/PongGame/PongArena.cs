using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PingPong.SimpleSprite;

namespace PingPong.Implementation.PongGame
{
    public class PongArena : GameEntity.GameEntity
    {
        public PongArena(GraphicsDevice graphics, Color color, int width, int height)
        {
            Texture = PaddleTexture.CreatePaddleTexture(graphics, color, width, height);
        }
    }
}

