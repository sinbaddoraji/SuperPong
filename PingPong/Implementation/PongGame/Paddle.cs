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
    internal class Paddle : PongGameEntity
    {
        public int Width { get; }
        public int Height { get; }

        private GraphicsDevice _graphics;

        public Paddle(GraphicsDevice graphics, Color color, int width, int height)
        {
            Width = width;
            Height = height;

            _graphics = graphics;

            // Create paddle texture
            Texture = PaddleTexture.CreatePaddleTexture(graphics, color, width, height);
        }

        public void ChangeColor(Color color)
        {
            Texture = PaddleTexture.CreatePaddleTexture(_graphics, color, Width, Height);
        }

        private float _computerPaddleSpeed = 1000;

        //private void HandleComputerLogic(ref Vector2 p0, ref float cpuReactionTimer, GameTime gameTime)
        //{
        //    cpuReactionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //    if (cpuReactionTimer >= _cpuReactionTime)
        //    {
        //        cpuReactionTimer = 0f;

        //        // Calculate the ball's position when it reaches the CPU's paddle
        //        float t = (p0.Y - _ballPosition.Y) / _ballVelocity.Y;
        //        float x = _ballPosition.X + _ballVelocity.X * t;

        //        // Move the CPU's paddle towards the ball's position
        //        if (x < p0.X + _cpuErrorMargin)
        //        {
        //            p0.X -= _computerPaddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //        }
        //        else if (x > p0.X + _paddleTexture2D.Width * _paddleScale.X - _cpuErrorMargin)
        //        {
        //            p0.X += _computerPaddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //        }
        //    }
        //}
    }
}
