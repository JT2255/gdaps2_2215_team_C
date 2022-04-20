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
        private bool beenTalkedTo;
        private List<string> fiveDialogue;
        private List<string> preKnifeDialogue;
        private List<string> postKnifeDialogue;
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
        /// Returns whether an npc has been talked to or not
        /// </summary>
        public bool BeenTalkedTo
        {
            get { return beenTalkedTo; }
            set { beenTalkedTo = value; }
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
            beenTalkedTo = false;
            dialogueNum = 0;
            fiveDialogue = new List<string>();
            preKnifeDialogue = new List<string>();
            postKnifeDialogue = new List<string>();

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
        public void Speak(SpriteBatch sb, SpriteFont font, Texture2D dialogueBox, Player player,
            int hour, Item knife)
        {
            //draw dialogue box
            if (isTalking)
            {
                sb.Draw(dialogueBox, new Rectangle(20, 310, 760, 100), Color.White);
            }
            

            // refDialouge keeps track of which dialouge to use during speaking
            List<string> refDialouge = null ;

            // If Atkins hasn't died
            if (hour == 5)
            {
                refDialouge = fiveDialogue;
                // if there is still dialogue left in array, advance to next line
                if (dialogueNum < fiveDialogue.Count)
                {
                    sb.DrawString(font, $"{fiveDialogue[dialogueNum]}", new Vector2(50, 335), Color.Black);
                    sb.Draw(dialogueBox, new Rectangle(40, 270, 220, 40), Color.White);
                    sb.DrawString(font, $"{name}", new Vector2(45, 280), Color.Black);
                }
            }
            // If the knife has not been found and Atkins has died
            else if (hour > 5 && !player.Inventory.Contains(knife))
            {
                refDialouge = preKnifeDialogue;
                // if there is still dialogue left in array, advance to next line
                if (dialogueNum < preKnifeDialogue.Count)
                {
                    sb.DrawString(font, $"{preKnifeDialogue[dialogueNum]}", new Vector2(50, 335), Color.Black);
                    sb.Draw(dialogueBox, new Rectangle(40, 270, 220, 40), Color.White);
                    sb.DrawString(font, $"{name}", new Vector2(45, 280), Color.Black);
                }
            }
            else if (hour > 5 && player.Inventory.Contains(knife))
            {
                refDialouge = postKnifeDialogue;
                // if there is still dialogue left in array, advance to next line
                if (dialogueNum < postKnifeDialogue.Count)
                {
                    sb.DrawString(font, $"{postKnifeDialogue[dialogueNum]}", new Vector2(50, 335), Color.Black);
                    sb.Draw(dialogueBox, new Rectangle(40, 270, 220, 40), Color.White);
                    sb.DrawString(font, $"{name}", new Vector2(45, 280), Color.Black);
                }
            }
           
            // if there is no more dialouge
            if (dialogueNum == refDialouge.Count)
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
                string lineFromFile;

                //while theres still stuff, read in
                while ((lineFromFile = reader.ReadLine()) != null)
                {
                    // If the pre knife dialogue has been reached
                    if (lineFromFile == "0")
                    {
                        while ((lineFromFile = reader.ReadLine()) != null)
                        {
                            // If the post knife dialogue has been reached
                            if (lineFromFile == "1")
                            {
                                while ((lineFromFile = reader.ReadLine()) != null)
                                {
                                    // If we get to this point, add the dialogue to post knife
                                    postKnifeDialogue.Add(lineFromFile);
                                }
                            }
                            // If we get to this point, add the dialogue to pre knife
                            if (lineFromFile != null)
                            {
                                preKnifeDialogue.Add(lineFromFile);
                            }
                        }
                    }
                    // If we get to this point, add the dialogue to pre five dialogue
                    if (lineFromFile != null)
                    {
                        fiveDialogue.Add(lineFromFile);
                    }
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

        public void HoverName(SpriteBatch sb, SpriteFont font, Texture2D dialogueBox)
        {
            sb.Draw(dialogueBox, new Rectangle(position.X, position.Y - 45, 205, 30), Color.White);
            sb.DrawString(font, $"{name}", new Vector2(position.X, position.Y - 40), Color.Black);
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
            // Draw if they are not dead.
            if(!isDead)
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
