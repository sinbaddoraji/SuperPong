using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Enum;
using PingPong.Inerface;

namespace PingPong.Screens
{
    internal class PongGameScreen : IGameScreen
    {

        Texture2D ballTexture;
        Vector2 ballPosition;
        Vector2 ballVelocity;

        Vector2 paddle1Position;
        Vector2 paddle2Position;

        float paddleSpeed = 800f;
        float ballSpeedIncrease = 5f; // Increase in ball speed over time

        private Texture2D _paddleTexture2D;

        public int screenWidth;
        public int screenHeight;

        Vector2 ballScale = new Vector2(1f, 1f); // Scaling factor for the ball
        Vector2 paddleScale = new Vector2(1f, 1f); // Scaling factor for the paddles

        private GraphicsDeviceManager _graphics;
        private readonly SpriteBatch _spriteBatch;

        private int ballDiameter = 20;

        private float cpuReactionTime = 0.1f; // Time in seconds before CPU reacts
        private float cpuReactionTimer = 0f;  // Timer for the reaction delay
        private float cpuErrorMargin = 10f;   // Adds a margin of error to make the CPU less perfect




        public GameMode GameMode { get; set; }

        public async Task Initialize(ContentManager contentManager)
        {
            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;

            InitializeGame();
        }

        public PongGameScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            ballTexture = BallTexture.CreateBallTexture(graphicsDevice, ballDiameter);
            _paddleTexture2D = PaddleTexture.CreatePaddleTexture(graphicsDevice, 300, 20);


            _graphics = graphics;

            InitializeGame();
        }

        private void InitializeGame()
        {
            // Initial positions
            ballPosition = new Vector2(screenWidth / 2, screenHeight / 2);
            ballVelocity = new Vector2(150, 150); // Set the ball's initial velocity

            paddle1Position = new Vector2(screenWidth / 2 - _paddleTexture2D.Width * paddleScale.X / 2, 50); // Top of the screen
            paddle2Position = new Vector2(screenWidth / 2 - _paddleTexture2D.Width * paddleScale.X / 2, screenHeight - 70); // Bottom of the screen
        }



       

        public async Task Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kstate = Keyboard.GetState();

            // Player 1 controls (A and D keys for left and right movement)
            if (GameMode == GameMode.PlayerToComputer)
            {
                
                HandleComputerLogic(ref paddle1Position, gameTime);
            }
            else
            {
                if (kstate.IsKeyDown(Keys.A))
                {
                    paddle1Position.X -= paddleSpeed * deltaTime;
                }

                if (kstate.IsKeyDown(Keys.D))
                {
                    paddle1Position.X += paddleSpeed * deltaTime;
                }
            }

            // Player 2 controls (Left and Right keys for left and right movement)
            if (kstate.IsKeyDown(Keys.Left))
            {
                paddle2Position.X -= paddleSpeed * deltaTime;
            }
            if (kstate.IsKeyDown(Keys.Right))
            {
                paddle2Position.X += paddleSpeed * deltaTime;
            }


            // Ensure paddles stay within screen bounds
            paddle1Position.X = MathHelper.Clamp(paddle1Position.X, 0, screenWidth - _paddleTexture2D.Width * paddleScale.X);
            paddle2Position.X = MathHelper.Clamp(paddle2Position.X, 0, screenWidth - _paddleTexture2D.Width * paddleScale.X);

            // Ball movement logic
            ballPosition += ballVelocity * deltaTime;

            // Ball collision with left and right of the screen
            if (ballPosition.X <= 0 || ballPosition.X >= screenWidth - ballTexture.Width * ballScale.X)
            {
                ballVelocity.X *= -1; // Reverse X velocity
            }

            // Ball collision with paddles
            if (ballPosition.Y <= paddle1Position.Y + _paddleTexture2D.Height * paddleScale.Y &&
                ballPosition.X + ballTexture.Width * ballScale.X >= paddle1Position.X &&
                ballPosition.X <= paddle1Position.X + _paddleTexture2D.Width * paddleScale.X)
            {
                ballVelocity.Y *= -1; // Reverse Y velocity
                ballPosition.Y = paddle1Position.Y + _paddleTexture2D.Height * paddleScale.Y; // Ensure ball doesn't get stuck
                ballVelocity *= 1 + ballSpeedIncrease * deltaTime; // Speed up the ball
            }

            if (ballPosition.Y + ballTexture.Height * ballScale.Y >= paddle2Position.Y &&
                ballPosition.X + ballTexture.Width * ballScale.X >= paddle2Position.X &&
                ballPosition.X <= paddle2Position.X + _paddleTexture2D.Width * paddleScale.X)
            {
                ballVelocity.Y *= -1; // Reverse Y velocity
                ballPosition.Y = paddle2Position.Y - ballTexture.Height * ballScale.Y; // Ensure ball doesn't get stuck
                ballVelocity *= 1 + ballSpeedIncrease * deltaTime; // Speed up the ball
            }

            // Ball reset if it goes out of bounds (top or bottom)
            if (ballPosition.Y < 0 || ballPosition.Y > screenHeight)
            {
                InitializeGame();
            }

        }

       
        private void HandleComputerLogic(ref Vector2 p0, GameTime gameTime)
        {
            cpuReactionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (cpuReactionTimer >= cpuReactionTime)
            {
                cpuReactionTimer = 0f;

                // Calculate the ball's position when it reaches the CPU's paddle
                float t = (p0.Y - ballPosition.Y) / ballVelocity.Y;
                float x = ballPosition.X + ballVelocity.X * t;

                // Move the CPU's paddle towards the ball's position
                if (x < p0.X + cpuErrorMargin)
                {
                    p0.X -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (x > p0.X + _paddleTexture2D.Width * paddleScale.X - cpuErrorMargin)
                {
                    p0.X += paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public async Task Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, 0f, Vector2.Zero, ballScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_paddleTexture2D, paddle1Position, null, Color.White, 0f, Vector2.Zero, paddleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_paddleTexture2D, paddle2Position, null, Color.White, 0f, Vector2.Zero, paddleScale, SpriteEffects.None, 0f);
        }

        public async Task UnloadContent()
        {
            throw new NotImplementedException();
        }
    }
}
