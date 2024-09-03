using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PingPong.Interface;

namespace PingPong.Implementation.GameEntitiy;

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

    public bool Intersects(IGameEntity entity)
    {
        return GetRectangle().Intersects(entity.GetRectangle());
    }

    public bool Contains(Vector2 point)
    {
        return GetRectangle().Contains(point);
    }

    public bool Contains(IGameEntity entity)
    {
        return GetRectangle().Contains(entity.GetRectangle());
    }

    public void Move(Vector2 direction)
    {
        Position += direction;
    }


    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public void SetTexture(Texture2D texture)
    {
        Texture = texture;
    }

    public void SetSize(Vector2 size)
    {
        Texture = new Texture2D(Texture.GraphicsDevice, (int)size.X, (int)size.Y);
    }



}