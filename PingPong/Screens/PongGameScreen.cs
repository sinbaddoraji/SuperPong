﻿using System;
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

        float _paddleSpeed = 800f;
        float _ballSpeedIncrease = 1.2f; // Increase in ball speed over time

        private Texture2D _paddleTexture2D;

        public int ScreenWidth;
        public int ScreenHeight;

        private GraphicsDeviceManager _graphics;
        private readonly SpriteBatch _spriteBatch;

        private int _ballDiameter = 20;

        private float _cpu1ReactionTimer = 0f;  // Timer for the reaction delay
        private float _cpu2ReactionTimer = 0.1f; // Time in seconds before CPU reacts

        private float _cpuReactionTime = 0.1f; // Time in seconds before CPU reacts
       
        private float _cpuErrorMargin = 10f;   // Adds a margin of error to make the CPU less perfect

        public bool Player1IsCpu = false;
        public bool Player2IsCpu = false;

        private PaddleBallLaunchAimer _paddleBallLaunchAimer;
        private Paddle player1Paddle;
        private Paddle player2Paddle;
        private Ball ball;

        private bool isPlayer1sTurn = true;

        private Rectangle ArenaRectangle { get; set; }

        public GameState GameState { get; set; } = GameState.Player1WaitingToServe;

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
            if (GameState == GameState.Player1WaitingToServe)
            {
                _paddleBallLaunchAimer.IsActive = true;
                _paddleBallLaunchAimer.IsVisible = true;
                ball.IsVisible = false;
            }

            if (GameState == GameState.Player2WaitingToServe)
            {

            }

            GameEntities.ForEach(entity => entity.Update(gameTime));


            

            
        }

        public void NavigateTo(INavigationManager navigationManager, string pageName)
        {
            throw new NotImplementedException();
        }

        public void OnNavigateTo(INavigationManager navigationManager, dynamic parameters)
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

        public void NavigateBackward(INavigationManager navigationManager)
        {
            
        }

        public async Task Initialize(ContentManager contentManager)
        {
           

            InitializeGame();
        }

        public PongGameScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            //_ballTexture = BallTexture.CreateBallTexture(graphicsDevice, _ballDiameter);
            //_paddleTexture2D = PaddleTexture.CreatePaddleTexture(graphicsDevice, 300, 20);

            player1Paddle = new Paddle(graphicsDevice, Color.Green, 200, 30);
            player2Paddle = new Paddle(graphicsDevice, Color.Red, 200, 30);
            ball = new Ball(graphicsDevice, Color.White, 30);

            _paddleBallLaunchAimer = new PaddleBallLaunchAimer(graphicsDevice, Color.Green, 50, 30);
            _paddleBallLaunchAimer.IsActive = false;
            _paddleBallLaunchAimer.IsVisible = false;

            _paddleBallLaunchAimer.OnBallLaunch += PaddleBallLaunchAimerOnOnBallLaunch;

            void PaddleBallLaunchAimerOnOnBallLaunch(Vector2 launchdirection)
            {
                GameState = GameState.Player2WaitingToServe;
            }


            _graphics = graphics;

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;

            int pongAreaWidth = ScreenWidth - 100;
            int pongAreaHeight = ScreenHeight - 100;

            PongArena pongArena = new PongArena(graphicsDevice, Color.White, pongAreaWidth, pongAreaHeight);
            // Align the arena to the center of the screen
            pongArena.Position = new Vector2(ScreenWidth / 2 - pongArena.Texture.Width / 2, ScreenHeight / 2 - pongArena.Texture.Height / 2);

            // Set the arena rectangle for collision detection
            ArenaRectangle = new Rectangle((int)pongArena.Position.X, (int)pongArena.Position.Y, pongAreaWidth, pongAreaHeight);

            // Add player paddle1 to the bottom middle of the arena
            // Set paddle position to the bottom middle of the arena rectangle


            player1Paddle.Position = new Vector2(ScreenWidth / 2 - player1Paddle.Texture.Width / 2, ScreenHeight - 100);
          
            player2Paddle.Position = new Vector2(ScreenWidth / 2 - player2Paddle.Texture.Width / 2, 70);

            SetAimerPositions();

            // if player 1's turn, draw ball just above player 1's paddle
            ball.Position = isPlayer1sTurn ? 
                new Vector2(ScreenWidth / 2 - ball.Texture.Width / 2, ScreenHeight - 100 - ball.Texture.Height) : 
                new Vector2(ScreenWidth / 2 - ball.Texture.Width / 2, 70 + player2Paddle.Texture.Height);

            GameEntities = new List<IGameEntity>
            {
                pongArena, player1Paddle, player2Paddle, ball,
                _paddleBallLaunchAimer
            };

            InitializeGame();
        }

        private void SetAimerPositions()
        {
            if (isPlayer1sTurn)
            {
                
                // Draw line above ball
                var paddleLocation = player1Paddle.GetRectangle();

                var paddle1Width = player1Paddle.Width;
                _paddleBallLaunchAimer.Position = new Vector2(paddleLocation.X + (paddle1Width/2), paddleLocation.Y);
            }
        }

        private void InitializeGame()
        {
            // Initial positions
            _ballPosition = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            _ballVelocity = new Vector2(150, 150); // Set the ball's initial velocity

            //_paddle1Position = new Vector2(ScreenWidth / 2 - _paddleTexture2D.Width * _paddleScale.X / 2, 50); // Top of the screen
            //_paddle2Position = new Vector2(ScreenWidth / 2 - _paddleTexture2D.Width * _paddleScale.X / 2, ScreenHeight - 70); // Bottom of the screen

            
        }

        public async Task Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(_ballTexture, _ballPosition, null, Color.White, 0f, Vector2.Zero, _ballScale, SpriteEffects.None, 0f);
            // spriteBatch.Draw(_paddleTexture2D, _paddle1Position, null, Color.White, 0f, Vector2.Zero, _paddleScale, SpriteEffects.None, 0f);
            // spriteBatch.Draw(_paddleTexture2D, _paddle2Position, null, Color.White, 0f, Vector2.Zero, _paddleScale, SpriteEffects.None, 0f);
        }

    }
}
