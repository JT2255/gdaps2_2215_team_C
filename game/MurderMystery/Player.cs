using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MurderMystery
{
    class Player
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private List<Item> inventory;
        private IDictionary<bool, string> clues;
        private int moveSpeed;
        private Rectangle position;
        private Texture2D texture;
        private KeyboardState kbState;
        private int height;
        private int width;
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties
        public Rectangle Position
        {
            get
            {
                return position;
            }
        }
        #endregion
        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        public Player(string name, Rectangle position, Texture2D texture, int height, int width)
        {
            this.name = name;
            this.position = position;
            this.texture = texture;
            this.width = width;
            this.height = height;
            moveSpeed = 5;
            inventory = new List<Item>();
            clues = new Dictionary<bool, string>();
        }
        #endregion
        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        //move character with A and D or left and right keys
        public void Move()
        {
            kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
            {
                position.X += moveSpeed;
            }

            if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
            {
                position.X -= moveSpeed;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        public void Center()
        {
            position.X = width / 2;
        }

        public void Right()
        {
            position.X = width;
        }

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
