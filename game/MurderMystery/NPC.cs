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
        private bool isTalking;
        private Rectangle position;
        private Texture2D texture;
        private string[] aliveDialogue;
        private string[] deadDialogue;
        private int dialogueNum;
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

        public bool IsTalking
        {
            get { return isTalking; }
            set { isTalking = value; }
        }

        public int DialogueNum
        {
            get { return dialogueNum;  }
            set { dialogueNum = value; }
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
            isTalking = false;
            dialogueNum = 0;
            this.aliveDialogue = new string[3] {"First line", "Second Line", "Third Line"};
            //this.deadDialogue = deadDialogue;
        }
        #endregion

        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        public void Speak(SpriteBatch sb, SpriteFont font)
        { 
            //if there is still dialogue left in array, advance to next line
            if (dialogueNum < aliveDialogue.Length)
            {
                sb.DrawString(font, $"{aliveDialogue[dialogueNum]}", new Vector2(position.X, position.Y + 100), Color.White);
            } 
            
            //if no more dialogue, stop talking
            if (dialogueNum == aliveDialogue.Length)
            {
                isTalking = false;
            }
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
