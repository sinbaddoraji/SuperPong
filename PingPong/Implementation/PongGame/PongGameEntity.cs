using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace PingPong.Implementation.PongGame
{
    /// <summary>
    /// Base class for game entities with physics and game logic.
    /// </summary>
    public abstract class PongGameEntity : GameEntity.GameEntity
    {
        // Unit conversion constants (ensure consistency with other classes)
        protected const float UnitToPixel = 100f; // 1 meter = 100 pixels
        protected const float PixelToUnit = 1f / UnitToPixel;

        // Physics body for the entity
        protected Body PhysicsBody;

        // Position of the entity in pixels
        public Vector2 Position
        {
            get => PhysicsBody.Position * UnitToPixel; // Convert to pixels
            set
            {
                if (PhysicsBody != null)
                {
                    PhysicsBody.Position = value * PixelToUnit; // Convert to simulation units
                }
            }
        }


        // Reference to the physics world
        protected World World;

        protected PongGameEntity(World world)
        {
            World = world;
            PhysicsBody = new Body();
        }

        /// <summary>
        /// Initialize the physics body of the entity.
        /// </summary>
        public abstract void InitializePhysics(Vector2 initialPosition);
    }
}