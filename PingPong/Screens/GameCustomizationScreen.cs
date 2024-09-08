using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        GameEntity player1Panel = new GameEntity();
        GameEntity player2Panel = new GameEntity();
        GameEntity player1PaddlePreview = new GameEntity();
        GameEntity player2PaddlePreview = new GameEntity();

        List<Color> colors = new List<Color>
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

        private string player1Instructions = "(A)     Left\n\n(D)     Right\n\n(Spacebar)     Start game\n\n(Esc or Select)     Quit";
        private string player2Instructions = "(Left)     Left\n\n(Right)     Right\n\n(Enter)     Start game\n\n(Delete or Select)     Quit";

        private GameEntity instuction1Text = new GameEntity();
        private GameEntity instuction2Text = new GameEntity();


        int player1ColorIndex = 0;
        int player2ColorIndex = 1;

        //Dirty path for a bug -> This screen seems to be getting the action event (Enter or spacebar press) from the previous screen
        float keyActionCooldown = 0;

        private PongGameStartOptions _pongGameStartOptions = new PongGameStartOptions();

        INavigationManager _navigationManager;

        public GameCustomizationScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            ScreenSize = (graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // Create the paddles side by side according to screen size
            player1Panel.Position = new Vector2(0, 0);
            player1Panel.Texture = SimpleSprite.PaddleTexture.CreatePaddleTexture(graphicsDevice, Color.White,
                ScreenSize.Item1 / 2, ScreenSize.Item2);

            player2Panel.Position = new Vector2(ScreenSize.Item1 / 2, 0);
            player2Panel.Texture = SimpleSprite.PaddleTexture.CreatePaddleTexture(graphicsDevice, Color.White, ScreenSize.Item1 / 2, ScreenSize.Item2);

            player1PaddlePreview = new Paddle(graphicsDevice, colors[player1ColorIndex], 200, 30);
            player2PaddlePreview = new Paddle(graphicsDevice, colors[player2ColorIndex], 200, 30);

            // Draw the paddles to the middle of the panels
            player1PaddlePreview.Position = new Vector2(player1Panel.Position.X + player1Panel.Texture.Width / 2 - player1PaddlePreview.Texture.Width / 2,
                               player1Panel.Position.Y + player1Panel.Texture.Height / 2 - player1PaddlePreview.Texture.Height / 2);

            
            player2PaddlePreview.Position = new Vector2(player2Panel.Position.X + player2Panel.Texture.Width / 2 - player2PaddlePreview.Texture.Width / 2,
                player1Panel.Position.Y + player2Panel.Texture.Height / 2 - player2PaddlePreview.Texture.Height / 2);

            // Add instruction text to top middle of panels
            instuction1Text.Position = new Vector2(player1Panel.Position.X + player1Panel.Texture.Width / 2 - 200,
                               player1Panel.Position.Y + 40);

            instuction2Text.Position = new Vector2(player2Panel.Position.X + player2Panel.Texture.Width / 2 - 200,
                               player2Panel.Position.Y + 40);

            GameEntities = new List<IGameEntity>
            {
                player1Panel,
                player2Panel,
                player1PaddlePreview,
                player2PaddlePreview
            };


            GameScreenControllerManager = new GameScreenControllerManager(400);
        }

        SpriteFont gameFont;
        public void Initialize(ContentManager contentManager)
        {
            gameFont = contentManager.Load<SpriteFont>("SmallFont");

        }

        public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
        {
            instuction1Text.DrawText(gameTime, spriteBatch, gameFont, player1Instructions, Color.White);
            instuction2Text.DrawText(gameTime, spriteBatch, gameFont, player2Instructions, Color.White);


            GameEntities.ForEach(x => x.Draw(gameTime, spriteBatch, Color.White));
        }

        public void UpdateEntities(GameTime gameTime)
        {
            keyActionCooldown += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            GameScreenControllerManager.Update(gameTime);
            GameEntities.ForEach(x => x.Update(gameTime));

            if (GameScreenControllerManager.PlayerOneKeyLeft())
            {
                player1ColorIndex = player1ColorIndex == 0 ? colors.Count - 1 : player1ColorIndex - 1;
                ((Paddle)player1PaddlePreview).ChangeColor(colors[player1ColorIndex]);
            }

            if (GameScreenControllerManager.PlayerOneKeyRight())
            {
                player1ColorIndex = player1ColorIndex == colors.Count - 1 ? 0 : player1ColorIndex + 1;
                ((Paddle)player1PaddlePreview).ChangeColor(colors[player1ColorIndex]);
            }

            if (GameScreenControllerManager.PlayerTwoKeyLeft())
            {
                player2ColorIndex = player2ColorIndex == 0 ? colors.Count - 1 : player2ColorIndex - 1;
                ((Paddle)player2PaddlePreview).ChangeColor(colors[player2ColorIndex]);
            }

            if (GameScreenControllerManager.PlayerTwoKeyRight())
            {
                player2ColorIndex = player2ColorIndex == colors.Count - 1 ? 0 : player2ColorIndex + 1;
                ((Paddle)player2PaddlePreview).ChangeColor(colors[player2ColorIndex]);
            }

            if (GameScreenControllerManager.AnyPlayerKeyAction() && keyActionCooldown > 100)
            {
                // Reset the game screen controller cooldown period
                keyActionCooldown = 0;

                _navigationManager.NavigateTo(nameof(PongGameScreen), _pongGameStartOptions);
            }

            var keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Delete))
            {
                keyActionCooldown = 0;
            }

        }

        public void OnNavigateTo(INavigationManager navigationManager, dynamic parameters)
        {
            var parms = (GameMode)parameters;

            _navigationManager = navigationManager;

            _pongGameStartOptions.Player1Color = colors[player1ColorIndex];
            _pongGameStartOptions.Player2Color = colors[player2ColorIndex];
            _pongGameStartOptions.GameMode = parms;
        }
    }
}
