using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ButtonDemo
{
    class Button
    {
        //Fields
        #region Fields
        private MouseState mState;
        private MouseState prevMState;

        private bool isHovering;
        private bool beenClicked;

        private SpriteFont font;
        private Texture2D texture;
        private Rectangle position;
        #endregion



        //Constructor
        public Button(Texture2D texture, SpriteFont font, Rectangle position)
        {
            this.texture = texture;
            this.font = font;

            this.position = position;

            //Set to false by default, updated in update
            isHovering = false;
            beenClicked = false;
        }



        //Properties
        #region Properties

        public bool BeenClicked
        {
            get
            {
                return beenClicked;
            }

            set
            {
                beenClicked = value;
            }
        }

        public Rectangle Position
        {
            get
            {
                return position;
            }
        }
        #endregion



        //Methods
        #region Methods
        /// <summary>
        /// Draw the box and the text, if any
        /// </summary>
        /// <param name="gametime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="text"></param>
        public void Draw(GameTime gametime, SpriteBatch spriteBatch, string text)
        {
            //The default colour for the button
            Color myColour = Color.White;

            //If the mouse is hovering over the button, draw it as a different colour
            if (isHovering)
            {
                myColour = Color.DarkRed;
            }

            //Draw the box
            spriteBatch.Draw(texture, Position, myColour);

            //if the text isn't null, write it
            if (text != null || text.Length != 0)
            {
                //Gets the x and y for the text, measure by their size
                float x = (Position.X + (Position.Width / 2)) - (font.MeasureString(text).X / 2);
                float y = (Position.X + (Position.Height / 2)) - (font.MeasureString(text).Y / 2);

                spriteBatch.DrawString(font, text, new Vector2(x, y), Color.White);
            }
        }

        /// <summary>
        /// Gets the mouse position to see if it is hovering, as well as if it clicks
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            mState = Mouse.GetState();

            //Gets the current position of the mouse to be used for its hitbox
            Rectangle mouseHitBox = new Rectangle(mState.X, mState.Y, 1, 1);

            //Changes the colour to red
            //Works!
            if (mouseHitBox.Intersects(Position))
            {
                isHovering = true;
            }
            else
            {
                isHovering = false;
            }

            //Changes been clicked to be true on click
            //Working
            if (mState.LeftButton == ButtonState.Pressed
                    && prevMState.LeftButton == ButtonState.Released && isHovering)
            {
                beenClicked = true;

            }
            else
            {
                beenClicked = false;
            }

            prevMState = mState;
        }
        #endregion

    }
}
