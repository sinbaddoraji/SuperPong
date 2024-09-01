using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PingPong.SimpleSprite
{
    internal class MenuSelectorTexture
    {
        public static Texture2D CreateMenuSelectorTexture(GraphicsDevice graphicsDevice, int width, int height)
        {
            // Create triangle texture pointing at the right
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            Color[] data = new Color[width * height];

            // Define the triangle pattern

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create a simple triangle-shaped menu selector
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || x == y)
                    {
                        data[y * width + x] = Color.White;
                    }
                    else
                    {
                        data[y * width + x] = Color.Transparent;
                    }
                }
            }


            texture.SetData(data);

            return texture;
        }   
    }
}
