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
    class Item
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private string description;
        private int id;
        private bool pickedUp;
        private Texture2D texture;
        private Rectangle position;
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties

        /// <summary>
        /// Returns the item's location
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
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

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
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
        #endregion
    }
}
