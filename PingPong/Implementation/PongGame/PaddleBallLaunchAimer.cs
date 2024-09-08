﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Interface;
using PingPong.SimpleSprite;
using System;

namespace PingPong.Implementation.PongGame;

public class PaddleBallLaunchAimer : PongGameEntity
{
    private readonly GraphicsDevice _graphics;
    private readonly Color _color;
    private int _length;
    private int _angle;
    private bool _isPointingUpwards = false;

    private int effectiveMinAngle = 0;
    private int effectiveMaxAngle = 0;

    private int _maxHeight = 100;
    private int _minHeight = 50;

    // Release the ball event
    public delegate void BallLaunchHandler(Vector2 launchDirection);
    public event BallLaunchHandler OnBallLaunch;

    IGameScreenControllerManager _gameScreenControllerManager;

    public PaddleBallLaunchAimer(GraphicsDevice graphics, IGameScreenControllerManager gameScreenControllerManager, Color color, int length, int angle,bool isPointingUpwards = true)
    {
        _graphics = graphics;
        _color = color;
        _length = length;
        _angle = angle;
        _isPointingUpwards = isPointingUpwards;
        // Create paddle texture

        _gameScreenControllerManager = gameScreenControllerManager;


        OnBallLaunch += (launchDirection) =>
        {
            // Launch the ball
        };

        if (_isPointingUpwards)
        {
             _angle = 180;
            effectiveMinAngle = 115;
            effectiveMaxAngle = 245;

            // TODO: Comment the below out
            //effectiveMinAngle = int.MinValue;
            //effectiveMaxAngle = int.MaxValue;
        }
        else
        {
            _angle = 0;
        }

        length = 200;
        Texture = LineTexture.CreateLineTexture(_graphics, _color, length);
    }

    public void ChangeColor(Color color)
    {
        Texture = LineTexture.CreateLineTexture(_graphics, color, _length);
    }

    
    public void AimLeft()
    {
        if (_isPointingUpwards)
        {
            if (_angle <= effectiveMinAngle)
            {
                return;
            }
        }

        _angle -= 1;

        // rotate the texture to point left
        
    }

    public void AimRight()
    {
        if (_isPointingUpwards)
        {
            if (_angle >= effectiveMaxAngle)
            {
                return;
            }
        }
        _angle += 1;
    }

    private void ExtendAimLine()
    {
        if(_length < _maxHeight)
             _length++;
        Redraw();
    }

    private void ReduceAimLine()
    {
        if (_length > _minHeight)
            _length--;

        Redraw();
    }

    private void Redraw()
    {
        Texture = LineTexture.CreateLineTexture(_graphics, _color, _length);
    }

    public void LaunchBall()
    {
        // Launch the ball based on angle and rotation of the Texture
        var launchDirection = new Vector2(0, 0);
        if (_isPointingUpwards)
        {
            launchDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(_angle)), (float)Math.Sin(MathHelper.ToRadians(_angle)));
        }
        else
        {
            launchDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(_angle)), (float)Math.Sin(MathHelper.ToRadians(_angle)));
        }

        OnBallLaunch.Invoke(launchDirection);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.Draw(Texture, Position, null, color, MathHelper.ToRadians(_angle), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
    }

    public override void Update(GameTime gameTime)
    {
        if(!IsActive)
            return;
       
        // TODO: Add logic that checks if the controller is for player 1 or 2
        var gamePadState = GamePad.GetState(PlayerIndex.One);

        if (IsActive)
        {
            if (!_isPointingUpwards)
            {
                if (_gameScreenControllerManager.PlayerTwoKeyUp())
                {
                    ExtendAimLine();
                }

                if (_gameScreenControllerManager.PlayerTwoKeyDown())
                {
                    ReduceAimLine();
                }

                if (_gameScreenControllerManager.PlayerTwoKeyLeft())
                {
                    AimLeft();
                }

                if (_gameScreenControllerManager.PlayerTwoKeyRight())
                {
                    AimRight();
                }

                if (_gameScreenControllerManager.PlayerTwoKeyAction())
                {
                    LaunchBall();
                }
            }
            else
            {
                if (_gameScreenControllerManager.PlayerOneKeyUp())
                {
                    ExtendAimLine();
                }

                if (_gameScreenControllerManager.PlayerOneKeyDown())
                {
                    ReduceAimLine();
                }

                if (_gameScreenControllerManager.PlayerOneKeyLeft())
                {
                    AimLeft();
                }

                if (_gameScreenControllerManager.PlayerOneKeyRight())
                {
                    AimRight();
                }

                if (_gameScreenControllerManager.PlayerOneKeyAction())
                {
                    LaunchBall();
                }
            }
        }
    }
}