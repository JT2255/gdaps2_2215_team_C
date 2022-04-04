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
        Instructions,
        Game,
        Inventory,
        PauseMenu,
        EndMenu,
        ExitGame
    }

    // Enumeration for the Room
    enum Rooms
    {
        Room1,
        Room2,
        Room3,
        Room4,
        Room5,
        Room6
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

        #region NPCs
        private NPC NPC1;
        private NPC NPC2;
        private NPC NPC3;
        private NPC NPC4;
        private NPC NPC5;
        private NPC NPC6;
        #endregion

        #region Buttons
        private Button topButton;
        private Button pauseButton;
        private Button bottomButton;
        private Button inventoryButton;
        private Button exitButton;
        private Button testStairsButton;
        #endregion

        //Configs
        private int windowWidth;
        private int windowHeight;
        private Vector2 playerPos;

        //Textures
        private Texture2D playerTexture;
        private Texture2D testNPCTexture;
        #region Buttons
        private Texture2D menuButtonTexture;
        private Texture2D pauseTexture;
        private Texture2D inventoryTexture;
        private Texture2D exitTexture;
        private Texture2D testStairs;
        #endregion

        //Misc
        private StreamReader reader = null;
        private List<Item> items;
        private double totalTime;
        private double currentTime;
        private int hour;
        private List<Rectangle> itemInvPos;
        private List<GameObject> gameObjects;
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
            // Initialize window
            windowHeight = _graphics.PreferredBackBufferHeight;
            windowWidth = _graphics.PreferredBackBufferWidth;

            // Game starts at the main menu by default
            currentState = State.MainMenu;
            // Game starts in the first room by default
            currentRoom = Rooms.Room1;

            // Position of character
            playerPos = new Vector2(windowWidth / 2, windowHeight - 200);

            items = new List<Item>();
            itemInvPos = new List<Rectangle>();
            //time per in-game hour
            totalTime = 120;
            //keep track of seconds
            currentTime = 0;
            //current hour
            hour = 6;

            gameObjects = new List<GameObject>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Textures
            playerTexture = Content.Load<Texture2D>("ElizabethSprite");
            testNPCTexture = Content.Load<Texture2D>("npc");
            #region Button Textures
            menuButtonTexture = Content.Load<Texture2D>("MenuBox");
            pauseTexture = Content.Load<Texture2D>("PauseButton");
            inventoryTexture = Content.Load<Texture2D>("InventorySingle");
            exitTexture = Content.Load<Texture2D>("ExitBox");
            testStairs = Content.Load<Texture2D>("stairsTest");
            #endregion

            // Load Fonts
            font = Content.Load<SpriteFont>("font");

            // Initialize Objects
            player = new Player("Char", playerPos, playerTexture, windowHeight, windowWidth);

            #region NPC Initialization
            NPC1 = new NPC("test1", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            NPC2 = new NPC("test2", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            NPC3 = new NPC("test3", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            NPC4 = new NPC("test4", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            NPC5 = new NPC("test5", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            NPC6 = new NPC("test6", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            #endregion

            #region Button Initialization
            topButton = new Button("Menu", menuButtonTexture, font,
                new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                GraphicsDevice.Viewport.Height / 3 - menuButtonTexture.Height / 2
                , menuButtonTexture.Width, menuButtonTexture.Height));

            //Positioned in upper right corner
            pauseButton = new Button("Pause", pauseTexture, font,
                new Rectangle(GraphicsDevice.Viewport.Width-pauseTexture.Width/4,10,pauseTexture.Width/4,pauseTexture.Height/4));
            
            inventoryButton = new Button("Inventory", inventoryTexture, font,
            new Rectangle(GraphicsDevice.Viewport.Width - inventoryTexture.Width - 10, 10, inventoryTexture.Width / 2, inventoryTexture.Height / 2));

            bottomButton = new Button("Menu", menuButtonTexture, font,
                new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                GraphicsDevice.Viewport.Height / 2 - menuButtonTexture.Height / 2
                , menuButtonTexture.Width, menuButtonTexture.Height));

            exitButton = new Button("Exit", exitTexture, font,
                new Rectangle(GraphicsDevice.Viewport.Width - exitTexture.Width/3, 10, exitTexture.Width/3 , exitTexture.Height/3));

            testStairsButton = new Button("", testStairs, font,
                new Rectangle(0, 100, 100, windowHeight));
            #endregion

            // Load In Items
            LoadItems();

            gameObjects.Add(items[0]);
            gameObjects.Add(NPC1);
            gameObjects.Add(NPC2);
            gameObjects.Add(NPC3);
            gameObjects.Add(NPC4);
            gameObjects.Add(NPC5);
            gameObjects.Add(NPC6);
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

            switch (currentState)
            {
                case State.MainMenu:
                    //THe Play Button
                    topButton.Update();
                    //Play Exit Button (currently does nothing)
                    bottomButton.Update();
                    ProcessMainMenu(kbState);
                    break;
                case State.Instructions:
                    //the continue button
                    bottomButton.Update();
                    ProcessInstructions(kbState);
                    break;
                case State.Game:
                    //the inventory
                    inventoryButton.Update();
                    //the pause button
                    pauseButton.Update();
                    testStairsButton.Update();
                    ProcessTimer(gameTime);
                    ProcessGame(kbState);
                    break;
                case State.Inventory:
                    //the return button (x)
                    exitButton.Update();
                    ProcessInventory(kbState);
                    break;
                case State.EndMenu:
                    bottomButton.Update();
                    ProcessEndMenu(kbState);
                    break;
                case State.PauseMenu:
                    //The return button
                    topButton.Update();
                    //the exit to main menu button
                    bottomButton.Update();
                    ProcessPauseMenu(kbState);
                    break;
                case State.ExitGame:
                    Exit();
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
            ShapeBatch.Begin(_graphics.GraphicsDevice);

            switch (currentState)
            {
                case State.MainMenu:
                    // Main Menu text
                    _spriteBatch.DrawString(font, "Main Menu\nPress P or click the play button to proceed to game" +
                        "\nPress Q or click the exit button to leave the game", new Vector2(0, 0), Color.White);
                    // Main Menu Background
                    GraphicsDevice.Clear(Color.Red);

                    //Main Menu Play Button
                    topButton.Draw(_spriteBatch, "");
                    //Hardcoded to center currently, will patch in future
                    _spriteBatch.DrawString(font, "PLAY", new Vector2(370, 150), Color.Black);
                    //Exit game
                    //Technically we don't have a state for this yet, but I'm still going to put this here
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "EXIT", new Vector2(370, 230), Color.Black);

                    break;

                case State.Instructions:
                    // Instructions background
                    GraphicsDevice.Clear(Color.Pink);

                    // Instructions text
                    _spriteBatch.DrawString(font, "Instructions\nUse WASD to move\nClick on NPCs to talk and use spacebar to advance text\n" +
                        "Click on items to pick them up\nPress P to enter the game or click the continue button to proceed", new Vector2(0, 0), Color.White);

                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "CONTINUE", new Vector2(340,230), Color.Black);
           
                    break;

                case State.Game:
                    // Enter Debug Text
                    //_spriteBatch.DrawString(font, $"You are now playing the game.\nPress M to go back\nor I to go to inventory." +
                    //            $"\n\n{currentRoom}", new Vector2(0, 0), Color.White);

                    _spriteBatch.DrawString(font, $"{currentRoom}\n{hour} PM", new Vector2(0, 0), Color.White);

                    //Draw the pause button and inventory button before switch so that it appears on all screens
                    pauseButton.Draw(_spriteBatch, "");
                    inventoryButton.Draw(_spriteBatch, "");


                    // When in game state, check for what room state you are in
                    switch (currentRoom)
                    {
                        case Rooms.Room1:
                            GraphicsDevice.Clear(Color.Navy);

                            NPC2.Draw(_spriteBatch);

                            if (NPC2.IsTalking)
                            {
                                NPC2.Speak(_spriteBatch, font);
                            }

                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room2:

                            GraphicsDevice.Clear(Color.DarkOliveGreen);
                            
                            testStairsButton.Draw(_spriteBatch);
                            NPC3.Draw(_spriteBatch);

                            if (NPC3.IsTalking)
                            {
                                NPC3.Speak(_spriteBatch, font);
                            }

                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room3:

                            //_spriteBatch.DrawString(font, $"\n\n\n\n{currentRoom}\n#{testNPC.DialogueNum} Talking:{testNPC.IsTalking}", new Vector2(0, 0), Color.White);

                            GraphicsDevice.Clear(Color.Gray);
                            NPC1.Draw(_spriteBatch);
                            player.Draw(_spriteBatch);

                            //if currently talking, draw dialogue
                            if (NPC1.IsTalking)
                            {
                                NPC1.Speak(_spriteBatch, font);
                            }

                            // If the knife is not picked up, draw it
                            if (!items[0].PickedUp)
                            {
                                items[0].Draw(_spriteBatch);
                            }

                            break;
                        case Rooms.Room4:
                            GraphicsDevice.Clear(Color.Black);  
                            
                            testStairsButton.Draw(_spriteBatch);
                            NPC4.Draw(_spriteBatch);

                            if (NPC4.IsTalking)
                            {
                                NPC4.Speak(_spriteBatch, font);
                            }

                            player.Draw(_spriteBatch);
                            break;                   
                        case Rooms.Room5:
                            GraphicsDevice.Clear(Color.DarkSlateGray);

                            NPC5.Draw(_spriteBatch);

                            if (NPC5.IsTalking)
                            {
                                NPC5.Speak(_spriteBatch, font);
                            }

                            player.Draw(_spriteBatch);
                            break;
                        case Rooms.Room6:
                            GraphicsDevice.Clear(Color.DarkViolet);

                            NPC6.Draw(_spriteBatch);

                            if (NPC6.IsTalking)
                            {
                                NPC6.Speak(_spriteBatch, font);
                            }

                            player.Draw(_spriteBatch);
                            break;
                        default:
                            break;
                    }

                    break;

                case State.Inventory:
                    _spriteBatch.DrawString(font, "You are now in the inventory.\nPress I to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Green);

                    exitButton.Draw(_spriteBatch, "");

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
                        items[0].Draw(_spriteBatch);
                        thisItem.Position = new Rectangle((int) formatPos.X, (int) formatPos.Y, thisItem.Position.Width, thisItem.Position.Height);
                        _spriteBatch.DrawString(font,
                            thisItem.ToString(),
                            formatPos + new Vector2(0,thisItem.Position.Height),
                            Color.White);
                    }
                    break;
                case State.PauseMenu:
                    _spriteBatch.DrawString(font, "You are now paused.\nPress ESC to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Cyan);
                    //Continue
                    topButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "CONTINUE", new Vector2(340, 150), Color.Black);
                    //Return to the main menu
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "MAIN MENU", new Vector2(330, 230), Color.Black);
                    break;
                case State.EndMenu:
                    _spriteBatch.DrawString(font, "You ran out of time!\nPress ESC to go back to the main menu.", new Vector2(0, 0), Color.White);
                    //Return to the main menu
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "MAIN MENU", new Vector2(330, 230), Color.Black);
                    GraphicsDevice.Clear(Color.DarkMagenta);
                    break;
                default:
                    break;
            }

            _spriteBatch.End();
            ShapeBatch.End();
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
                // if mouse is clicked, would return true,
                // else would return false
                return SingleMousePress(mState);
            }
            else // Mouse is not intersecting obj
            {
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
                // Item Clicked
                if (SingleMousePress(mState)) 
                {
                    return true;
                }
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
            if (SingleKeyPress(Keys.P, kbState) || topButton.BeenClicked)
            {
                currentState = State.Instructions;
            }

            if (SingleKeyPress(Keys.Q, kbState) || bottomButton.BeenClicked)
            {
                currentState = State.ExitGame;
            }
        }

        /// <summary>
        /// If the player inputs the "P" key, take them to the instructions
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessInstructions(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.P, kbState) || bottomButton.BeenClicked)
            {
                currentState = State.Game;
                //Reset or Set the Game
                ResetGame();
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

            if (SingleKeyPress(Keys.I, kbState) || inventoryButton.BeenClicked)
            {
                currentState = State.Inventory;
            }

            if (SingleKeyPress(Keys.Escape, kbState) || pauseButton.BeenClicked)
            {
                currentState = State.PauseMenu;
            }

            // Runs hover logic
            bool hoveredOver = false;
            // Searches all of the game objects
            foreach (GameObject g in gameObjects)
            {
                // If we're hovering over any of them, set the flag
                if (g.Hover(mState))
                {
                    hoveredOver = true; 
                }
            }
            // If the flag has been set
            if (hoveredOver)
            {
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);
            }

            #region Talking To NPCS
            if (NPC1.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC1.DialogueNum++;
            }

            if (NPC2.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC2.DialogueNum++;
            }

            if (NPC3.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC3.DialogueNum++;
            }

            if (NPC4.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC4.DialogueNum++;
            }

            if (NPC5.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC5.DialogueNum++;
            }

            if (NPC6.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC6.DialogueNum++;
            }
            #endregion


            // Simulate room movement 
            switch (currentRoom)
            {
                case Rooms.Room1:
                    player.Move(kbState);
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

                    if (Clicked(NPC1))
                    {
                        // Change to
                        NPC1.IsTalking = !NPC1.IsTalking;
                    }
                    break;
                case Rooms.Room2:
                    player.Move(kbState);
                    //if you walk right, go back to room 1
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room1;
                        player.Left();
                    }

                    //if you click on stairs, move to room 4
                    if (testStairsButton.BeenClicked)
                    {
                        currentRoom = Rooms.Room4;
                        player.Left();
                    }
                    break;
                case Rooms.Room3:
                    player.Move(kbState);
                    //if you walk left, go back to room 1
                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room1;
                        player.Right();
                    }
                    // ~~~ GAME OBJECTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    #region Game Objects
                    //if you click on him, toggle dialogue
                    if (Clicked(NPC1))
                    {
                        // Change to
                        NPC1.IsTalking = !NPC1.IsTalking;
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
                        //TODO: fix this later
                        items[0].Position = new Rectangle(1000, 1000, 90, 40);
                        player.Inventory.Add(items[0]);
                    }
                    #endregion
                    break;
                case Rooms.Room4:
                    player.Move(kbState);

                    //if you click on stairs, move to room 2
                    if (testStairsButton.BeenClicked)
                    {
                        currentRoom = Rooms.Room2;
                        player.Left();
                    }

                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room5;
                        player.Left();
                    }

                    break;
                case Rooms.Room5:
                    player.Move(kbState);

                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room4;
                        player.Right();
                    }

                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room6;
                        player.Left();
                    }

                    break;
                case Rooms.Room6:
                    player.Move(kbState);

                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room5;
                        player.Right();
                    }

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
            if (SingleKeyPress(Keys.I, kbState) || exitButton.BeenClicked)
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
            if (SingleKeyPress(Keys.Escape, kbState) || topButton.BeenClicked)
            {
                currentState = State.Game;
            }

            if (SingleKeyPress(Keys.M, kbState) || bottomButton.BeenClicked)
            {
                currentState = State.MainMenu;
            }
        }

        private void ProcessEndMenu(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.Escape, kbState) || bottomButton.BeenClicked)
            {
                currentState = State.MainMenu;
            }
        }

        private void ProcessTimer(GameTime gameTime)
        {
            //if current time is greater than the total time per hour, increment the hour 
            if (currentTime > totalTime)
            {
                hour++;
                currentTime = 0;
            }
            else
            {
                currentTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            
            //if time hits 12AM, end game
            if (hour == 12)
            {
                currentState = State.EndMenu;
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
        /// <summary>
        /// Reset the game, by setting default values and remaking a new player.
        /// </summary>
        private void ResetGame() 
        {
            // Reset Timer
            totalTime = 120;
            // Reset to default room
            currentRoom = Rooms.Room1;
            // Position of character
            playerPos = new Vector2(windowWidth / 2, windowHeight - 200);
            // Reset all value of character by remaking the character
            player = new Player("Char", playerPos, playerTexture, windowHeight, windowWidth);
            // Reset all items loaded
            foreach(Item a in items) 
            {
                a.ResetItem();
            }
        }
    }
}