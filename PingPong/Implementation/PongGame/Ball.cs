using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PingPong.SimpleSprite;

namespace PingPong.Implementation.PongGame;

public class Ball : PongGameEntity
{
    public Ball(GraphicsDevice graphics, Color color, int diameter)
    {
        // Create ball texture
        Texture = BallTexture.CreateBallTexture(graphics, color, diameter);
    }
}