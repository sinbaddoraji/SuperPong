using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Collision.Shapes;
using PingPong.SimpleSprite;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Collision;
using static nkast.Aether.Physics2D.Dynamics.Contacts.ContactSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using nkast.Aether.Physics2D.Common;

namespace PingPong.Implementation.PongGame
{
    /// <summary>
    /// Represents the ball entity in the Pong game.
    /// </summary>
    public class Ball : PongGameEntity
    {
        private readonly float _radius;

        public int PaddleWidth { get; set; } = 200;

        public event EventHandler<string> OnBallHitWall;


        /// <summary>
        /// Initializes a new instance of the <see cref="Ball"/> class.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="world">The physics world.</param>
        /// <param name="color">The color of the ball.</param>
        /// <param name="diameter">The diameter of the ball in pixels.</param>
        public Ball(GraphicsDevice graphics, World world, Color color, int diameter) : base(world)
        {
            _radius = (diameter / 2f) * PixelToUnit; // Convert pixels to simulation units

            // Create ball texture
            Texture = BallTexture.CreateBallTexture(graphics, color, diameter);

            OnBallHitWall += (sender, args) =>
            {
                Debug.WriteLine($"Ball hit {args}");
            };
        }


        /// <summary>
        /// Initializes the physics body for the ball.
        /// </summary>
        /// <param name="initialPosition">The starting position of the ball in pixels.</param>
        public override void InitializePhysics(Vector2 initialPosition)
        {
            // Convert initial position from pixels to simulation units
            Vector2 simPosition = initialPosition * PixelToUnit;

            // Create a new dynamic body at the simulation position
            PhysicsBody = World.CreateBody(simPosition, 0f, BodyType.Dynamic);
            PhysicsBody.Tag = "Ball";
            PhysicsBody.OnCollision += OnCollision;

            // Create a circle shape for the ball
            var circleShape = new CircleShape(_radius, 1f); // Radius in meters, density

            // Attach the shape to the body as a fixture
            var fixture = PhysicsBody.CreateFixture(circleShape);

            // Set the restitution and friction for the fixture
            fixture.Restitution = 1f; // Perfectly elastic collision
            fixture.Friction = 0f;    // No friction
        }

        private bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            // Handle collision logic here
            return HandleCollisionWithPaddle(other, contact);
        }

        private bool HandleCollisionWithPaddle(Fixture otherFixture, Contact contact)
        {
            if (otherFixture.Tag.Equals("TopWall") || otherFixture.Tag.Equals("BottomWall"))
            {
                // Call event handler
                OnBallHitWall?.Invoke(this, otherFixture.Tag.ToString());
            }

            if (otherFixture.Tag != null && otherFixture.Tag.Equals("Paddle"))
            {
                // The ball has collided with the paddle
                AdjustBallVelocityBasedOnPaddleHit(contact, otherFixture);
            }
            else
            {
                // The ball has collided with something else
                // Bounce the ball off the collision normal
                Bounce(contact.Manifold.LocalNormal);
            }


            return true; // Return true to process the collision normally
        }

        private void AdjustBallVelocityBasedOnPaddleHit(Contact contact, Fixture paddleFixture)
        {
            // Get the collision world manifold to find the contact points
            contact.GetWorldManifold(out Vector2 normal, out FixedArray2<Vector2> points);

            // Ensure there's at least one contact point
            if (contact.Manifold.PointCount > 0)
            {
                // Use the first contact point
                Vector2 contactPoint = points[0];

                // Get the paddle's position
                Vector2 paddlePosition = paddleFixture.Body.Position;

                // Calculate the difference between the contact point and the paddle's center
                float hitPosition = contactPoint.X - paddlePosition.X;

                // Get the paddle's half-width in simulation units
                float halfPaddleWidth = (PaddleWidth / 2f) * PixelToUnit;

                // Normalize the hit position (-1 to 1)
                float normalizedHitPosition = hitPosition / halfPaddleWidth;

                // Clamp the normalized position to ensure it stays within [-1, 1]
                normalizedHitPosition = MathHelper.Clamp(normalizedHitPosition, -1f, 1f);

                // Adjust the ball's velocity
                float speed = PhysicsBody.LinearVelocity.Length();

                // Calculate the bounce angle
                float maxBounceAngle = MathHelper.ToRadians(60); // Maximum bounce angle in radians
                float bounceAngle = normalizedHitPosition * maxBounceAngle;

                float newXVelocity = speed * (float)Math.Sin(bounceAngle);
                float newYVelocity = speed * (float)Math.Cos(bounceAngle) * -Math.Sign(PhysicsBody.LinearVelocity.Y);

                PhysicsBody.LinearVelocity = new Vector2(newXVelocity, newYVelocity);
            }
        }



        /// <summary>
        /// Updates the ball's state.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the visual position based on the physics simulation
            Position = PhysicsBody.Position * UnitToPixel; // Convert back to pixels
        }

        /// <summary>
        /// Resets the ball's position and velocity.
        /// </summary>
        /// <param name="position">The new position in pixels.</param>
        /// <param name="linearVelocity">The new linear velocity in pixels per second.</param>
        public void Reset(Vector2 position, Vector2 linearVelocity)
        {
            // Convert position and velocity to simulation units
            Vector2 simPosition = position * PixelToUnit;
            Vector2 simVelocity = linearVelocity * PixelToUnit;

            // Reset the position and linear velocity of the ball
            PhysicsBody.Position = simPosition;
            PhysicsBody.LinearVelocity = simVelocity;
            PhysicsBody.Awake = true; // Ensure the body is active
        }

        /// <summary>
        /// Reflects the ball's velocity off a surface normal.
        /// </summary>
        /// <param name="normal">The normal vector of the surface.</param>
        public void Bounce(Vector2 normal)
        {
            // Reflect the ball's velocity off the surface normal
            PhysicsBody.LinearVelocity = Vector2.Reflect(PhysicsBody.LinearVelocity, normal);
        }

        /// <summary>
        /// Sets the velocity of the ball.
        /// </summary>
        /// <param name="velocity">The new velocity in pixels per second.</param>
        public void SetVelocity(Vector2 velocity)
        {
            // Convert velocity to simulation units
            PhysicsBody.LinearVelocity = velocity * PixelToUnit;
        }

        /// <summary>
        /// Draws the ball to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            // Draw the ball with its origin at the center
            spriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White,
                0f,
                new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}
