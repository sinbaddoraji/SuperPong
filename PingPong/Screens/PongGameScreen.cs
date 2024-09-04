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
using PingPong.Implementation.PongGame;
using PingPong.Interface;
using PingPong.SimpleSprite;

namespace PingPong.Screens
{
    internal class PongGameScreen : IGameScreen
    {

        Texture2D _ballTexture;
        Vector2 _ballPosition;
        Vector2 _ballVelocity;

        Vector2 _paddle1Position;
        Vector2 _paddle2Position;

        float _paddleSpeed = 800f;
        float _ballSpeedIncrease = 1.2f; // Increase in ball speed over time

        private Texture2D _paddleTexture2D;

        public int ScreenWidth;
        public int ScreenHeight;

        Vector2 _ballScale = new Vector2(1f, 1f); // Scaling factor for the ball
        Vector2 _paddleScale = new Vector2(1f, 1f); // Scaling factor for the paddles

        private GraphicsDeviceManager _graphics;
        private readonly SpriteBatch _spriteBatch;

        private int _ballDiameter = 20;

        private float _cpu1ReactionTimer = 0f;  // Timer for the reaction delay
        private float _cpu2ReactionTimer = 0.1f; // Time in seconds before CPU reacts

        private float _cpuReactionTime = 0.1f; // Time in seconds before CPU reacts
       
        private float _cpuErrorMargin = 10f;   // Adds a margin of error to make the CPU less perfect

        public bool Player1IsCpu = false;
        public bool Player2IsCpu = false;


        public GameMode GameMode { get; set; }

        public (int, int) ScreenSize { get; set; }
        public List<IGameEntity> GameEntities { get; set; }
        void IGameScreen.Initialize(ContentManager contentManager)
        {
            //throw new NotImplementedException();
        }

        public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameEntities.ForEach(entity => entity.Draw(gameTime, spriteBatch, Color.White));
        }

        public void UpdateEntities(GameTime gameTime)
        {
            GameEntities.ForEach(entity => entity.Update(gameTime));
        }

        public void NavigateTo(string pageName)
        {
            throw new NotImplementedException();
        }

        public void OnNavigateTo(dynamic parameters)
        {
            var parms = (GameMode)parameters;

            if (parms == GameMode.PlayerToPlayer)
            {
                Player1IsCpu = false;
                Player2IsCpu = false;
            }
            else if (parms == GameMode.PlayerToComputer)
            {
                Player1IsCpu = true;
                Player2IsCpu = false;
            }
            else if (parms == GameMode.ComputerToComputer)
            {
                Player1IsCpu = true;
                Player2IsCpu = true;
            }

        }

        public void NavigateBackward()
        {
            throw new NotImplementedException();
        }

        public async Task Initialize(ContentManager contentManager)
        {
           

            InitializeGame();
        }

        public PongGameScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            _ballTexture = BallTexture.CreateBallTexture(graphicsDevice, _ballDiameter);
            _paddleTexture2D = PaddleTexture.CreatePaddleTexture(graphicsDevice, 300, 20);


            _graphics = graphics;

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;

            PongArena pongArena = new PongArena(graphicsDevice, ScreenWidth - 100, ScreenHeight - 100);
            // Align the arena to the center of the screen
            pongArena.Position = new Vector2(ScreenWidth / 2 - pongArena.Texture.Width / 2, ScreenHeight / 2 - pongArena.Texture.Height / 2);

            GameEntities = new List<IGameEntity> { pongArena };

