using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Enum;
using PingPong.Implementation.Navigation;
using PingPong.Interface;
using PingPong.Screens;

namespace PingPong
{
    public class PongGame : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;


        private IGameScreen _mainMenuScreen;
        private IGameScreen _pongGameScreen;
        private INavigationManager _navigationManager;

        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            _navigationManager = new NavigationManager();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize screens
            _mainMenuScreen = new GameMenuScreen(GraphicsDevice, _graphics);
            //_mainMenyScreen.ScreenSize = (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            _pongGameScreen = new PongGameScreen(GraphicsDevice, _graphics);

            _navigationManager.RegisterScreen(nameof(_mainMenuScreen), _mainMenuScreen);
            _navigationManager.RegisterScreen(nameof(_pongGameScreen), _pongGameScreen);

            // Subscribe to event
            ((GameMenuScreen)_mainMenuScreen).OnMenuOptionSelected += selectedOption =>
            {
                _navigationManager.NavigateTo(nameof(_pongGameScreen), (GameMode)selectedOption);
            };

            // Initialize screens
            _mainMenuScreen.Initialize(Content);
            _pongGameScreen.Initialize(Content);
        }

       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _navigationManager.CurrentScreen.UpdateEntities(gameTime);
            
            base.Update(gameTime);
        }

       

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _navigationManager.CurrentScreen.DrawEntities(gameTime, _spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        

    }
}
