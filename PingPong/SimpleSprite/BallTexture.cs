using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PingPong.SimpleSprite;

internal class BallTexture
{
    public static Texture2D CreateBallTexture(GraphicsDevice graphicsDevice, int diameter)
    {
        // Create ball texture
        Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);

        Color[] data = new Color[diameter * diameter];

        // Define the ball pattern
        for (int x = 0; x < diameter; x++)
        {
            for (int y = 0; y < diameter; y++)
            {
                // Create a simple circle-shaped ball
                if (Math.Pow(x - diameter / 2, 2) + Math.Pow(y - diameter / 2, 2) <= Math.Pow(diameter / 2, 2))
                {
                    data[y * diameter + x] = Color.White;
                }
                else
                {
                    data[y * diameter + x] = Color.Transparent;
                }
            }
        }

        texture.SetData(data);

        return texture;
    }
}