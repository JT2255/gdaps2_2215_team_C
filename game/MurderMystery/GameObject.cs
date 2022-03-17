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
        public abstract void Draw(SpriteBatch sb);

        public abstract void Update();
        #endregion
    }
}
