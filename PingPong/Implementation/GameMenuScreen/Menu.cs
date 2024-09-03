using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PingPong.Implementation.GameEntitiy;
using PingPong.Interface;

namespace PingPong.Implementation.GameMenuScreen
{
    public enum MenuOrientation { Vertical, Horizontal }

    public class Menu : GameEntity
    {
        public MenuOrientation Orientation { get; set; }
        public List<MenuItem> Items { get; set; }

        public float Spacing { get; set; }
    

        public Menu(MenuOrientation orientation, List<string> menuOptions)
        {
            Orientation = orientation;
            Items = new List<MenuItem>();

            // Create menu items and add them based on position

            Vector2 nextPosition = Position;
            if (orientation == MenuOrientation.Horizontal)
            {
                nextPosition.X += Spacing;
            }
            else
            {
                nextPosition.Y += Spacing;
            }

            foreach (var option in menuOptions)
            {
                var item = new MenuItem();
                item.Texture = null;
                item.Position = nextPosition;
                Items.Add(item);

                if (orientation == MenuOrientation.Horizontal)
                {
                    nextPosition.X += Spacing;
                }
                else
                {
                    nextPosition.Y += Spacing;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var item in Items)
            {
                item.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var item in Items)
            {
                item.Draw(gameTime, spriteBatch);
            }
        }


    }

    public class MenuItem : GameEntity
    {

    }
}
