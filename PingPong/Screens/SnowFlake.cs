using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Screens
{
    public class Snowflake
    {
        private readonly Random _random;
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public Texture2D Texture { get; set; }

        public Snowflake(Vector2 position, float speed, Texture2D texture)
        {
            Position = position;
            Speed = speed;
            Texture = texture;
            _random = new Random();
        }

        public void Update(GameTime gameTime, int screenWidth, int screenHeight)
        {
            // Move snowflake diagonally from top right to bottom left
            Position += new Vector2(-Speed, Speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Respawn the snowflake at a random position at the top or right edge if it moves off the screen
            if (Position.Y > screenHeight || Position.X < 0)
            {
                Position = new Vector2(screenWidth + _random.Next(0, screenWidth), -_random.Next(0, screenHeight));
                Speed = (float)_random.NextDouble() * 50 + 50; // Assign a new random speed
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }

}
