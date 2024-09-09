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
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;


        private IGameScreen MainMenuScreen { get; set; }
        private IGameScreen PongGameScreen { get; set; }
        private IGameScreen GameCustomizationScreen { get; set; }
        private readonly INavigationManager _navigationManager;


        public PongGame()
        {
            Graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size
            Graphics.PreferredBackBufferWidth = 1200;
            Graphics.PreferredBackBufferHeight = 800;
            Graphics.ApplyChanges();

            _navigationManager = new NavigationManager();
        }

        protected override void Initialize()
        {
            base.Initialize();
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize screens
            MainMenuScreen = new GameMenuScreen(GraphicsDevice, Graphics);
            //_mainMenyScreen.ScreenSize = (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            PongGameScreen = new PongGameScreen(GraphicsDevice, Graphics);
            GameCustomizationScreen = new GameCustomizationScreen(GraphicsDevice, Graphics);

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

            SpriteBatch.Begin();

            _navigationManager.CurrentScreen.DrawEntities(gameTime, SpriteBatch);

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        

    }
}
