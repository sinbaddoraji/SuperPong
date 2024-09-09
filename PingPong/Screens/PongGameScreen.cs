using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using PingPong.Enum;
using PingPong.Helpers;
using PingPong.Implementation.Controller;
using PingPong.Implementation.PongGame;
using PingPong.Interface;
using PingPong.Model.PongGame;


namespace PingPong.Screens
{
    internal class PongGameScreen : IGameScreen
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public bool Player1IsCpu { get; set; }
        public bool Player2IsCpu { get; set; }

        private readonly PaddleBallLaunchAimer _paddleBallLaunchAimer;
        private readonly PaddleBallLaunchAimer _paddleBallLaunchAimer2;

        private readonly PongArena _pongArena;
        private readonly Paddle _player1Paddle;
        private readonly Paddle _player2Paddle;
        private readonly Ball _ball;

        private readonly bool _isPlayer1STurn = true;

        private Rectangle ArenaRectangle { get; set; }

        public GameState GameState { get; set; } = GameState.Player1WaitingToServe;

        public (int, int) ScreenSize { get; set; }
        public List<IGameEntity> GameEntities { get; set; }

        public Color Player1Color { get; set; } = Color.Green;

        public Color Player2Color { get; set; } = Color.Red;

        public int Player1Score { get; set; } = 0;

        public int Player2Score { get; set; } = 0;

        private SpriteFont _gameFont;

        private readonly World _world;

        public IGameScreenControllerManager GameScreenControllerManager { get; set; } = new GameScreenControllerManager(0);


        public PongGameScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            _world = new World();

            _player1Paddle = new Paddle(graphicsDevice, ref _world, Player1Color, 200, 30);
            _player2Paddle = new Paddle(graphicsDevice, ref _world, Player2Color, 200, 30);
            _ball = new Ball(graphicsDevice, ref _world, Color.White, 30)
            {
                IsVisible = false
            };

            _paddleBallLaunchAimer = new PaddleBallLaunchAimer(graphicsDevice, ref _world, GameScreenControllerManager, Player1Color, 50, 30, true);
            _paddleBallLaunchAimer2 = new PaddleBallLaunchAimer(graphicsDevice, ref _world, GameScreenControllerManager, Player2Color, 50, 30, false);


            _paddleBallLaunchAimer.IsActive = false;
            _paddleBallLaunchAimer.IsVisible = false;

            _paddleBallLaunchAimer.OnBallLaunch += PaddleBallLaunchAimerOnOnBallLaunch;

            void PaddleBallLaunchAimerOnOnBallLaunch(Vector2 launchdirection)
            {
                _ball.IsVisible = true;
                 // ball.Velocity = launchdirection;
                _paddleBallLaunchAimer.IsActive = false;
                _paddleBallLaunchAimer.IsVisible = false;
            }



            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;

            int pongAreaWidth = ScreenWidth - 100;
            int pongAreaHeight = ScreenHeight - 100;

            
            _pongArena = new PongArena(graphicsDevice, Color.White, pongAreaWidth, pongAreaHeight);
            // Align the arena to the center of the screen
            _pongArena.Position = new Vector2(ScreenWidth / 2 - _pongArena.Texture.Width / 2, ScreenHeight / 2 - _pongArena.Texture.Height / 2);

            // Set the arena rectangle for collision detection
            ArenaRectangle = new Rectangle((int)_pongArena.Position.X, (int)_pongArena.Position.Y, pongAreaWidth, pongAreaHeight);

            GameEntities = new List<IGameEntity>
            {
                _pongArena, _player1Paddle, _player2Paddle, _ball, 
                _paddleBallLaunchAimer, _paddleBallLaunchAimer2
            };

            _world = new World();

