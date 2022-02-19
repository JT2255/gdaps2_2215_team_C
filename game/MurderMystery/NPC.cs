using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MurderMystery
{
    class NPC
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private string name;
        private bool isMurderer;
        private bool isDead;
        private Rectangle position;
        private Texture2D texture;
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

        public Rectangle Position
        {
            get
            {
                return position;
            }
        }
        #endregion

        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        public NPC(string name, bool isMurderer, bool isDead, Rectangle position, Texture2D texture)
        {
            this.name = name;
            this.isMurderer = isMurderer;
            this.isDead = isDead;
            this.position = position;
            this.texture = texture;
            //this.aliveDialogue = aliveDialogue;
            //this.deadDialogue = deadDialogue;
        }
        #endregion

        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        public void Speak(SpriteBatch sb, SpriteFont font)
        {
            //draw string of dialogue
            sb.DrawString(font, "stop clicking me :(", new Vector2(position.X, position.Y + 100), Color.White);
        }

        public void Draw(SpriteBatch sb)
        {
            //draw npc
            sb.Draw(texture, position, Color.White);
        }
        #endregion

        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
