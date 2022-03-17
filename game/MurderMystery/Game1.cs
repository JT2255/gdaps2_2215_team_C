using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MurderMystery
{
    // Enumeration for the Game's State
    enum State
    {
        MainMenu,
        Game,
        Inventory,
        PauseMenu,
        EndMenu
    }

    // Enumeration for the Room
    enum Rooms
    {
        Room1,
        Room2,
        Room3,
        Room4
    }

    public class Game1 : Game
    {
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Controller States
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private MouseState mState;
        private MouseState prevMState;

        //Enums
        private State currentState;
        private Rooms currentRoom;

        //Objects
        private SpriteFont font;
        private Player player;
        private NPC testNPC;
       // private Button playButton;

        //Configs
        private int windowWidth;
        private int windowHeight;
        private Vector2 playerPos;

        //Textures
        private Texture2D playerTexture;
        private Texture2D testNPCTexture;
        private Texture2D menuButton;

        //Misc
        private StreamReader reader = null;
        private List<Item> items;
        private List<Rectangle> itemInvPos;
        #endregion

        // ~~~ GAME LOOP STUFF ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Typical Game1 Methods: Initialize, LoadContent, Update, and Draw
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Game starts at the main menu by default
            currentState = State.MainMenu;
            // Game starts in the first room by default
            currentRoom = Rooms.Room1;

            // Initialize window
            windowHeight = _graphics.PreferredBackBufferHeight;
            windowWidth = _graphics.PreferredBackBufferWidth;

            // Position of character
            playerPos = new Vector2(windowWidth / 2, windowHeight - 200);

            items = new List<Item>();
            itemInvPos = new List<Rectangle>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Textures
            playerTexture = Content.Load<Texture2D>("ElizabethSprite");
            testNPCTexture = Content.Load<Texture2D>("npc");
            menuButton = Content.Load<Texture2D>("MenuBox");

            // Load Fonts
            font = Content.Load<SpriteFont>("font");

            // Initialize Objects
            player = new Player("Char", playerPos, playerTexture, windowHeight, windowWidth);
            testNPC = new NPC("test 1", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            #region Button Initialization

            #endregion

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
            switch (currentState)
            {
                case State.MainMenu:
                    ProcessMainMenu(kbState);
                    break;
                case State.Game:
                    ProcessGame(kbState);
                    break;
                case State.Inventory:
                    ProcessInventory(kbState);
                    break;
                case State.EndMenu:
                    break;
                case State.PauseMenu:
                    ProcessPauseMenu(kbState);
                    break;
                default:
                    break;
            }

            // Retrieve the previous states
            prevKbState = kbState;
            prevMState = mState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            switch (currentState)
            {
                case State.MainMenu:
                    // Main Menu text
                    _spriteBatch.DrawString(font, "Main Menu\nPress P to go to play state.", new Vector2(0, 0), Color.White);
                    // Main Menu Background
                    GraphicsDevice.Clear(Color.Red);
                    break;

                case State.Game:
                    // Enter Debug Text
                    _spriteBatch.DrawString(font, $"You are now playing the game.\nPress M to go back\nor I to go to inventory." +
                                $"\n\n{currentRoom}", new Vector2(0, 0), Color.White);
                    // When in game state, check for what room state you are in
                    switch (currentRoom)
                    {
                        case Rooms.Room1:
                            GraphicsDevice.Clear(Color.Navy);
                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room2:

                            GraphicsDevice.Clear(Color.DarkOliveGreen);
                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room3:

                            _spriteBatch.DrawString(font, $"\n\n\n\n{currentRoom}\n#{testNPC.DialogueNum} Talking:{testNPC.IsTalking}", new Vector2(0, 0), Color.White);

                            GraphicsDevice.Clear(Color.Gray);
                            player.Draw(_spriteBatch);
                            testNPC.Draw(_spriteBatch);

                            //if currently talking, draw dialogue
                            if (testNPC.IsTalking)
                            {
                                testNPC.Speak(_spriteBatch, font);
                            }

                            // If the knife is not picked up, draw it
                            if (!items[0].PickedUp)
                            {
                                items[0].Draw(_spriteBatch);
                            }

                            break;
                        default:
                            break;
                    }

                    break;

                case State.Inventory:
                    _spriteBatch.DrawString(font, "You are now in the inventory.\nPress I to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Green);
                    // Draws the inventory items
                    // Display Inventory -----
                    // Should format the inventory into a small arrangement of Item objects
                    for(int i = 0; i < player.Inventory.Count; i++) 
                    {
                        // Label what to use
                        Item thisItem = player.Inventory[i]; // Item of that iteration
                        Vector2 startPoint = new Vector2(40,40); // Start Point for displaying items
                        int offset = 5;
                        // Final position is the startPoint + the offset in the Y-coord
                        Vector2 formatPos = startPoint + new Vector2(0, i * offset);
                        // Draw the Item and write the description.
                        thisItem.Draw(_spriteBatch, formatPos);
                        _spriteBatch.DrawString(font,
                            thisItem.ToString(),
                            formatPos + new Vector2(0,thisItem.Position.Height),
                            Color.White);
                    }
                    break;
                case State.PauseMenu:
                    _spriteBatch.DrawString(font, "You are now paused.\nPress ESC to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Cyan);
                    break;
                default:
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        // ~~~ INPUT CAPTURES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Input Captures
        /// <summary>
        /// Checks for a single key press.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="currentKbState"></param>
        /// <returns></returns>
        private bool SingleKeyPress(Keys key, KeyboardState currentKbState)
        {
            if (currentKbState.IsKeyDown(key) && prevKbState.IsKeyUp(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for a single left mouse click
        /// </summary>
        /// <param name="currentMState"></param>
        /// <returns></returns>
        private bool SingleMousePress(MouseState currentMState)
        {
            if (currentMState.LeftButton == ButtonState.Pressed &&
                prevMState.LeftButton == ButtonState.Released)
            {
                return true;
            }

            return false;
        }

        //TODO: Find a way to implement Clicked() better.
        /// <summary>
        /// If mouse is held down inside of npc, return true
        /// </summary>
        /// <param name="npc">npc object</param>
        /// <returns>returns true or false depending on if mouse is clicking on object</returns>
        private bool Clicked(NPC npc)
        {
            // If mouse intersects the obj
            if (mState.X > npc.Position.Left &&
                mState.X < npc.Position.Right &&
                mState.Y > npc.Position.Top &&
                mState.Y < npc.Position.Bottom)
            {
                // Set it to hover
                Mouse.SetCursor(MouseCursor.Hand);
                // if mouse is clicked, would return true,
                // else would return false
                return SingleMousePress(mState);
            }
            else // Mouse is not intersecting obj
            {
                Mouse.SetCursor(MouseCursor.Arrow);
                return false;
            }
        }

        /// <summary>
        /// If mouse is held down inside of an item, return true
        /// </summary>
        /// <param name="item">item object</param>
        /// <returns>returns true or false depending on if mouse is clicking on object</returns>
        private bool Clicked(Item item)
        {
            // Hover over item 
            if (mState.X > item.Position.Left &&
                mState.X < item.Position.Right &&
                mState.Y > item.Position.Top &&
                mState.Y < item.Position.Bottom)
            {
                Mouse.SetCursor(MouseCursor.Hand);
                // Item Clicked
                if (SingleMousePress(mState)) 
                {
                    return true;
                }
            }
            else
            {
                // No hover
                Mouse.SetCursor(MouseCursor.Arrow);
            }
            // False if not specifically clicked
            return false;
        }
        #endregion

        // ~~~ GAME LOGIC ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Game Logic

        /// <summary>
        /// If the player inputs the "P" key, take them to the game
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessMainMenu(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.P, kbState))
            {
                currentState = State.Game;
            }
        }

        /// <summary>
        /// If the player inputs the "M" key, take them to the main menu
        /// If the player inputs the "I" key, take them to the inventory
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessGame(KeyboardState kbState)
        {
            // Switch States
            if (SingleKeyPress(Keys.M, kbState))
            {
                currentState = State.MainMenu;
            }

            if (SingleKeyPress(Keys.I, kbState))
            {
                currentState = State.Inventory;
            }

            if (SingleKeyPress(Keys.Escape, kbState))
            {
                currentState = State.PauseMenu;
            }

            // Let Player Move
            player.Move(kbState);

            // Talking to NPCs
            // On spacebar press, advance dialogue
            if (testNPC.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                testNPC.DialogueNum++;
            }

            // Simulate room movement 
            switch (currentRoom)
            {
                case Rooms.Room1:
                    //if you walk to the left, go to room 2
                    if (player.Position.X < 0)
                    {
                        // Change Room
                        currentRoom = Rooms.Room2;
                        // Re-Orient Player
                        player.Right();
                    }

                    //if you walk to the right, go to room 3
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room3;
                        player.Left();
                    }
                    break;
                case Rooms.Room2:
                    //if you walk right, go back to room 1
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room1;
                        player.Left();
                    }
                    break;
                case Rooms.Room3:
                    //if you walk left, go back to room 1
                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room1;
                        player.Right();
                    }
                    // ~~~ GAME OBJECTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    #region Game Objects
                    //if you click on him, toggle dialogue
                    if (Clicked(testNPC))
                    {
                        //if (testNPC.IsTalking)
                        //{
                        //    testNPC.IsTalking = false;
                        //}
                        //else if (!testNPC.IsTalking)
                        //{
                        //    testNPC.IsTalking = true;
                        //}
                        // Change to
                        testNPC.IsTalking = !testNPC.IsTalking;
                    }

                    // If you click on the knife, toggle it
                    // Possible implementation:
                    /*  Create a local list of items for that given room
                     *  Loop into that list to check if the item has been clicked
                     *  IF clicked, set PickedUp to true, and then add it to the inventory.
                     */
                    if (Clicked(items[0]))
                    {
                        items[0].PickedUp = true;
                        player.Inventory.Add(items[0]);
                    }
                    #endregion
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
                currentState = State.Game;
            }
        }

        /// <summary>
        /// If the player inputs the "Escape" key, take them to the game
        /// If the player inputs the "M" key, take them back to the main menu
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessPauseMenu(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.Escape, kbState))
            {
                currentState = State.Game;
            }

            if (SingleKeyPress(Keys.M, kbState))
            {
                currentState = State.MainMenu;
            }
        }
        #endregion

        /// <summary>
        /// Loads in items from item text file
        /// </summary>
        private void LoadItems()
        {
            try
            {
                // Open the file
                reader = new StreamReader("../../../../../data_files/items.txt");

                string line;

                // While there are still items to read in, keep reading
                // Also set line into that specific item
                while ((line = reader.ReadLine()) != null)
                {
                    string[] splitData = line.Split(',');

                    //add new item to item list
                    items.Add(new Item(
                        splitData[0], //name
                        splitData[1], //description
                        int.Parse(splitData[2]), //id
                        Content.Load<Texture2D>(splitData[3]), //texture
                        new Rectangle( //position
                            int.Parse(splitData[4]), // x-cord
                            int.Parse(splitData[5]), // y-cord
                            int.Parse(splitData[6]), // width
                            int.Parse(splitData[7])))); // height
                }

                // Close the file
                if (reader != null) 
                {
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("File not loaded correctly: " + e.Message);
            }
        }
    }
}