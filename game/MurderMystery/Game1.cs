﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace MurderMystery
{
    enum CurrentState
    {
        MainMenu,
        Game,
        Inventory,
        PauseMenu,
        EndMenu
    }

    enum CurrentRoom
    {
        Room1,
        Room2,
        Room3,
        Room4
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Controller States
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private MouseState mState;
        private MouseState prevMState;

        //Enums
        private CurrentState gameState;
        private CurrentRoom room;

        //Objects
        private SpriteFont font;
        private Player player;
        private NPC testNPC;

        //Configs
        private int windowWidth;
        private int windowHeight;
        private Vector2 playerPos;

        //Textures
        private Texture2D playerTexture;
        private Texture2D testNPCTexture;

        //Misc
        private StreamReader reader = null;
        private List<Item> items;
         
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Game starts at the main menu by default
            gameState = CurrentState.MainMenu;
            // Game starts in the first room by default
            room = CurrentRoom.Room1;

            // Initialize window
            windowHeight = _graphics.PreferredBackBufferHeight;
            windowWidth = _graphics.PreferredBackBufferWidth;

            // Position of character
            playerPos = new Vector2(windowWidth / 2, windowHeight - 200);

            items = new List<Item>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load Textures
            playerTexture = Content.Load<Texture2D>("ElizabethSprite");
            testNPCTexture = Content.Load<Texture2D>("npc");

            // Load Fonts
            font = Content.Load<SpriteFont>("font");

            // Initialize Objects
            player = new Player("Char", playerPos, playerTexture, windowHeight, windowWidth);
            testNPC = new NPC("test 1", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);

            // Load In Items
            LoadItems();
        }

        protected override void Update(GameTime gameTime)
        {
           // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
          //      Exit();

            // Sets up our states
            kbState = Keyboard.GetState();
            mState = Mouse.GetState();

            // Updates the player animation
            player.UpdateAnim(gameTime);

            // Processes the proper game logic
            switch (gameState)
            {
                case CurrentState.MainMenu:
                    ProcessMainMenu(kbState);
                    break;
                case CurrentState.Game:
                    ProcessGame(kbState);
                    break;
                case CurrentState.Inventory:
                    ProcessInventory(kbState);
                    break;
                case CurrentState.EndMenu:
                    break;
                case CurrentState.PauseMenu:
                    ProcessPauseMenu(kbState);
                    break;
                default:
                    break;
            }

            prevKbState = kbState;
            prevMState = mState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            switch (gameState)
            {
                case CurrentState.MainMenu:
                    _spriteBatch.DrawString(font, "Main Menu\nPress P to go to play state.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Red);
                    break;

                case CurrentState.Game:

                    //when in game state, check for what room state you are in
                    switch (room)
                    {
                        case CurrentRoom.Room1:

                            _spriteBatch.DrawString(font, $"You are now playing the game.\nPress M to go back\nor I to go to inventory." +
                                $"\n\n{room}", new Vector2(0, 0), Color.White);

                            GraphicsDevice.Clear(Color.Navy);
                            player.Draw(_spriteBatch);

                            break;
                        case CurrentRoom.Room2:

                            _spriteBatch.DrawString(font, $"You are now playing the game.\nPress M to go back\nor I to go to inventory." +
                                $"\n\n{room}", new Vector2(0, 0), Color.White);

                            GraphicsDevice.Clear(Color.DarkOliveGreen);
                            player.Draw(_spriteBatch);

                            break;
                        case CurrentRoom.Room3:

                            _spriteBatch.DrawString(font, $"You are now playing the game.\nPress M to go back\nor I to go to inventory." +
                                $"\n\n{room}\n{testNPC.DialogueNum}  {testNPC.IsTalking}", new Vector2(0, 0), Color.White);

                            GraphicsDevice.Clear(Color.Gray);
                            player.Draw(_spriteBatch);
                            testNPC.Draw(_spriteBatch);

                            //if currently talking, draw dialogue
                            if (testNPC.IsTalking)
                            {
                                testNPC.Speak(_spriteBatch, font);                     
                            }

                            break;
                        default:
                            break;
                    }
                    
                    break;

                case CurrentState.Inventory:
                    _spriteBatch.DrawString(font, "You are now in the inventory.\nPress I to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Green);
                    break;
                case CurrentState.PauseMenu:
                    _spriteBatch.DrawString(font, "You are now paused.\nPress ESC to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Cyan);
                    break;
                default:
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        ///check for single key press helper method
        private bool SingleKeyPress(Keys key, KeyboardState currentKbState)
        {
            if(currentKbState.IsKeyDown(key) && prevKbState.IsKeyUp(key))
            {
                return true;
            }

            return false;
        }

        //check for single mouse press
        private bool SingleMousePress(MouseState currentMState)
        {
            if (currentMState.LeftButton == ButtonState.Pressed &&
                prevMState.LeftButton == ButtonState.Released)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// If the player inputs the "P" key, take them to the game
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessMainMenu(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.P, kbState))
            {
                gameState = CurrentState.Game;
            }
        }

        /// <summary>
        /// If the player inputs the "M" key, take them to the main menu
        /// If the player inputs the "I" key, take them to the inventory
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessGame(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.M, kbState))
            {
                gameState = CurrentState.MainMenu;
            }

            if (SingleKeyPress(Keys.I, kbState))
            {
                gameState = CurrentState.Inventory;
            }

            if (SingleKeyPress(Keys.Escape, kbState))
            {
                gameState = CurrentState.PauseMenu;
            }

            //on spacebar press, advance dialogue
            if (testNPC.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                testNPC.DialogueNum++;                     
            }

            //if you click on him, toggle dialogue
            if (Clicked(testNPC))
            {
                if (testNPC.IsTalking)
                {
                    testNPC.IsTalking = false;
                }
                else if (!testNPC.IsTalking)
                {
                    testNPC.IsTalking = true;
                }
            }

            // Simulate room movement 
            switch (room)
            {
                case CurrentRoom.Room1:
                    //if you walk to the left, go to room 2
                    if (player.Position.X < 0)
                    {
                        room = CurrentRoom.Room2;
                        player.Right();
                    }

                    //if you walk to the right, go to room 3
                    if (player.Position.X > windowWidth)
                    {
                        room = CurrentRoom.Room3;
                        player.Left();
                    }
                    player.Move(kbState);
                    break;
                case CurrentRoom.Room2:
                    //if you walk right, go back to room 1
                    if (player.Position.X > windowWidth)
                    {
                        room = CurrentRoom.Room1;
                        player.Left();
                    }                
                    player.Move(kbState);
                    break;
                case CurrentRoom.Room3:
                    //if you walk left, go back to room 1
                    if (player.Position.X < 0)
                    {
                        room = CurrentRoom.Room1;
                        player.Right();
                    }
                    player.Move(kbState);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// If the player inputs the "I" key, take them back to the game
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessInventory(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.I, kbState))
            {
                gameState = CurrentState.Game;
            }
        }

        /// <summary>
        /// If the player inputs the "Escape" key, take them to the game
        /// If the player inputs the "M" key, take them back to the main menu
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessPauseMenu(KeyboardState kbState)
        {
            if(SingleKeyPress(Keys.Escape, kbState))
            {
                gameState = CurrentState.Game;
            }

            if(SingleKeyPress(Keys.M, kbState))
            {
                gameState = CurrentState.MainMenu;
            }
        }

        /// <summary>
        /// If mouse is held down inside of npc, return true
        /// </summary>
        /// <param name="npc">npc object</param>
        /// <returns>returns true or false depending on if mouse is clicking on object</returns>
        private bool Clicked(NPC npc)
        {
            if (mState.X > npc.Position.Left &&
                mState.X < npc.Position.Right &&
                mState.Y > npc.Position.Top &&
                mState.Y < npc.Position.Bottom &&
                SingleMousePress(mState))
            {
                return true;
            }

            if (mState.X > npc.Position.Left &&
                mState.X < npc.Position.Right &&
                mState.Y > npc.Position.Top &&
                mState.Y < npc.Position.Bottom)
            {
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);  
            }

            return false;
        }

        /// <summary>
        /// Loads in items from item text file
        /// </summary>
        private void LoadItems()
        {
            try
            {
                //get item file
                reader = new StreamReader("../../../../../data_files/items.txt");

                string line;

                //while there are still items to read in, keep reading
                while ((line = reader.ReadLine()) != null)
                {
                    string[] splitData = line.Split(',');

                    //add new item to item list
                    items.Add(new Item(splitData[0], splitData[1], int.Parse(splitData[2])));
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
