using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Inerface;
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
        private SpriteFont _menuTitleFont;
        private SpriteFont _menuItemFont;
        private readonly GraphicsDevice _graphicsDevice;
        private Texture2D _menuSelectionTexture;

        Vector2 optionOnePosition, 
            optionTwoPosition, 
            optionThreePosition, 
            _selectorPosition,
            optionOneSize,
            optionTwoSize,
            optionThreeSize,
            titleSize,
            titlePosition;

        string titleName = "Ping Pong";
        string optionOne = "Player vs Player";
        string optionTwo = "Player vs Computer";
        string optionThree = "Computer vs Computer";

        public delegate void MenuOptionSelectedHandler(int selectedOption);
        public event MenuOptionSelectedHandler OnMenuOptionSelected;


        private int _menuSelection = 1;
        private float _inputCooldown = 0.2f; // 200ms cooldown for input
        private float _timeSinceLastInput = 0;

        public GameMenuScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _graphicsDevice = graphicsDevice;
            _screenWidth = _graphics.PreferredBackBufferWidth;
            _screenHeight = _graphics.PreferredBackBufferHeight;
        }

        public Task Initialize(ContentManager contentManager)
        {
            _random = new Random();
            _snowflakes = new List<Snowflake>();

            // Load snowflake texture
            _snowflakeTexture = SnowFlakeTexture.CreateSnowflakeTexture(_graphicsDevice, 8, 8);

            using (FileStream stream = new FileStream("MenuSelector.png", FileMode.Open))
            {
                _menuSelectionTexture = Texture2D.FromStream(_graphicsDevice, stream);
            }

            _menuTitleFont = contentManager.Load<SpriteFont>("MenuTitleFont");
            _menuItemFont = contentManager.Load<SpriteFont>("MenuItem");

            // Initialize snowflakes with random positions and speeds
            for (int i = 0; i < 50; i++)
            {
                var position = new Vector2(_random.Next(_screenWidth), _random.Next(_screenHeight));
                var speed = (float)_random.NextDouble() * 50 + 50; // Random speed between 50 and 100
                _snowflakes.Add(new Snowflake(position, speed, _snowflakeTexture));
            }

            titleSize = _menuTitleFont.MeasureString(titleName); 
            titlePosition = new Vector2(_screenWidth / 2 - titleSize.X / 2, 50);

            optionOneSize = _menuItemFont.MeasureString(optionOne);
            optionTwoSize = _menuItemFont.MeasureString(optionTwo);
            optionThreeSize = _menuItemFont.MeasureString(optionThree);

            _selectorPosition = new Vector2((_screenWidth / 2 - (_menuSelectionTexture.Width * 1.5f) / 2) - 150, 200);
            optionOnePosition = new Vector2(_screenWidth / 2 - optionOneSize.X / 2, 200);
            optionTwoPosition = new Vector2(_screenWidth / 2 - optionTwoSize.X / 2, 250);
            optionThreePosition = new Vector2(_screenWidth / 2 - optionThreeSize.X / 2, 300);

            return Task.CompletedTask;
        }

        public async Task Update(GameTime gameTime)
        {
            _timeSinceLastInput += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update snowflakes
            foreach (var snowflake in _snowflakes)
            {
                snowflake.Update(gameTime, _screenWidth, _screenHeight);
            }

            // Handle menu selection
            var keyboardState = Keyboard.GetState();

            // Move the selector up and down
            if (_timeSinceLastInput >= _inputCooldown)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && _menuSelection < 3)
                {
                    _menuSelection++;
                    _selectorPosition.Y += 50;
                    _timeSinceLastInput = 0; // Reset cooldown
                }
                else if (keyboardState.IsKeyDown(Keys.Up) && _menuSelection > 1)
                {
                    _menuSelection--;
                    _selectorPosition.Y -= 50;
                    _timeSinceLastInput = 0; // Reset cooldown
                }
                else if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    // Trigger the event and pass the selected option to the parent object
                    OnMenuOptionSelected?.Invoke(_menuSelection);
                }
            }
            
        }



        public async Task Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
          

            // Draw snowflakes

            foreach (var snowflake in _snowflakes)
            {
                snowflake.Draw(spriteBatch);
            }

            // Draw the menu selector

            spriteBatch.Draw(_menuSelectionTexture, _selectorPosition, null, Color.White, 0f, Vector2.Zero, 0.2f, SpriteEffects.None, 0f);

            // Draw the title at the top center

            spriteBatch.DrawString(_menuTitleFont, titleName, titlePosition, Color.White);

            // Draw the menu options


            spriteBatch.DrawString(_menuItemFont, optionOne, optionOnePosition, Color.White);
            spriteBatch.DrawString(_menuItemFont, optionTwo, optionTwoPosition, Color.White);
            spriteBatch.DrawString(_menuItemFont, optionThree, optionThreePosition, Color.White);
        }

        public async Task UnloadContent()
        {
            throw new NotImplementedException();
        }
    }
}
