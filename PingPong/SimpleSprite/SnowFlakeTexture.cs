using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PingPong.SimpleSprite
{
    internal class SnowFlakeTexture
    {
        public static Texture2D CreateSnowflakeTexture(GraphicsDevice graphicsDevice, int width, int height)
        {
            // Create a new texture
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            // Create a 2D array to hold the pixel data
            Color[] data = new Color[width * height];

            // Define the snowflake pattern (for simplicity, this will be a small cross)
            int centerX = width / 2;
            int centerY = height / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create a simple cross-shaped snowflake
                    if (x == centerX || y == centerY)
                    {
                        data[y * width + x] = Color.White;
                    }
                    else
                    {
                        data[y * width + x] = Color.Transparent;
                    }
                }
            }

            // Set the pixel data on the texture
            texture.SetData(data);

            return texture;
        }
    }
}
