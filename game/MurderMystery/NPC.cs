using System;
using System.Collections.Generic;
using System.Text;

namespace MurderMystery
{
    class NPC
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private bool isMurderer;
        private bool isDead;
        private string[] aliveDialogue;
        private string[] deadDialogue;
        #endregion

        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties
        public string Name
        {
            get { return name; }
        }

        public bool IsMurderer
        {
            get { return isMurderer; }
        }

        public bool IsDead
        {
            get { return isDead; }
        }
        #endregion

        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        public NPC(string name, bool isMurderer, bool isDead)
        {
            this.name = name;
            this.isMurderer = isMurderer;
            this.isDead = isDead;
            this.aliveDialogue = aliveDialogue;
            this.deadDialogue = deadDialogue;
        }
        #endregion

        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        public void Speak()
        {

        }
        #endregion

        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