            InitializePhysicsEntities();

        }

        void IGameScreen.Initialize(ContentManager contentManager)
        {
            _gameFont = contentManager.Load<SpriteFont>("MenuItem");
        }

        public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //
            //GameEntities.ForEach(entity => entity.Draw(gameTime, spriteBatch, Color.White))

            // Update paddleBallLaunchAimer position
            SetAimerPositions();

            // Draw the game entities
            _player1Paddle.Draw(spriteBatch);
            _player2Paddle.Draw(spriteBatch);
            _ball.Draw(spriteBatch);
            _paddleBallLaunchAimer.Draw(gameTime,spriteBatch, Player1Color);
            _paddleBallLaunchAimer2.Draw(gameTime, spriteBatch, Player2Color);
            _pongArena.Draw(gameTime, spriteBatch, Color.White);


            // Draw player 1 score

            //Get size of score text
            var player2ScoreTextSize = _gameFont.MeasureString("Player 1: 0");

            var p2Pos = new Vector2(ArenaRectangle.X + ArenaRectangle.Width - player2ScoreTextSize.X, ArenaRectangle.Y - player2ScoreTextSize.Y - 5);
            spriteBatch.DrawString(_gameFont, $"Score: {Player1Score}", p2Pos, Player2Color);

            // Draw player 2 score
            var p1Pos = new Vector2(ArenaRectangle.X, ArenaRectangle.Y + ArenaRectangle.Height + 5);
            spriteBatch.DrawString(_gameFont, $"Score: {Player2Score}", p1Pos, Player1Color);

        }

        public void UpdateEntities(GameTime gameTime)
        {
            // Step the physics world (progress the physics simulation)
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _world.Step(deltaTime);


            GameScreenControllerManager.Update(gameTime);

            if (GameState == GameState.Player1WaitingToServe)
            {
                _paddleBallLaunchAimer.IsActive = true;
                _paddleBallLaunchAimer.IsVisible = true;
                _ball.IsVisible = false;
            }

            if (GameState == GameState.Player2WaitingToServe)
            {
                _paddleBallLaunchAimer2.IsActive = true;
                _paddleBallLaunchAimer2.IsVisible = true;
                _ball.IsVisible = false;
            }

            // Update the game entities
            _paddleBallLaunchAimer.Update(gameTime);
            _paddleBallLaunchAimer2.Update(gameTime);
            _player1Paddle.Update(gameTime);
            _player2Paddle.Update(gameTime);
            _ball.Update(gameTime);


            // GameEntities.ForEach(entity => entity.Update(gameTime));

        }

        public void OnNavigateTo(INavigationManager navigationManager, dynamic parameters)
        {
            var options = (PongGameStartOptions)parameters;

            var parms = options.GameMode;

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

            Player1Color = options.Player1Color;
            Player2Color = options.Player2Color;

            ResetPaddleProperties();
        }

        private void ResetPaddleProperties()
        {
            _player1Paddle.ChangeColor(Player1Color);
            _player2Paddle.ChangeColor(Player2Color);

            _paddleBallLaunchAimer.ChangeColor(Player1Color);
            _paddleBallLaunchAimer2.ChangeColor(Player2Color);
        }

        private void SetPaddlePosition()
        {
            _player1Paddle.Position = new Vector2(ScreenWidth / 2 - _player1Paddle.Texture.Width / 2, ScreenHeight - 100);
            _player2Paddle.Position = new Vector2(ScreenWidth / 2 - _player2Paddle.Texture.Width / 2, 70);
        }

        private void SetAimerPositions()
        {
            // Draw line above ball
            var paddle1Location = _player1Paddle.GetRectangle();

            var paddle1Width = _player1Paddle.Width;
            _paddleBallLaunchAimer.Position = new Vector2(paddle1Location.X + (paddle1Width / 2), paddle1Location.Y);

            var paddle2Location = _player2Paddle.GetRectangle();
            var paddle2Width = _player2Paddle.Width;
            _paddleBallLaunchAimer2.Position = new Vector2(paddle2Location.X + (paddle2Width / 2), paddle2Location.Y + _player2Paddle.Height);
        }

        private void SetBallPosition()
        {
            // if player 1's turn, draw ball just above player 1's paddle
            _ball.Position = _isPlayer1STurn ?
                new Vector2(ScreenWidth / 2 - _ball.Texture.Width / 2, ScreenHeight - 100 - _ball.Texture.Height) :
                new Vector2(ScreenWidth / 2 - _ball.Texture.Width / 2, 70 + _player2Paddle.Texture.Height);
        }

        
        private void InitializePhysicsEntities()
        {
            // Set the initial positions of the entities
            SetPaddlePosition();
            SetAimerPositions();
            SetBallPosition();

            // Create static bodies for arena walls
            CreateWall(new Vector2(ScreenWidth / 2, ArenaRectangle.Top), new Vector2(ScreenWidth, 10)); // Top wall
            CreateWall(new Vector2(ScreenWidth / 2, ArenaRectangle.Bottom), new Vector2(ScreenWidth, 10)); // Bottom wall
            CreateWall(new Vector2(ArenaRectangle.Left, ScreenHeight / 2), new Vector2(10, ScreenHeight)); // Left wall
            CreateWall(new Vector2(ArenaRectangle.Right, ScreenHeight / 2), new Vector2(10, ScreenHeight)); // Right wall
        }

        private void CreateWall(Vector2 position, Vector2 size)
        {
            var wallBody = _world.CreateRectangle(size.X, size.Y, 1f);
            wallBody.BodyType = BodyType.Static; // Walls don't move
            wallBody.Position = ConvertUnits.ToSimUnits(position); // Position in simulation units
        }


    }
}
