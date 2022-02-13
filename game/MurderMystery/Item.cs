using System;
using System.Collections.Generic;
using System.Text;

namespace MurderMystery
{
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
        //override to string to say what each item is.
        //Individual items will have a longer description describing their purpose.
        //Ex. This is a rope. The ends are frayed and it looks quite worn out.
        public override string ToString()
        {
            return "This is a " + name + ".\n" + description;
        }
        #endregion
    }
}
