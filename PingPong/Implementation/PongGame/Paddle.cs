using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using PingPong.Helpers;
using PingPong.SimpleSprite;
using System;
using PingPong.Implementation.Controller;
using PingPong.Interface;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using static nkast.Aether.Physics2D.Dynamics.Contacts.ContactSolver;
using nkast.Aether.Physics2D.Dynamics.Joints;

namespace PingPong.Implementation.PongGame
{
    internal class Paddle : PongGameEntity
    {
        public int Width { get; }
        public int Height { get; }

        private readonly GraphicsDevice _graphics;
        private readonly float _computerPaddleSpeed = 5f;

        public bool? IsPlayer1 { get; set; } = null;

        public float PaddleSpeed { get; set; } = 5f;

       private IGameScreenControllerManager _gameScreenControllerManager;

        public Paddle(GraphicsDevice graphics, World world, Color color, int width, int height) : base(world)
        {
            Width = width;
            Height = height;

            _graphics = graphics;

            // Create paddle texture
            Texture = PaddleTexture.CreatePaddleTexture(graphics, color, width, height);

            _gameScreenControllerManager = new GameScreenControllerManager();
            _gameScreenControllerManager.CooldownPeriod = 0;
        }

        /// <summary>
        /// Changes the paddle's color by regenerating the texture.
        /// </summary>
        /// <param name="color">The new color.</param>
        public void ChangeColor(Color color)
        {
            Texture = PaddleTexture.CreatePaddleTexture(_graphics, color, Width, Height);
        }

        /// <summary>
        /// Initializes the paddle's physics body.
        /// </summary>
        /// <param name="initialPosition">The initial position in pixels.</param>
        public override void InitializePhysics(Vector2 initialPosition)
        {
            // Convert initial position to simulation units
            Vector2 simPosition = initialPosition * PixelToUnit;

            // Create a kinematic body for the paddle at the simulation position
            PhysicsBody = World.CreateBody(simPosition, 0f, BodyType.Kinematic);
            PhysicsBody.Tag = "Paddle";

            // In InitializePhysics, after creating the PhysicsBody
            // var axis = new Vector2(1f, 0f); // Movement along the X-axis
            // var joint = JointFactory.CreatePrismaticJoint(World, null, PhysicsBody, PhysicsBody.Position, axis);
            // joint.LimitEnabled = false; // Set to true and define limits if you want to restrict the paddle's range


            PhysicsBody.OnCollision += OnCollision;



            // Create rectangle vertices for the paddle
            float halfWidth = (Width / 2f) * PixelToUnit;
            float halfHeight = (Height / 2f) * PixelToUnit;
            Vertices paddleVertices = PolygonTools.CreateRectangle(halfWidth, halfHeight);

            // Create a polygon shape using the paddle vertices
            var paddleShape = new PolygonShape(paddleVertices, 1f); // Density

            // Attach the shape to the body as a fixture
            var fixture = PhysicsBody.CreateFixture(paddleShape);
            fixture.Tag = "Paddle";

            // Set fixture properties
            fixture.Restitution = 0f;
            fixture.Friction = 0f;

            // Ensure the physics body doesn't rotate
            PhysicsBody.FixedRotation = true;
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // Get the tag of the other body
            string otherTag = fixtureB.Body.Tag as string;

            if (otherTag == "LeftWall")
            {
                // Collision with the left wall - push paddle to the right
                ApplyPushback(Vector2.UnitX);
            }
            else if (otherTag == "RightWall")
            {
                // Collision with the right wall - push paddle to the left
                ApplyPushback(-Vector2.UnitX);
            }

            // Continue with normal collision processing
            return true;
        }

        private void ApplyPushback(Vector2 direction)
        {
            // Define the impulse magnitude
            float impulseMagnitude = 0.5f; // Adjust this value as needed

            // Apply the impulse to the paddle's physics body
            PhysicsBody.ApplyLinearImpulse(direction * impulseMagnitude);

            // Optionally, limit the paddle's velocity to prevent it from moving too fast
            float maxVelocity = 5f; // Adjust as needed
            PhysicsBody.LinearVelocity = Vector2.Clamp(
                PhysicsBody.LinearVelocity,
                new Vector2(-maxVelocity, 0f),
                new Vector2(maxVelocity, 0f)
            );
        }




