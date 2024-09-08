using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PingPong.Implementation.Controller;
using PingPong.Implementation.GameEntity;
using PingPong.Implementation.GameMenuScreen;
using PingPong.Interface;
using PingPong.SimpleSprite;

namespace PingPong.Screens;

public class GameMenuScreen : IGameScreen
{
    private readonly int _screenWidth;
    private readonly int _screenHeight;

    private Random _random;
    private Texture2D _snowflakeTexture;
    private readonly GraphicsDevice _graphicsDevice;

    private const string TitleName = "Super Pong";
    private const string OptionOne = "Arcade Mode";
    private const string OptionTwo = "Player vs Player";
    private const string OptionThree = "Player vs Computer";
    private const string OptionFour = "Comuter vs Computer";
    private const string OptionFive = "Exit";

    private VerticalMenu _verticalMenu;
    private readonly GraphicsDeviceManager _graphics;

    public GameMenuScreen(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        _graphicsDevice = graphicsDevice;
        _graphics = graphics;
        _screenWidth = graphics.PreferredBackBufferWidth;
        _screenHeight = graphics.PreferredBackBufferHeight;
    }

    public (int, int) ScreenSize { get; set; }
    public List<IGameEntity> GameEntities { get; set; } = new();

    public IGameScreenControllerManager GameScreenControllerManager { get; set; }

    public delegate void MenuOptionSelectedHandler(int selectedOption);
    public event MenuOptionSelectedHandler OnMenuOptionSelected;

    public void Initialize(ContentManager contentManager)
    {
        _random = new Random();

        // Load snowflake texture
        _snowflakeTexture = SnowFlakeTexture.CreateSnowflakeTexture(_graphicsDevice, 8, 8);
        GameScreenControllerManager = new GameScreenControllerManager();

        // Initialize snowflakes with random positions and speeds
        for (int i = 0; i < 50; i++)
        {
            var position = new Vector2(_random.Next(_screenWidth), _random.Next(_screenHeight));
            var speed = (float)_random.NextDouble() * 50 + 50; // Random speed between 50 and 100
            GameEntities.Add(
                new Snowflake(position, speed, _snowflakeTexture)
                {
                    ParentSize = (_screenWidth, _screenHeight)
                });
        }


        // Create menu
        _verticalMenu = new VerticalMenu(TitleName, 
            new List<string> { OptionOne, OptionTwo, OptionThree, OptionFour, OptionFive }, 
            contentManager.Load<SpriteFont>("MenuItem"), Color.White, Color.Yellow, GameScreenControllerManager)
        {
            Spacing = 50,
            TitleSpriteFont = contentManager.Load<SpriteFont>("MenuTitleFont")
        };

        _verticalMenu.OnMenuOptionSelected += selectedOption =>
        {
            // Invoke on menu option selected event
            OnMenuOptionSelected?.Invoke(selectedOption);
        };

        _verticalMenu.Initialize(_graphicsDevice, _graphics, contentManager);

        GameEntities.Add(_verticalMenu);
    }

    public void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch)
    {
        GameEntities.ForEach(entity => entity.Draw(gameTime, spriteBatch, Color.White));
    }

    public void UpdateEntities(GameTime gameTime)
    {
        GameScreenControllerManager.Update(gameTime);

        GameEntities.ForEach(entity => entity.Update(gameTime));
    }

    public void NavigateTo(INavigationManager navigationManager, string pageName)
    {
        // Do Not
    }

    public void OnNavigateTo(INavigationManager navigationManager, dynamic parameters)
    {
        // Do nothing
    }

    public void NavigateBackward(INavigationManager navigationManager)
    {
        // Do nothing
    }
}