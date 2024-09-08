using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using PingPong.SimpleSprite;
using PingPong.Helpers;

namespace PingPong.Implementation.PongGame
{
    public class Ball : PongGameEntity
    {
        private float _radius;

        // Velocity is now handled by the physics engine, so we don't manually manage it here
        public Ball(GraphicsDevice graphics, ref World world, Color color, int diameter) : base(ref world)
        {
            _radius = diameter / 2f;

            // Create ball texture
            Texture = BallTexture.CreateBallTexture(graphics, color, diameter);
        }

        // Override InitializePhysics to set up the physics body for the ball
        public override void InitializePhysics(Vector2 initialPosition)
        {
            // Create a circular physics body for the ball in the world
            _physicsBody = _world.CreateCircle(_radius, 1f);  // Radius in simulation units (meters), density
            _physicsBody.BodyType = BodyType.Dynamic;         // Dynamic body so it can move
            // _physicsBody.Restitution = 1f;                    // High restitution for bouncing
            // _physicsBody.Friction = 0f;                       // No friction for smooth movement

            // Set the initial position of the ball in the physics world
            _physicsBody.Position = ConvertUnits.ToSimUnits(initialPosition);
        }

        // Update is now managed by the physics engine, which updates the ball's position
        public new void Update(GameTime gameTime)
        {
            // Update the position of the ball based on its physics body
            Position = ConvertUnits.ToDisplayUnits(_physicsBody.Position);

            // Optionally, you can adjust the velocity based on user input or game conditions
            // For example, adding some speed boost logic (if needed)
            // Velocity = ConvertUnits.ToDisplayUnits(_physicsBody.LinearVelocity);
        }
    }
}