        /// <summary>
        /// Moves the paddle to a new position.
        /// </summary>
        /// <param name="newPosition">The new position in pixels.</param>
        public void Move(Vector2 newPosition)
        {
            // Convert new position to simulation units
            Vector2 simPosition = newPosition * PixelToUnit;

            // Set the paddle's position directly for controlled movement
            PhysicsBody.Position = simPosition;
            PhysicsBody.Awake = true; // Ensure the body is active

            // Update the visual position
            Position = newPosition;
        }

        /// <summary>
        /// AI Paddle movement logic.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        /// <param name="ballPosition">The ball's position in pixels.</param>
        public void UpdateAi(GameTime gameTime, Vector2 ballPosition)
        {
            // Simple AI: Move paddle towards the ball's X position
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float targetX = ballPosition.X;
            float paddleSpeed = _computerPaddleSpeed * deltaTime * UnitToPixel; // Convert speed to pixels per frame

            // Calculate the new X position, moving towards the target X
            float newX = MathHelper.Lerp(Position.X, targetX, paddleSpeed / Math.Abs(targetX - Position.X + 0.01f)); // Added small value to prevent division by zero

            // Clamp the new X position within screen bounds (adjust as needed)
            newX = MathHelper.Clamp(newX, 0 + Width / 2f, _graphics.Viewport.Width - Width / 2f);

            // Set the new position
            Move(new Vector2(newX, Position.Y));
        }

        /// <summary>
        /// Updates the paddle's state.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _gameScreenControllerManager.Update(gameTime);

            if (IsActive)
            {
                if (IsPlayer1 == true)
                {
                    if (_gameScreenControllerManager.PlayerOneKeyLeft())
                    {
                        PhysicsBody.LinearVelocity = new Vector2(-5, 0);
                        PhysicsBody.Awake = true;
                    }
                    else if (_gameScreenControllerManager.PlayerOneKeyRight())
                    {
                        PhysicsBody.LinearVelocity = new Vector2(5, 0);
                        PhysicsBody.Awake = true;
                    }
                    else
                    {
                        // Stop the paddle when no key is pressed
                        PhysicsBody.LinearVelocity = Vector2.Zero;
                    }
                }
                else if (IsPlayer1 == false)
                {
                    if (_gameScreenControllerManager.PlayerTwoKeyLeft())
                    {
                        PhysicsBody.LinearVelocity = new Vector2(-PaddleSpeed, 0);
                        PhysicsBody.Awake = true;
                    }
                    else if (_gameScreenControllerManager.PlayerTwoKeyRight())
                    {
                        PhysicsBody.LinearVelocity = new Vector2(PaddleSpeed, 0);
                        PhysicsBody.Awake = true;
                    }
                    else
                    {
                        // Stop the paddle when no key is pressed
                        PhysicsBody.LinearVelocity = Vector2.Zero;
                    }
                }
                else
                {
                    // Call the AI update method for the AI-controlled paddle
                    // UpdateAi(gameTime, Ball.Position);
                }
            }

            // Sync the display position with the physics body's position
            Position = PhysicsBody.Position * UnitToPixel;
        }



        /// <summary>
        /// Draws the paddle to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the paddle with its origin at the center
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

        /// <summary>
        /// Gets the rectangle representing the paddle's bounds.
        /// </summary>
        /// <returns>A rectangle in pixels.</returns>
        public Rectangle GetRectangle()
        {
            return new Rectangle(
                (int)(Position.X - Texture.Width / 2f),
                (int)(Position.Y - Texture.Height / 2f),
                Texture.Width,
                Texture.Height);
        }

        /// <summary>
        /// Resets the paddle's velocity (if applicable).
        /// </summary>
        public void ResetVelocity()
        {
            PhysicsBody.LinearVelocity = Vector2.Zero;
            PhysicsBody.AngularVelocity = 0f;
            PhysicsBody.Awake = true;
        }
    }
}
