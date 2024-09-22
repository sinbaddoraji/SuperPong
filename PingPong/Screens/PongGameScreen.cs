using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Collision.Shapes;
using PingPong.Enum;
using PingPong.Helpers;
using PingPong.Implementation.Controller;
using PingPong.Implementation.PongGame;
using PingPong.Interface;
using PingPong.Model.PongGame;
using nkast.Aether.Physics2D.Common;

namespace PingPong.Screens
{
    internal class PongGameScreen : IGameScreen
    {
        // Screen dimensions in pixels
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public bool Player1IsCpu { get; set; }
        public bool Player2IsCpu { get; set; }

        private readonly PaddleBallLaunchAimer _paddleBallLaunchAimer;
        private readonly PaddleBallLaunchAimer _paddleBallLaunchAimer2;

        private readonly PongArena _pongArena;
        private readonly Paddle _player1Paddle;
        private readonly Paddle _player2Paddle;
        private Ball _ball { get; set; }

        private bool _isPlayer1STurn = true;

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

        // Unit conversion constants
        private const float UnitToPixel = 100f; // 1 meter = 100 pixels
        private const float PixelToUnit = 1f / UnitToPixel;

        public PongGameScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            _world = new World();
            _world.Gravity = new Vector2(0, 0);


            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;

            int pongAreaWidth = ScreenWidth - 100;
            int pongAreaHeight = ScreenHeight - 100;

            // Initialize the arena
            _pongArena = new PongArena(graphicsDevice, Color.White, pongAreaWidth, pongAreaHeight)
            {
                Position = new Vector2(ScreenWidth / 2f - pongAreaWidth / 2f, ScreenHeight / 2f - pongAreaHeight / 2f)
            };

            // Set the arena rectangle for collision detection
            ArenaRectangle = new Rectangle((int)_pongArena.Position.X, (int)_pongArena.Position.Y, pongAreaWidth, pongAreaHeight);

            // Initialize paddles
            _player1Paddle = new Paddle(graphicsDevice, _world, Player1Color, 200, 30);
            _player1Paddle.IsPlayer1 = true;

            _player2Paddle = new Paddle(graphicsDevice, _world, Player2Color, 200, 30);
            _player2Paddle.IsPlayer1 = false;

            // Initialize the ball
            _ball = new Ball(graphicsDevice, _world, Color.White, 30)
            {
                IsVisible = true
            };

            // Initialize aimers
            _paddleBallLaunchAimer = new PaddleBallLaunchAimer(graphicsDevice, _world, GameScreenControllerManager, Player1Color, 50, 30, true);
            _paddleBallLaunchAimer2 = new PaddleBallLaunchAimer(graphicsDevice, _world, GameScreenControllerManager, Player2Color, 50, 30, false);

            _paddleBallLaunchAimer.IsActive = false;
            _paddleBallLaunchAimer.IsVisible = false;

            _paddleBallLaunchAimer.OnBallLaunch += PaddleBallLaunchAimerOnOnBallLaunch;

            void PaddleBallLaunchAimerOnOnBallLaunch(Vector2 launchDirection)
            {
                // Set the ball position and launch it
                SetBallPosition();
                _ball.IsVisible = true;
                _ball.SetVelocity(launchDirection);

                _paddleBallLaunchAimer.IsActive = false;
                _paddleBallLaunchAimer.IsVisible = false;
                _paddleBallLaunchAimer2.IsActive = false;
                _paddleBallLaunchAimer2.IsActive = false;

                _player1Paddle.IsActive = true;
                _player2Paddle.IsActive = true;

                _isPlayer1STurn = !_isPlayer1STurn;

                // Transition to the playing state
                GameState = GameState.Playing;
            }

            // Initialize game entities list
            GameEntities = new List<IGameEntity>
            {
                _pongArena, _player1Paddle, _player2Paddle, _ball,
                _paddleBallLaunchAimer, _paddleBallLaunchAimer2
            };

            // Initialize physics entities
            InitializePhysicsEntities();
        }

