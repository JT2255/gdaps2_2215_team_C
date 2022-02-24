using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MurderMystery
{
    enum PlayerState
    {
        FacingLeft,
        FacingRight,
        WalkingLeft,
        WalkingRight
    }

    class Player
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private List<Item> inventory;
        private IDictionary<bool, string> clues;
        private int moveSpeed;
        private Vector2 position;
        private Texture2D texture;
        private KeyboardState kbState;
        private int height;
        private int width;
        private double fps;
        private double time;
        private int frame;
        private double timePerFrame;
        private int numOfFrames;
        private PlayerState state;
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties
        //return position of player
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        #endregion
        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        public Player(string name, Vector2 position, Texture2D texture, int height, int width)
        {
            this.name = name;
            this.position = position;
            this.texture = texture;
            this.width = width;
            this.height = height;
            moveSpeed = 4;
            inventory = new List<Item>();
            clues = new Dictionary<bool, string>();
            fps = 6;
            timePerFrame = 1 / fps;
            numOfFrames = 6;
            state = PlayerState.FacingLeft;
        }
        #endregion
        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        //move character with A and D or left and right keys
        public void Move()
        {
            kbState = Keyboard.GetState();

            switch (state)
            {
                //when facing right, able to walk right or look back left
                case (PlayerState.FacingRight):
                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        state = PlayerState.WalkingRight;
                    }

                    if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                    {
                        state = PlayerState.FacingLeft;
                    }
                    break;
                //when walking right, move character
                case (PlayerState.WalkingRight):                   
                    position.X += moveSpeed;

                    if (kbState.IsKeyUp(Keys.Right) && kbState.IsKeyUp(Keys.D))
                    {
                        state = PlayerState.FacingRight;
                    }
                    break;
                //when facing left, able to walk left or look back right
                case PlayerState.FacingLeft:
                    if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                    {
                        state = PlayerState.WalkingLeft;
                    }

                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        state = PlayerState.FacingRight;
                    }
                    break;
                //when walking left, move character
                case PlayerState.WalkingLeft:
                    position.X -= moveSpeed;

                    if (kbState.IsKeyUp(Keys.Left) && kbState.IsKeyUp(Keys.A))
                    {
                        state = PlayerState.FacingLeft;
                    }
                    break;
                default:
                    break;
            }       
        }

        /// <summary>
        /// draw player depending on current movement state
        /// </summary>
        /// <param name="sb">sprite batch</param>
        public void Draw(SpriteBatch sb)
        {
            switch (state)
            {
                case (PlayerState.FacingRight):
                    DrawStanding(sb, SpriteEffects.None);
                    break;
                case (PlayerState.WalkingRight):
                    DrawWalking(sb, SpriteEffects.None);
                    break;
                case PlayerState.FacingLeft:
                    DrawStanding(sb, SpriteEffects.FlipHorizontally);
                    break;
                case PlayerState.WalkingLeft:
                    DrawWalking(sb, SpriteEffects.FlipHorizontally);
                    break;
                default:
                    break;
            }
        }

        //draws character with given sprite and position
        public void DrawStanding(SpriteBatch sb, SpriteEffects flip)
        {
            //sb.Draw(texture, position, Color.White);
            sb.Draw(texture, position, new Rectangle(0, 0, 85, 211), Color.White, 0, Vector2.Zero, .5f, flip, 0);
        }

        public void DrawWalking(SpriteBatch sb, SpriteEffects flip)
        {
            sb.Draw(texture, position, new Rectangle(85 * frame, 0, 85, 211), Color.White, 0, Vector2.Zero, .5f, flip, 0);
        }

        /// <summary>
        /// Update frame animation for player, 
        /// method code by Erika Mesh
        /// </summary>
        /// <param name="time"></param>
        public void UpdateAnim(GameTime time)
        {
            this.time += time.ElapsedGameTime.TotalSeconds; //check time passed

            if (this.time >= timePerFrame)
            {
                frame++; //increase current frame

                if (frame > numOfFrames) //check walk cycle
                {
                    frame = 0; //if cycle over, reset
                }

                this.time -= timePerFrame;
            }
        }

        //center player in screen
        public void Center()
        {
            position.X = width / 2;
        }

        //put player on right side of screen to simulate walking into room from the right
        public void Right()
        {
            position.X = width;
        }

        //put player on left side of screen to simulate walking into room from the left
        public void Left()
        {
            position.X = 0;
        }
        #endregion
        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
