using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using PingPong.SimpleSprite;
using PingPong.Helpers;

namespace PingPong.Implementation.PongGame
{
    public class Ball : PongGameEntity
    {
        private readonly float _radius;

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
            PhysicsBody = World.CreateCircle(_radius, 1f);  // Radius in simulation units (meters), density
            PhysicsBody.BodyType = BodyType.Dynamic;         // Dynamic body so it can move

            // Set the initial position of the ball in the physics world
            PhysicsBody.Position = ConvertUnits.ToSimUnits(initialPosition);
        }

        // Update is now managed by the physics engine, which updates the ball's position
        public new void Update(GameTime gameTime)
        {
            // Update the position of the ball based on its physics body
            Position = ConvertUnits.ToDisplayUnits(PhysicsBody.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(!IsVisible) return;
            // Draw the paddle at the current position
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
