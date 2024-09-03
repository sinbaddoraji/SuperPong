using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Enum;
using PingPong.Interface;
using PingPong.Screens;

namespace PingPong
{
    public class PongGame : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;


        private IGameScreen _mainMenyScreen;
        private PongGameScreen _pongGameScreen;

        private GameState _currentGameState = GameState.MainMenu;

        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize screens
            _mainMenyScreen = new GameMenuScreen(GraphicsDevice, _graphics);
            _pongGameScreen = new PongGameScreen(GraphicsDevice, _graphics);

            // Subscribe to event
            ((GameMenuScreen)_mainMenyScreen).OnMenuOptionSelected += selectedOption =>
            {
                _currentGameState = GameState.Playing;
               _pongGameScreen.GameMode =  (GameMode)selectedOption;

               if ((GameMode)selectedOption == GameMode.PlayerToPlayer)
               {
                   _pongGameScreen.player1IsCPU = false;
                   _pongGameScreen.player2IsCPU = false;
               }
               else if ((GameMode)selectedOption == GameMode.PlayerToComputer)
               {
                   _pongGameScreen.player1IsCPU = true;
                   _pongGameScreen.player2IsCPU = false;
               }
               else if ((GameMode)selectedOption == GameMode.ComputerToComputer)
               {
                   _pongGameScreen.player1IsCPU = true;
                   _pongGameScreen.player2IsCPU = true;
               }
            };

            // Initialize screens
            _mainMenyScreen.Initialize(Content);
            _pongGameScreen.Initialize(Content);
        }

       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (_currentGameState)
            {
                case GameState.MainMenu:
                    _mainMenyScreen.Update(gameTime);
                    break;
                case GameState.Playing:
                    _pongGameScreen.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

       

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (_currentGameState == GameState.MainMenu)
            {
                _mainMenyScreen.Draw(gameTime, _spriteBatch);
            }
            else if (_currentGameState == GameState.Playing)
            {
                _pongGameScreen.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        

    }
}
