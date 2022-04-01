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
    class NPC : GameObject
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private bool isMurderer;
        private bool isDead;
        private bool isTalking;
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
            : base (name, texture, position)
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
        /// Has the NPC write out their dialouge
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="font"></param>
        public void Speak(SpriteBatch sb, SpriteFont font)
        {
            // refDialouge keeps track of which dialouge to use during speaking
            string[] refDialouge;
            // Check if alive to set up dialouge
            if (!isDead) 
            {
                refDialouge = aliveDialogue;
            }
            else 
            {
                refDialouge = deadDialogue;
            }

            // if there is still dialogue left in array, advance to next line
            if (dialogueNum < refDialouge.Length)
            {
                sb.DrawString(font, $"{refDialouge[dialogueNum]}", new Vector2(position.X, position.Y + 100), Color.White);
            }
            // if there is no more dialouge
            else if (dialogueNum == refDialouge.Length)
            {
                // Stop talking
                isTalking = false;
                // Restart Dialouge num
                dialogueNum = 0;
            }
        }

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

        /// <summary>
        /// Draws the NPC
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
