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


        private IGameScreen MainMenuScreen { get; set; }
        private IGameScreen PongGameScreen { get; set; }
        private IGameScreen GameCustomizationScreen { get; set; }
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
            MainMenuScreen = new GameMenuScreen(GraphicsDevice, _graphics);
            //_mainMenyScreen.ScreenSize = (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            PongGameScreen = new PongGameScreen(GraphicsDevice, _graphics);
            GameCustomizationScreen = new GameCustomizationScreen(GraphicsDevice, _graphics);

            _navigationManager.RegisterScreen(nameof(MainMenuScreen), MainMenuScreen);
            _navigationManager.RegisterScreen(nameof(PongGameScreen), PongGameScreen);
            _navigationManager.RegisterScreen(nameof(GameCustomizationScreen), GameCustomizationScreen);


            // Subscribe to event
            ((GameMenuScreen)MainMenuScreen).OnMenuOptionSelected += selectedOption =>
            {
                if (((GameMode)selectedOption) == GameMode.Exit)
                {
                    Exit();
                }
                //_navigationManager.NavigateTo(nameof(_pongGameScreen), (GameMode)selectedOption);
                _navigationManager.NavigateTo(nameof(GameCustomizationScreen), (GameMode)selectedOption);
            };

            // Initialize screens
            MainMenuScreen.Initialize(Content);
            GameCustomizationScreen.Initialize(Content);
            PongGameScreen.Initialize(Content);
            
        }

       
        protected override void Update(GameTime gameTime)
        {
            _navigationManager.CurrentScreen.UpdateEntities(gameTime);

            var keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Delete))
                _navigationManager.NavigateBackward();

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
