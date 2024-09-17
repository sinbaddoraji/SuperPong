using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using PingPong.Enum;
using PingPong.Implementation.Controller;
using PingPong.Implementation.GameEntity;
using PingPong.Implementation.PongGame;
using PingPong.Interface;
using PingPong.Model.PongGame;

namespace PingPong.Screens
{

    internal class GameCustomizationScreen : IGameScreen
    {
        public (int, int) ScreenSize { get; set; }
        public List<IGameEntity> GameEntities { get; set; }
        public IGameScreenControllerManager GameScreenControllerManager { get; set; }

        public Color Player2Color { get; set; } = Color.Red;

        public Color Player1Color { get; set; } = Color.CadetBlue;

        readonly GameEntity _player1Panel = new GameEntity();
        readonly GameEntity _player2Panel = new GameEntity();
        readonly GameEntity _player1PaddlePreview = new GameEntity();
        readonly GameEntity _player2PaddlePreview = new GameEntity();

        readonly List<Color> _colors = new List<Color>
        {
            Color.CadetBlue,
            Color.Red,
            Color.Green,
            Color.Yellow,
            Color.Purple,
            Color.Orange,
            Color.Pink,
            Color.Blue,
            Color.Brown,
            Color.Cyan,
            Color.DarkBlue,
            Color.DarkGreen,
            Color.DarkRed,
            Color.DarkOrange,
            Color.DarkViolet,
            Color.Gray,
            Color.LightBlue,
            Color.LightGreen,
            Color.LightYellow,
            Color.White,
        };

        private readonly string _player1Instructions = "(A)     Left\n\n(D)     Right\n\n(Spacebar)     Start game\n\n(Esc or Select)     Quit";
        private readonly string _player2Instructions = "(Left)     Left\n\n(Right)     Right\n\n(Enter)     Start game\n\n(Delete or Select)     Quit";

        private readonly GameEntity _instuction1Text = new GameEntity();
        private readonly GameEntity _instuction2Text = new GameEntity();


        int _player1ColorIndex = 0;
        int _player2ColorIndex = 1;

        //Dirty path for a bug -> This screen seems to be getting the action event (Enter or spacebar press) from the previous screen
        float _keyActionCooldown = 0;

        private readonly PongGameStartOptions _pongGameStartOptions = new PongGameStartOptions();

        INavigationManager _navigationManager;

        public GameCustomizationScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            ScreenSize = (graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // Create the paddles side by side according to screen size
            _player1Panel.Position = new Vector2(0, 0);
            _player1Panel.Texture = SimpleSprite.PaddleTexture.CreatePaddleTexture(graphicsDevice, Color.White,
                ScreenSize.Item1 / 2, ScreenSize.Item2);

            _player2Panel.Position = new Vector2(ScreenSize.Item1 / 2, 0);
            _player2Panel.Texture = SimpleSprite.PaddleTexture.CreatePaddleTexture(graphicsDevice, Color.White, ScreenSize.Item1 / 2, ScreenSize.Item2);

            World abstratWorld = new World();
            _player1PaddlePreview = new Paddle(graphicsDevice, abstratWorld, _colors[_player1ColorIndex], 200, 30);
            _player2PaddlePreview = new Paddle(graphicsDevice, abstratWorld, _colors[_player2ColorIndex], 200, 30);

            // Draw the paddles to the middle of the panels
            _player1PaddlePreview.Position = new Vector2(_player1Panel.Position.X + _player1Panel.Texture.Width / 2 - _player1PaddlePreview.Texture.Width / 2,
                               _player1Panel.Position.Y + _player1Panel.Texture.Height / 2 - _player1PaddlePreview.Texture.Height / 2);

            
            _player2PaddlePreview.Position = new Vector2(_player2Panel.Position.X + _player2Panel.Texture.Width / 2 - _player2PaddlePreview.Texture.Width / 2,
                _player1Panel.Position.Y + _player2Panel.Texture.Height / 2 - _player2PaddlePreview.Texture.Height / 2);

            // Add instruction text to top middle of panels
            _instuction1Text.Position = new Vector2(_player1Panel.Position.X + _player1Panel.Texture.Width / 2 - 200,
                               _player1Panel.Position.Y + 40);

            _instuction2Text.Position = new Vector2(_player2Panel.Position.X + _player2Panel.Texture.Width / 2 - 200,
                               _player2Panel.Position.Y + 40);

            GameEntities = new List<IGameEntity>
            {
                _player1Panel,
                _player2Panel,
                _player1PaddlePreview,
                _player2PaddlePreview
            };


            GameScreenControllerManager = new GameScreenControllerManager(400);
        }

        SpriteFont _gameFont;
        public void Initialize(ContentManager contentManager)
        {
            _gameFont = contentManager.Load<SpriteFont>("SmallFont");

        }

        public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _instuction1Text.DrawText(gameTime, spriteBatch, _gameFont, _player1Instructions, Color.White);
            _instuction2Text.DrawText(gameTime, spriteBatch, _gameFont, _player2Instructions, Color.White);


            GameEntities.ForEach(x => x.Draw(gameTime, spriteBatch, Color.White));
        }

        public void UpdateEntities(GameTime gameTime)
        {
            _keyActionCooldown += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            GameScreenControllerManager.Update(gameTime);
            GameEntities.ForEach(x => x.Update(gameTime));

            if (GameScreenControllerManager.PlayerOneKeyLeft())
            {
                _player1ColorIndex = _player1ColorIndex == 0 ? _colors.Count - 1 : _player1ColorIndex - 1;
                ((Paddle)_player1PaddlePreview).ChangeColor(_colors[_player1ColorIndex]);
            }

            if (GameScreenControllerManager.PlayerOneKeyRight())
            {
                _player1ColorIndex = _player1ColorIndex == _colors.Count - 1 ? 0 : _player1ColorIndex + 1;
                ((Paddle)_player1PaddlePreview).ChangeColor(_colors[_player1ColorIndex]);
            }

            if (GameScreenControllerManager.PlayerTwoKeyLeft())
            {
                _player2ColorIndex = _player2ColorIndex == 0 ? _colors.Count - 1 : _player2ColorIndex - 1;
                ((Paddle)_player2PaddlePreview).ChangeColor(_colors[_player2ColorIndex]);
            }

            if (GameScreenControllerManager.PlayerTwoKeyRight())
            {
                _player2ColorIndex = _player2ColorIndex == _colors.Count - 1 ? 0 : _player2ColorIndex + 1;
                ((Paddle)_player2PaddlePreview).ChangeColor(_colors[_player2ColorIndex]);
            }

            if (GameScreenControllerManager.AnyPlayerKeyAction() && _keyActionCooldown > 100)
            {
                // Reset the game screen controller cooldown period
                _keyActionCooldown = 0;

                _navigationManager.NavigateTo(nameof(PongGameScreen), _pongGameStartOptions);
            }

            var keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Delete))
            {
                _keyActionCooldown = 0;
            }

        }

        public void OnNavigateTo(INavigationManager navigationManager, dynamic parameters)
        {
            var parms = (GameMode)parameters;

            _navigationManager = navigationManager;

            _pongGameStartOptions.Player1Color = _colors[_player1ColorIndex];
            _pongGameStartOptions.Player2Color = _colors[_player2ColorIndex];
            _pongGameStartOptions.GameMode = parms;
        }
    }
}