        public void Initialize(ContentManager contentManager)
        {
            _gameFont = contentManager.Load<SpriteFont>("MenuItem");
        }

        public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Update aimer positions
            SetAimerPositions();

            // Draw the game entities
            _pongArena.Draw(gameTime, spriteBatch, Color.White);
            _player1Paddle.Draw(spriteBatch);
            _player2Paddle.Draw(spriteBatch);
            _ball.Draw(spriteBatch);

            _paddleBallLaunchAimer.Draw(gameTime, spriteBatch, Player1Color);
            _paddleBallLaunchAimer2.Draw(gameTime, spriteBatch, Player2Color);

            // Draw player scores
            DrawScores(spriteBatch);
        }

        private void DrawScores(SpriteBatch spriteBatch)
        {
            // Player 1 score
            var player1ScoreText = $"Score: {Player1Score}";
            var player1ScorePosition = new Vector2(ArenaRectangle.X, ArenaRectangle.Y + ArenaRectangle.Height + 5);
            spriteBatch.DrawString(_gameFont, player1ScoreText, player1ScorePosition, Player1Color);

            // Player 2 score
            var player2ScoreText = $"Score: {Player2Score}";
            var player2ScoreSize = _gameFont.MeasureString(player2ScoreText);
            var player2ScorePosition = new Vector2(ArenaRectangle.X + ArenaRectangle.Width - player2ScoreSize.X, ArenaRectangle.Y - player2ScoreSize.Y - 5);
            spriteBatch.DrawString(_gameFont, player2ScoreText, player2ScorePosition, Player2Color);
        }

        public void UpdateEntities(GameTime gameTime)
        {
            // Step the physics world
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _world.Step(deltaTime);

            GameScreenControllerManager.Update(gameTime);

            // Handle game states
            switch (GameState)
            {
                case GameState.Player1WaitingToServe:
                    PreparePlayer1Serve();
                    break;
                case GameState.Player2WaitingToServe:
                    PreparePlayer2Serve();
                    break;
                case GameState.Playing:
                    // Game is in progress
                    break;
                default:
                    break;
            }

            // Update game entities
            _paddleBallLaunchAimer.Update(gameTime);
            _paddleBallLaunchAimer2.Update(gameTime);
            _player1Paddle.Update(gameTime);
            _player2Paddle.Update(gameTime);
            _ball.Update(gameTime);

            // Check for scoring
            CheckForScore();
        }

        private void PreparePlayer1Serve()
        {
            _paddleBallLaunchAimer.IsActive = true;
            _paddleBallLaunchAimer.IsVisible = true;
            _ball.IsVisible = false;
        }

        private void PreparePlayer2Serve()
        {
            _paddleBallLaunchAimer2.IsActive = true;
            _paddleBallLaunchAimer2.IsVisible = true;
            _ball.IsVisible = false;
        }

        private void CheckForScore()
        {
            // If the ball goes beyond the top or bottom of the arena, update scores
            if (_ball.Position.Y < ArenaRectangle.Top)
            {
                // Player 1 scores
                Player1Score++;
                GameState = GameState.Player2WaitingToServe;
                 ResetBallAndPaddles();
            }
            else if (_ball.Position.Y > ArenaRectangle.Bottom)
            {
                // Player 2 scores
                Player2Score++;
                GameState = GameState.Player1WaitingToServe;
                ResetBallAndPaddles();
            }
        }

        private void ResetBallAndPaddles()
        {
            // Reset ball and paddles to their initial positions
            SetPaddlePosition();
            SetBallPosition();

            // Reset the ball with zero velocity
            _ball.Reset(_ball.Position, Vector2.Zero);

            // Optionally reset velocities
             _player1Paddle.ResetVelocity();
             _player2Paddle.ResetVelocity();
        }

        public void OnNavigateTo(INavigationManager navigationManager, dynamic parameters)
        {
            var options = (PongGameStartOptions)parameters;
            var gameMode = options.GameMode;

            if (gameMode == GameMode.PlayerToPlayer)
            {
                Player1IsCpu = false;
                Player2IsCpu = false;
            }
            else if (gameMode == GameMode.PlayerToComputer)
            {
                Player1IsCpu = false;
                Player2IsCpu = true;
            }
            else if (gameMode == GameMode.ComputerToComputer)
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
            // Position paddles at the starting positions
            _player1Paddle.Position = new Vector2(ScreenWidth / 2f, ScreenHeight - 100f);
            _player2Paddle.Position = new Vector2(ScreenWidth / 2f, 70f);

        }

        private void SetAimerPositions()
        {
            // Update aimer positions based on paddle positions
            var paddle1Rect = _player1Paddle.GetRectangle();
            _paddleBallLaunchAimer.Position = new Vector2(paddle1Rect.Center.X, paddle1Rect.Top);

            var paddle2Rect = _player2Paddle.GetRectangle();
            _paddleBallLaunchAimer2.Position = new Vector2(paddle2Rect.Center.X, paddle2Rect.Bottom);
        }

        private void SetBallPosition()
        {
            // Position the ball above or below the paddle based on the player's turn
            if (_isPlayer1STurn)
            {
                var paddleRect = _player1Paddle.GetRectangle();
                _ball.Position = new Vector2(paddleRect.Center.X, paddleRect.Top - _ball.Texture.Height / 2f);
            }
            else
            {
                var paddleRect = _player2Paddle.GetRectangle();
                _ball.Position = new Vector2(paddleRect.Center.X, paddleRect.Bottom + _ball.Texture.Height / 2f);
            }
        }

        private void InitializePhysicsEntities()
        {
            // Set the initial positions of the entities
            SetPaddlePosition();
            SetAimerPositions();
            SetBallPosition();

            // Initialize physics bodies with positions in simulation units
            _ball.InitializePhysics(_ball.Position * PixelToUnit);
            _player1Paddle.InitializePhysics(_player1Paddle.Position * PixelToUnit);
            _player2Paddle.InitializePhysics(_player2Paddle.Position * PixelToUnit);

            // Create arena walls
            CreateWalls();
        }

        private void CreateWalls()
        {
            // Convert positions and sizes to simulation units
            float arenaLeft = ArenaRectangle.Left * PixelToUnit;
            float arenaRight = ArenaRectangle.Right * PixelToUnit;
            float arenaTop = ArenaRectangle.Top * PixelToUnit;
            float arenaBottom = ArenaRectangle.Bottom * PixelToUnit;
            float arenaWidth = ArenaRectangle.Width * PixelToUnit;
            float arenaHeight = ArenaRectangle.Height * PixelToUnit;

            // Create walls around the arena
            // Top wall
            CreateWall(new Vector2(arenaLeft + arenaWidth / 2f, arenaTop), new Vector2(arenaWidth, 0.1f), "TopWall");
            // Bottom wall
            CreateWall(new Vector2(arenaLeft + arenaWidth / 2f, arenaBottom), new Vector2(arenaWidth, 0.1f), "BottomWall");
            // Left wall
            CreateWall(new Vector2(arenaLeft, arenaTop + arenaHeight / 2f), new Vector2(0.1f, arenaHeight), "LeftWall");
            // Right wall
            CreateWall(new Vector2(arenaRight, arenaTop + arenaHeight / 2f), new Vector2(0.1f, arenaHeight), "RightWall");
        }

        private void CreateWall(Vector2 position, Vector2 size, string tag)
        {
            // Create a static body at the simulation position
            var wallBody = _world.CreateBody(position, 0f, BodyType.Static);
            wallBody.Tag = tag;

            // Create rectangle vertices
            Vertices rectangleVertices = PolygonTools.CreateRectangle(size.X / 2f, size.Y / 2f);

            // Create a polygon shape using the rectangle vertices
            var rectangleShape = new PolygonShape(rectangleVertices, 1f); // Density

            // Attach the shape to the body as a fixture
            var fixture = wallBody.CreateFixture(rectangleShape);

            // Set fixture properties
            fixture.Restitution = 1f;
            fixture.Friction = 0f;
        }

    }
}

