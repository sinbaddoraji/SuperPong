using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PingPong.SimpleSprite
{
    /// <summary>
    /// Line pointing to a position in degrees
    /// </summary>
    internal class LineTexture
    {
        public static Texture2D CreateLineTexture(GraphicsDevice graphicsDevice, Color color, int length, int thickness = 3)
        {
            // Define the width and height of the texture
            int width = thickness; // Thickness determines the width of the line
            int height = length;    // Length is the height of the line

            // Create a new texture
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            // Create an array to hold the color data for each pixel in the texture
            Color[] data = new Color[width * height];

            // Fill the color array with the specified color
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color; // Set each pixel to the given color
            }

            // Set the pixel data on the texture
            texture.SetData(data);

            // Return the created texture
            return texture;
        }

    }
}
