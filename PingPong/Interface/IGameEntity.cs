using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PingPong.Interface;

public interface IGameEntity
{
    public Vector2 Position { get; set; }

    public Texture2D Texture { get; set; }

    Rectangle GetRectangle();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color);
}