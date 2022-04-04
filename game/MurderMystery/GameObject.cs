using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MurderMystery
{
    abstract class GameObject
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        protected string name;
        protected Texture2D texture;
        protected Rectangle position;
        protected bool beingDrawn;
        #endregion

        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties

        public Rectangle Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public bool BeingDrawn
        {
            get { return this.beingDrawn; }
            set { this.beingDrawn = value; }
        }

        #endregion

        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        protected GameObject(string name, Texture2D texture, Rectangle position)
        {
            this.name = name;
            this.texture = texture;
            this.position = position;
            beingDrawn = false;
        }
        #endregion

        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        /// <summary>
        /// Draw for most classes
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        /// <summary>
        /// Draw method for any object with text
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        public virtual void Draw(SpriteBatch sb, string text)
        {
            sb.Draw(texture, position, Color.White);
        }

        /// <summary>
        /// Update logic method
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Determines whether the mouse is hovering over
        /// the object
        /// </summary>
        /// <param name="mState"></param>
        /// <returns></returns>
        public bool Hover(MouseState mState)
        {
            // If mouse intersects the obj
            if (mState.X > this.Position.Left &&
                mState.X < this.Position.Right &&
                mState.Y > this.Position.Top &&
                mState.Y < this.Position.Bottom &&
                this.BeingDrawn)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
