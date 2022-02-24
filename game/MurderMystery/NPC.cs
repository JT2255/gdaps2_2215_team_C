using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace MurderMystery
{
    /// <summary>
    /// Represents a character in the game
    /// </summary>
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
        private StreamReader reader = null;
        #endregion

        // ~~~ PROPERTIES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Properties

        /// <summary>
        /// Returns the player's name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Returns whether the NPC is the murderer
        /// </summary>
        public bool IsMurderer
        {
            get { return isMurderer; }
        }

        /// <summary>
        /// Returns whether the NPC is dead
        /// </summary>
        public bool IsDead
        {
            get { return isDead; }
        }

        /// <summary>
        /// Returns whether the NPC is talking
        /// </summary>
        public bool IsTalking
        {
            get { return isTalking; }
            set { isTalking = value; }
        }

        /// <summary>
        /// Returns the NPC's current dialogue number
        /// and sets the curretn dialogueNum
        /// </summary>
        public int DialogueNum
        {
            get { return dialogueNum;  }
            set { dialogueNum = value; }
        }

        /// <summary>
        /// Returns the NPC's location
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }
        #endregion

        // ~~~ CONSTRUCTORS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors

        /// <summary>
        /// Builds an NPC
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isMurderer"></param>
        /// <param name="isDead"></param>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public NPC(string name, bool isMurderer, bool isDead, Rectangle position, Texture2D texture)
        {
            this.name = name;
            this.isMurderer = isMurderer;
            this.isDead = isDead;
            this.position = position;
            this.texture = texture;
            isTalking = false;
            dialogueNum = 0;
            //this.deadDialogue = deadDialogue;

            LoadDialogue();
        }
        #endregion

        // ~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods

        /// <summary>
        /// Has the NPC speak their lines
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="font"></param>
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

        /// <summary>
        /// Draws the NPC
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        //private bool Clicked(MouseState mState)
        //{
        //    if (mState.X > position.Left &&
        //        mState.X < position.Right &&
        //        mState.Y > position.Top &&
        //        mState.Y < position.Bottom &&
        //        SingleMousePress(mState))
        //    {
        //        return true;
        //    }

        //    if (mState.X > npc.Position.Left &&
        //        mState.X < npc.Position.Right &&
        //        mState.Y > npc.Position.Top &&
        //        mState.Y < npc.Position.Bottom)
        //    {
        //        Mouse.SetCursor(MouseCursor.Hand);
        //    }
        //    else
        //    {
        //        Mouse.SetCursor(MouseCursor.Arrow);
        //    }

        //    return false;
        //}

        /// <summary>
        /// Loads in the dialogue from a file
        /// </summary>
        private void LoadDialogue()
        {
            try
            {
                // Open file in data_files, file needs to be the samename as npc
                reader = new StreamReader($"../../../../../data_files/{name}.txt");
                string lineFromFile = reader.ReadLine();
                int index = 0;

                //initialize array with given length from file
                aliveDialogue = new string[int.Parse(lineFromFile)];

                //while theres still stuff, read in
                while ((lineFromFile = reader.ReadLine()) != null)
                {
                    aliveDialogue[index] = lineFromFile;
                    index++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // Attempts to close the file
            if (reader != null)
            {
                reader.Close();
            }


        }
        #endregion

        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
