using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;

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
        private StreamReader reader = null;
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

            try
            {
                //open file in data_files, file needs to be samename as npc
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

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


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
        #endregion

        // ~~~ OVERRIDES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Overrides
        #endregion
    }
}
