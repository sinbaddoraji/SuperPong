using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using PingPong.Helpers;
using PingPong.SimpleSprite;

namespace PingPong.Implementation.PongGame
{
    internal class Paddle : PongGameEntity
    {
        public int Width { get; }
        public int Height { get; }

        private GraphicsDevice _graphics;
        private float _computerPaddleSpeed = 1000f; // AI paddle speed

        public Paddle(GraphicsDevice graphics, ref World world, Color color, int width, int height) : base(ref world)
        {
            Width = width;
            Height = height;

            _graphics = graphics;

            // Create paddle texture
            Texture = PaddleTexture.CreatePaddleTexture(graphics, color, width, height);
        }

        // Changes the paddle's color by regenerating the texture
        public void ChangeColor(Color color)
        {
            Texture = PaddleTexture.CreatePaddleTexture(_graphics, color, Width, Height);
        }

        // Initialize the paddle's physics body
        public override void InitializePhysics(Vector2 initialPosition)
        {
            // Create a rectangle physics body for the paddle
            _physicsBody = _world.CreateRectangle(ConvertUnits.ToSimUnits(Width), ConvertUnits.ToSimUnits(Height), 1f);
            _physicsBody.BodyType = BodyType.Kinematic; // Paddles are kinematic as they are controlled directly
            // _physicsBody.Friction = 0f; // No friction for smooth movement
            _physicsBody.Position = ConvertUnits.ToSimUnits(initialPosition);
        }

        // Method to move the paddle for player or AI
        public void Move(Vector2 newPosition)
        {
            // Set the paddle's position directly for controlled movement
            _physicsBody.Position = ConvertUnits.ToSimUnits(newPosition);
        }

        // AI Paddle movement logic
        public void UpdateAI(GameTime gameTime, Vector2 ballPosition)
        {
            // Simple AI: Move paddle towards the ball's X position
            float targetX = ballPosition.X - Width / 2; // Center paddle on ball's X
            float paddleSpeed = _computerPaddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move paddle towards the target position with some speed limit
            Vector2 newPosition = new Vector2(MathHelper.Lerp(Position.X, targetX, paddleSpeed), Position.Y);

            // Set the new position (this automatically updates the physics body)
            Move(newPosition);
        }

        // Update the paddle position based on the physics body
        public new void Update(GameTime gameTime)
        {
            // Sync the display position with the physics body
            Position = ConvertUnits.ToDisplayUnits(_physicsBody.Position);
        }
    }
}
