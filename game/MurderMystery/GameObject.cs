using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MurderMystery
{
    abstract class GameObject
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region
        protected string name;
        protected Texture2D texture;
        protected Rectangle position;
        #endregion

        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region
        protected GameObject(string name, Texture2D texture, Rectangle position)
        {
            this.name = name;
            this.texture = texture;
            this.position = position;
        }
        #endregion

        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region
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
        #endregion
    }
}
