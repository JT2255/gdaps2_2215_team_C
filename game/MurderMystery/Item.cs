using System;
using System.Collections.Generic;
using System.Text;

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
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties
        #endregion
        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        public Item(string name, string description, int id)
        {
            this.name = name;
            this.description = description;
            this.id = id;
        }
        #endregion
        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
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
