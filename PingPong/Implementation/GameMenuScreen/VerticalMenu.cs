using System;
using System.Collections.Generic;
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

namespace PingPong.Implementation.GameMenuScreen
{
    public class VerticalMenu : GameEntity
    {
        public List<MenuItem> Items { get; set; }

        public float Spacing { get; set; }
    
        public int SelectedIndex { get; set; }

        private SpriteFont _spriteFont;

        public SpriteFont TitleSpriteFont { get; set; }

        private Texture2D _menuSelectionTexture;
        private Texture2D _menuSelection2Texture;
        private readonly string _title;
        private SpriteFont _menuTitleFont;
        private SpriteFont _menuItemFont;

        private int _menuSelection = 1;
        private float _inputCooldown = 0.2f; // 200ms cooldown for input
        private float _timeSinceLastInput = 0;

        public delegate void MenuOptionSelectedHandler(int selectedOption);
        public event MenuOptionSelectedHandler OnMenuOptionSelected;


        private Color SelectedMenuColor { get; set; } = Color.YellowGreen;

        public VerticalMenu(string title, List<string> menuOptions, SpriteFont spriteFont, Color color, Color selectColor)
        {
            _title = title;
            Items = new List<MenuItem>();
            _spriteFont = spriteFont;

            // Create menu items and add them based on position

            Vector2 nextPosition = Position;
            nextPosition.Y += Spacing;

            foreach (var option in menuOptions)
            {
                var item = new MenuItem
                {
                    Texture = null,
                    Position = nextPosition,
                    Text = option,
                    DefaultColor = color,
                    SelectColor = selectColor
                };
                Items.Add(item);

                nextPosition.Y += Spacing;
            }

            
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
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            // Draw menu title
            spriteBatch.DrawString(TitleSpriteFont, _title, Position, Color.White);

            DrawTextMenu(gameTime, spriteBatch, _spriteFont);

            // Drae menu fingers
            //var selectedPosition = Items[SelectedIndex].Position;
            //var selectedSize = Items[SelectedIndex].GetSize();
            //var selectedRectangle = new Rectangle((int)selectedPosition.X, (int)selectedPosition.Y, (int)selectedSize.X, (int)selectedSize.Y);

            //spriteBatch.Draw(_menuSelectionTexture, selectedRectangle, Color.White);
            //spriteBatch.Draw(_menuSelection2Texture, selectedRectangle, Color.White);
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
