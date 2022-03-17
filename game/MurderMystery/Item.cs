using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MurderMystery
{
    /// <summary>
    /// Represents an interactable object
    /// </summary>
    class Item : GameObject
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string description;
        private int id;
        private bool pickedUp;
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties

        /// <summary>
        /// Returns the item's location
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Returns whether the item has been picked up
        /// </summary>
        public bool PickedUp
        {
            get { return pickedUp; }
            set { pickedUp = value; }
        }

        #endregion
        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors

        /// <summary>
        /// Builds an item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="spriteName"></param>
        public Item(string name, string description, int id, Texture2D texture, Rectangle position)
            : base (name, texture, position)
        {
            this.name = name;
            this.description = description;
            this.id = id;
            this.texture = texture;
            this.position = position;
        }
        #endregion
        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods

        /// <summary>
        /// Draws the Item given a Vector2 for new position. Maintains the size of the object.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
        public void Draw(SpriteBatch sb, Vector2 position)
        {
            sb.Draw(texture,
                new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height),
                Color.White);
        }

        #endregion
        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides

        /// <summary>
        /// Overrides the ToString method and returns
        /// the name and description of the item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name + ": " + description;
        }

        /// <summary>
        /// Draws the Item's sprite.
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        /// <summary>
        /// Updates the object
        /// </summary>
        public override void Update()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
