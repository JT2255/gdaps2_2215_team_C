using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MurderMystery
{
    /// <summary>
    /// Tracks the current player movement state
    /// </summary>
    
    //Enum for Player's Movement
    enum PlayerMovementState
    {
        FacingLeft,
        FacingRight,
        WalkingLeft,
        WalkingRight
    }

    /// <summary>
    /// Represents a controllable character
    /// </summary>
    class Player
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private List<Item> inventory;
        // private IDictionary<bool, string> clues;
        private int moveSpeed;
        private Vector2 position;
        private Texture2D texture;
        private int windowHeight;
        private int windowWidth;
        private double fps;
        private double time;
        private int frame;
        private double timePerFrame;
        private int numOfFrames;
        private PlayerMovementState movementState;
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties
        
        /// <summary>
        /// Returns the player's position
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public List<Item> Inventory
        {
            get { return inventory; }
            set
            {
                inventory = value;
            }
        }

        //items[x]
        //public Item this[int i] 
        //{
        //    set { inventory[i] = value; }
        //}
        #endregion
        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors

        /// <summary>
        /// Builds a player object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public Player(string name, Vector2 position, Texture2D texture, int height, int width)
        {
            this.name = name;
            this.position = position;
            this.texture = texture;
            this.windowWidth = width;
            this.windowHeight = height;
            moveSpeed = 5;
            inventory = new List<Item>();
            // clues = new Dictionary<bool, string>();
            fps = 6;
            timePerFrame = 1 / fps;
            numOfFrames = 6;
            movementState = PlayerMovementState.FacingLeft;
        }
        #endregion
        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods

        //CODE FOR ANIMATING THE PLAYER 
        #region Animation Code
        /// <summary>
        /// Animates the player in the Game Loop.
        /// </summary>
        /// <param name="kbState"></param>
        public void Move(KeyboardState kbState, Rooms currentRoom)
        {
            switch (movementState)
            {
                //when facing right, able to walk right or look back left
                case PlayerMovementState.FacingRight:
                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        movementState = PlayerMovementState.WalkingRight;
                    }

                    if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                    {
                        movementState = PlayerMovementState.FacingLeft;
                    }
                    break;
                //when walking right, move character
                case PlayerMovementState.WalkingRight:                   
                    position.X += moveSpeed;

                    if (kbState.IsKeyUp(Keys.Right) && kbState.IsKeyUp(Keys.D))
                    {
                        movementState = PlayerMovementState.FacingRight;
                    }
                    break;
                //when facing left, able to walk left or look back right
                case PlayerMovementState.FacingLeft:
                    if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                    {
                        movementState = PlayerMovementState.WalkingLeft;
                    }

                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        movementState = PlayerMovementState.FacingRight;
                    }
                    break;
                //when walking left, move character
                case PlayerMovementState.WalkingLeft:
                    position.X -= moveSpeed;

                    if (kbState.IsKeyUp(Keys.Left) && kbState.IsKeyUp(Keys.A))
                    {
                        movementState = PlayerMovementState.FacingLeft;
                    }
                    break;
                default:
                    break;
            }       
        }

        /// <summary>
        /// Draw player depending on current movement state
        /// </summary>
        /// <param name="sb">sprite batch</param>
        public void Draw(SpriteBatch sb)
        {
            switch (movementState)
            {
                // Facing right
                case (PlayerMovementState.FacingRight):
                    DrawStanding(sb, SpriteEffects.None);
                    break;
                // Walking right
                case (PlayerMovementState.WalkingRight):
                    DrawWalking(sb, SpriteEffects.None);
                    break;
                // Facing left
                case PlayerMovementState.FacingLeft:
                    DrawStanding(sb, SpriteEffects.FlipHorizontally);
                    break;
                // Walking left
                case PlayerMovementState.WalkingLeft:
                    DrawWalking(sb, SpriteEffects.FlipHorizontally);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the player standing
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="possibleFlip"></param>
        public void DrawStanding(SpriteBatch sb, SpriteEffects possibleFlip)
        {
            sb.Draw(texture, position, new Rectangle(0, 0, 90, 325), Color.White, 0, Vector2.Zero, .5f, possibleFlip, 0);
        }

        /// <summary>
        /// Draws the player walking
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="possibleFlip"></param>
        public void DrawWalking(SpriteBatch sb, SpriteEffects possibleFlip)
        {
            sb.Draw(texture, position, new Rectangle(90 * frame, 0, 90, 325), Color.White, 0, Vector2.Zero, .5f, possibleFlip, 0);
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
        #endregion

        //CODE FOR POSITIONING THE PLAYER
        #region Positioning Code
        /// <summary>
        /// Centers the player on screen
        /// </summary>
        public void Center()
        {
            position.X = windowWidth / 2;
        }

        /// <summary>
        /// Places the player on the right side of the screen
        /// </summary>
        public void Right()
        {
            position.X = windowWidth;
        }

        /// <summary>
        /// Places the player on the left side of the screen
        /// </summary>
        public void Left()
        {
            position.X = 0;
        }
        #endregion

        public void DrawMap(SpriteBatch sb, Texture2D map1, Texture2D map2, Texture2D map3,
            Texture2D map4, Texture2D map5, Texture2D map7, Texture2D exclamation, Rooms room, int hour)
        {
            switch (room)
            {
                case Rooms.Room1:
                    sb.Draw(map1, new Rectangle(windowWidth - 100, windowHeight - 110, 100, 100), Color.White);                   
                    break;
                case Rooms.Room2:
                    sb.Draw(map2, new Rectangle(windowWidth - 100, windowHeight - 110, 100, 100), Color.White);
                    break;
                case Rooms.Room3:
                    sb.Draw(map3, new Rectangle(windowWidth - 100, windowHeight - 110, 100, 100), Color.White);
                    break;
                case Rooms.Room4:
                    sb.Draw(map4, new Rectangle(windowWidth - 100, windowHeight - 110, 100, 100), Color.White);
                    break;
                case Rooms.Room5:
                    sb.Draw(map5, new Rectangle(windowWidth - 100, windowHeight - 110, 100, 100), Color.White);
                    break;
                case Rooms.Room7:
                    sb.Draw(map7, new Rectangle(windowWidth - 100, windowHeight - 110, 100, 100), Color.White);
                    break;
                default:
                    break;
            }

            switch (hour)
            {
                case 7:
                    sb.Draw(exclamation, new Rectangle(771, 450, 3, 16), Color.White);
                    break;
                case 8:
                    sb.Draw(exclamation, new Rectangle(741, 450, 3, 16), Color.White);
                    break;
                case 9:
                    sb.Draw(exclamation, new Rectangle(711, 420, 3, 16), Color.White);
                    break;
                case 10:
                    sb.Draw(exclamation, new Rectangle(711, 420, 3, 16), Color.White);
                    break;
                case 11:
                    sb.Draw(exclamation, new Rectangle(711, 450, 3, 16), Color.White);
                    break;
                default:
                    break;
            }
        }

        #endregion

        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
