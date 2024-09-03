using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Implementation.GameEntitiy;
using PingPong.Implementation.GameMenuScreen;
using PingPong.Interface;
using PingPong.Properties;
using PingPong.SimpleSprite;

namespace PingPong.Screens
{
    public class GameMenuScreen : IGameScreen
    {
        private Game _game;
        private GraphicsDeviceManager _graphics;
        private int _screenWidth;
        private int _screenHeight;

        private List<Snowflake> _snowflakes;
        private Random _random;
        private Texture2D _snowflakeTexture;
        private readonly GraphicsDevice _graphicsDevice;

        string titleName = "Super Pong";

        private string optionOne = "Arcade Mode";
        private string optionTwo = "Player vs Player";
        private string optionThree = "Player vs Computer";
        private string optionFour = "Comuter vs Computer";
        private string optionFive = "Exit";

        private VerticalMenu verticalMenu;


        public GameMenuScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _graphicsDevice = graphicsDevice;
            _screenWidth = _graphics.PreferredBackBufferWidth;
            _screenHeight = _graphics.PreferredBackBufferHeight;
        }

        public (int, int) ScreenSize { get; set; }
        public List<IGameEntity> GameEntities { get; set; } = new List<IGameEntity>();

        public void Initialize(ContentManager contentManager)
        {
            _random = new Random();
            _snowflakes = new List<Snowflake>();

            // Load snowflake texture
            _snowflakeTexture = SnowFlakeTexture.CreateSnowflakeTexture(_graphicsDevice, 8, 8);


            // Initialize snowflakes with random positions and speeds
            for (int i = 0; i < 50; i++)
            {
                var position = new Vector2(_random.Next(_screenWidth), _random.Next(_screenHeight));
                var speed = (float)_random.NextDouble() * 50 + 50; // Random speed between 50 and 100
                _snowflakes.Add(
                    new Snowflake(position, speed, _snowflakeTexture)
                    {
                        ParentSize = (_screenWidth, _screenHeight)
                    });
            }


            // Create menu
            verticalMenu = new VerticalMenu(titleName, new List<string> { optionOne, optionTwo, optionThree, optionFour, optionFive }, contentManager.Load<SpriteFont>("MenuItem"), Color.White, Color.Yellow);
            verticalMenu.Position = new Vector2(_screenWidth / 2 - 100, _screenHeight / 2 - 100);
            verticalMenu.Spacing = 50;
            verticalMenu.TitleSpriteFont = contentManager.Load<SpriteFont>("MenuTitleFont");
            verticalMenu.Initalize(_graphicsDevice,contentManager);

            GameEntities.Add(verticalMenu);
        }

        public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameEntities.ForEach(entity => entity.Draw(gameTime, spriteBatch, Color.White));
        }

        public void UpdateEntities(GameTime gameTime)
        {
            GameEntities.ForEach(entity => entity.Update(gameTime));
        }

        
        public void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        public Rectangle GetRectangle()
        {
            throw new NotImplementedException();
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw snowflakes

            foreach (var snowflake in _snowflakes)
            {
                snowflake.Draw(gameTime,spriteBatch);
            }
        }
    }
}
