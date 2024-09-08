using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using nkast.Aether.Physics2D.Dynamics;
using PingPong.Helpers;

namespace PingPong.Implementation.PongGame
{
    /// <summary>
    /// Base class for game entities with physics and game logic.
    /// </summary>
    public abstract class PongGameEntity : GameEntity.GameEntity
    {
        // Physics body for the entity
        protected Body _physicsBody;

        // Position of the entity in the world
        public Vector2 Position
        {
            get => ConvertUnits.ToDisplayUnits(_physicsBody.Position);
            set => _physicsBody.Position = ConvertUnits.ToSimUnits(value);
        }

        // Reference to the physics world
        protected World _world;

        public PongGameEntity(ref World world)
        {
            _world = world;
            _physicsBody = new Body();
        }

        /// <summary>
        /// Initialize the physics body of the entity.
        /// </summary>
        public abstract void InitializePhysics(Vector2 initialPosition);

        /// <summary>
        /// Updates the entity's state based on physics.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update the entity position based on physics body movement
            Position = ConvertUnits.ToDisplayUnits(_physicsBody.Position);
        }

        /// <summary>
        /// Draw the entity on the screen.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }
    }
}