using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PingPong.Interface;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace PingPong.Implementation.GameEntity;

public class GameEntity : IGameEntity
{
    public Vector2 Position { get; set; }
    public Texture2D Texture { get; set; }

    // Define update event
    public event EventHandler<IGameEntity> UpdateEvent;

    public virtual void Update(GameTime gameTime)
    {
        // Invoke the update event
        UpdateEvent?.Invoke(this, this);
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.Draw(Texture, Position, color);
    }

    public virtual void DrawText(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Color textColor)
    {
        spriteBatch.DrawString(spriteFont, text, Position, textColor);
    }

    public Rectangle GetRectangle()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
    }
}