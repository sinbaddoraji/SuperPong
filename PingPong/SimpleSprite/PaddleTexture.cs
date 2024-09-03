using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PingPong.SimpleSprite
{
    internal class PaddleTexture
    {
        public static Texture2D CreatePaddleTexture(GraphicsDevice graphicsDevice, int width, int height)
        {
            // Create paddle texture
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            Color[] data = new Color[width * height];

            // Define the paddle pattern
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create a simple rectangle-shaped paddle
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
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