            InitializeGame();
        }

        private void InitializeGame()
        {
            // Initial positions
            _ballPosition = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            _ballVelocity = new Vector2(150, 150); // Set the ball's initial velocity

            _paddle1Position = new Vector2(ScreenWidth / 2 - _paddleTexture2D.Width * _paddleScale.X / 2, 50); // Top of the screen
            _paddle2Position = new Vector2(ScreenWidth / 2 - _paddleTexture2D.Width * _paddleScale.X / 2, ScreenHeight - 70); // Bottom of the screen

            
        }


        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        public Rectangle GetRectangle()
        {
            throw new NotImplementedException();
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            throw new NotImplementedException();
        }


        public async Task Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kstate = Keyboard.GetState();

            // Player 1 controls (A and D keys for left and right movement)
            if (Player1IsCpu)
            {
                
                HandleComputerLogic(ref _paddle1Position, ref _cpu1ReactionTimer, gameTime);
            }
            else
            {
                if (kstate.IsKeyDown(Keys.A))
                {
                    _paddle1Position.X -= _paddleSpeed * deltaTime;
                }

                if (kstate.IsKeyDown(Keys.D))
                {
                    _paddle1Position.X += _paddleSpeed * deltaTime;
                }
            }

            // Player 2 controls (Left and Right keys for left and right movement)
            if (Player2IsCpu)
            {
                HandleComputerLogic(ref _paddle2Position, ref _cpu2ReactionTimer, gameTime);
            }
            else
            {
                if (kstate.IsKeyDown(Keys.Left))
                {
                    _paddle2Position.X -= _paddleSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.Right))
                {
                    _paddle2Position.X += _paddleSpeed * deltaTime;
                }
            }



            // Ensure paddles stay within screen bounds
            _paddle1Position.X = MathHelper.Clamp(_paddle1Position.X, 0, ScreenWidth - _paddleTexture2D.Width * _paddleScale.X);
            _paddle2Position.X = MathHelper.Clamp(_paddle2Position.X, 0, ScreenWidth - _paddleTexture2D.Width * _paddleScale.X);

            // Ball movement logic
            _ballPosition += _ballVelocity * deltaTime;

            // Ball collision with left and right of the screen
            if (_ballPosition.X <= 0 || _ballPosition.X >= ScreenWidth - _ballTexture.Width * _ballScale.X)
            {
                _ballVelocity.X *= -1; // Reverse X velocity
            }

            // Ball collision with paddles
            if (_ballPosition.Y <= _paddle1Position.Y + _paddleTexture2D.Height * _paddleScale.Y &&
                _ballPosition.X + _ballTexture.Width * _ballScale.X >= _paddle1Position.X &&
                _ballPosition.X <= _paddle1Position.X + _paddleTexture2D.Width * _paddleScale.X)
            {
                _ballVelocity.Y *= -1; // Reverse Y velocity
                _ballPosition.Y = _paddle1Position.Y + _paddleTexture2D.Height * _paddleScale.Y; // Ensure ball doesn't get stuck
                _ballVelocity *= 1 + _ballSpeedIncrease * deltaTime; // Speed up the ball
            }

            if (_ballPosition.Y + _ballTexture.Height * _ballScale.Y >= _paddle2Position.Y &&
                _ballPosition.X + _ballTexture.Width * _ballScale.X >= _paddle2Position.X &&
                _ballPosition.X <= _paddle2Position.X + _paddleTexture2D.Width * _paddleScale.X)
            {
                _ballVelocity.Y *= -1; // Reverse Y velocity
                _ballPosition.Y = _paddle2Position.Y - _ballTexture.Height * _ballScale.Y; // Ensure ball doesn't get stuck
                _ballVelocity *= 1 + _ballSpeedIncrease * deltaTime; // Speed up the ball
            }

            // Ball reset if it goes out of bounds (top or bottom)
            if (_ballPosition.Y < 0 || _ballPosition.Y > ScreenHeight)
            {
                InitializeGame();
            }

            // Update paddle speed based on the ball's speed
            _paddleSpeed = _ballVelocity.Length() * 2.5f;
            _computerPaddleSpeed = _paddleSpeed * 2.5f;

        }

        private float _computerPaddleSpeed = 1000;
       
        private void HandleComputerLogic(ref Vector2 p0, ref float cpuReactionTimer, GameTime gameTime)
        {
            cpuReactionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (cpuReactionTimer >= _cpuReactionTime)
            {
                cpuReactionTimer = 0f;

                // Calculate the ball's position when it reaches the CPU's paddle
                float t = (p0.Y - _ballPosition.Y) / _ballVelocity.Y;
                float x = _ballPosition.X + _ballVelocity.X * t;

                // Move the CPU's paddle towards the ball's position
                if (x < p0.X + _cpuErrorMargin)
                {
                    p0.X -= _computerPaddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (x > p0.X + _paddleTexture2D.Width * _paddleScale.X - _cpuErrorMargin)
                {
                    p0.X += _computerPaddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public async Task Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_ballTexture, _ballPosition, null, Color.White, 0f, Vector2.Zero, _ballScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_paddleTexture2D, _paddle1Position, null, Color.White, 0f, Vector2.Zero, _paddleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_paddleTexture2D, _paddle2Position, null, Color.White, 0f, Vector2.Zero, _paddleScale, SpriteEffects.None, 0f);
        }

        public async Task UnloadContent()
        {
            throw new NotImplementedException();
        }
    }
}
