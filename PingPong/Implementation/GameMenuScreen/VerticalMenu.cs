using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PingPong.Implementation.GameEntitiy;
using PingPong.Interface;
using PingPong.Properties;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace PingPong.Implementation.GameMenuScreen
{
    public class VerticalMenu : GameEntity
    {
        public List<MenuItem> Items { get; set; }

        public float Spacing { get; set; } = 50;
    
        public int SelectedIndex { get; set; }

        private SpriteFont _spriteFont;

        public SpriteFont TitleSpriteFont { get; set; }

        private Texture2D _menuSelectionTexture;
        private Texture2D _menuSelection2Texture;
        private readonly string _title;
        private SpriteFont _menuTitleFont;
        private SpriteFont _menuItemFont;

        private GameEntity MenuTitle { get; set; }

        private int _menuSelection = 1;
        private float _inputCooldown = 0.2f; // 200ms cooldown for input
        private float _timeSinceLastInput = 0;

        public delegate void MenuOptionSelectedHandler(int selectedOption);
        public event MenuOptionSelectedHandler OnMenuOptionSelected;


        private Color DefaultMenuColor { get; set; } = Color.White;
        private Color SelectedMenuColor { get; set; } = Color.YellowGreen;

        private int _screenWidth = 1200;

        private int _screenHeight = 800;

        private int _startingMenuPosition = 80;

        private List<string> MenuOptions { get; set; }

        public VerticalMenu(string title, List<string> menuOptions, SpriteFont spriteFont, Color color, Color selectColor)
        {
            _title = title;
            Items = new List<MenuItem>();
            _spriteFont = spriteFont;

            // Create menu items and add them based on position
            DefaultMenuColor = color;
            SelectedMenuColor = selectColor;

            MenuOptions = menuOptions;
        }

        public void Initalize(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            using (MemoryStream stream = new MemoryStream(Resources.MenuSelector))
            {
                _menuSelectionTexture = Texture2D.FromStream(graphicsDevice, stream);
            }

            using (MemoryStream stream = new MemoryStream(Resources.MenuSelector2))
            {
                _menuSelection2Texture = Texture2D.FromStream(graphicsDevice, stream);
            }

            _menuTitleFont = contentManager.Load<SpriteFont>("MenuTitleFont");
            _menuItemFont = contentManager.Load<SpriteFont>("MenuItem");

            float startingSpacing = _startingMenuPosition;

            MenuTitle = new GameEntity
            {
                Texture = null,
                Position = new Vector2(_screenWidth / 2 - _menuTitleFont.MeasureString(_title).X / 2, startingSpacing),
            };

            startingSpacing += Spacing * 2;

            //optionOneSize = _menuItemFont.MeasureString(optionOne);
            //optionOnePosition = new Vector2(_screenWidth / 2 - optionOneSize.X / 2, 200);

            foreach (var option in MenuOptions)
            {
                var menuItemSize = _menuItemFont.MeasureString(option);
                var menuItemPosition = new Vector2(_screenWidth / 2 - menuItemSize.X / 2, startingSpacing);

                var item = new MenuItem
                {
                    Texture = null,
                    Position = menuItemPosition,
                    Text = option,
                    DefaultColor = DefaultMenuColor,
                    SelectColor = SelectedMenuColor
                };
                Items.Add(item);

                startingSpacing += Spacing;
            }

        }

        public void MoveUp()
        {
            SelectedIndex--;
            if (SelectedIndex < 0)
            {
                SelectedIndex = Items.Count - 1;
            }
        }

        public void MoveDown()
        {
            SelectedIndex++;
            if (SelectedIndex >= Items.Count)
            {
                SelectedIndex = 0;
            }
        }

        
        public override void Update(GameTime gameTime)
        {
            foreach (var item in Items)
            {
                item.Update(gameTime);
            }
            var keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var gamePadState = Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            if (_timeSinceLastInput > 0)
            {
                _timeSinceLastInput -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && _timeSinceLastInput <= 0)
            {
                MoveDown();
                _timeSinceLastInput = _inputCooldown;
            }
            else if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && _timeSinceLastInput <= 0)
            {
                MoveUp();
                _timeSinceLastInput = _inputCooldown;
            }

            if (gamePadState.ThumbSticks.Left.Y > 0.5 && _timeSinceLastInput <= 0)
            {
                MoveUp();
                _timeSinceLastInput = _inputCooldown;
            }
            else if (gamePadState.ThumbSticks.Left.Y < -0.5 && _timeSinceLastInput <= 0)
            {
                MoveDown();
                _timeSinceLastInput = _inputCooldown;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            // Draw menu title
            MenuTitle.DrawText(gameTime, spriteBatch, _menuTitleFont, _title, Color.White);

            DrawTextMenu(gameTime, spriteBatch, _spriteFont);

            // Draw fingers pointing at selected option
            int xSpacing = 100;

            var selectedItemSize = _menuItemFont.MeasureString(Items[SelectedIndex].Text);
            var menuPointerWidth = _menuSelectionTexture.Width;

            var selectedRectangle1 = new Rectangle((int)Items[SelectedIndex].Position.X - (menuPointerWidth / 2) - xSpacing, (int)Items[SelectedIndex].Position.Y, 50, 50);
            var selectedRectangle2 = new Rectangle((int)((int)Items[SelectedIndex].Position.X + selectedItemSize.X + xSpacing), (int)Items[SelectedIndex].Position.Y, 50, 50);

            spriteBatch.Draw(_menuSelectionTexture, selectedRectangle1, Color.White);
            spriteBatch.Draw(_menuSelection2Texture, selectedRectangle2, Color.White);
        }
        private void DrawTextMenu(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            int index = 0;
            foreach (var item in Items)
            {
                var color = index == SelectedIndex ? item.SelectColor : item.DefaultColor;
                item.DrawText(gameTime, spriteBatch, spriteFont, item.Text, color);
                index++;
            }
        }


    }
}
