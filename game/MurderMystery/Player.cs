using System;
using System.Collections.Generic;
using System.Text;

namespace MurderMystery
{
    class Player
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private List<Item> inventory;
        private IDictionary<bool, string> clues;
        private float moveSpeed;
        #endregion
        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties
        #endregion
        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        public Player(string name)
        {
            this.name = name;
            inventory = new List<Item>();
            clues = new Dictionary<bool, string>();
        }
        #endregion
        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        //move character with A and D or left and right keys
        public void Move()
        {
            
        }
        #endregion
        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
