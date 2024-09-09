using Microsoft.Xna.Framework;
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
        protected Body PhysicsBody;

        // Position of the entity in the world
        public new Vector2 Position
        {
            get => ConvertUnits.ToDisplayUnits(PhysicsBody.Position);
            set => PhysicsBody.Position = ConvertUnits.ToSimUnits(value);
        }

        // Reference to the physics world
        protected World World;

        protected PongGameEntity(ref World world)
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