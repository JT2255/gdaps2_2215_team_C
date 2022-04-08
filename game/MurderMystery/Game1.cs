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
        // ~~~ FIELDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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
        private Button accuseButton;
        #endregion

        //Configs
        private int windowWidth;
        private int windowHeight;
        private Vector2 playerPos;

        //Textures
        #region Character Textures
        private Texture2D playerTexture;
        private Texture2D testNPCTexture;
        private Texture2D claraFarleyTexture;
        private Texture2D edithEspinozaTexture;
        private Texture2D elizabethMaxwellTexture;
        private Texture2D summerHinesTexture;
        #endregion

        #region Button Textures 
        private Texture2D menuButtonTexture;
        private Texture2D pauseTexture;
        private Texture2D inventoryTexture;
        private Texture2D exitTexture;
        private Texture2D testStairs;
        private Texture2D accuseTexture;
        #endregion

        //text box texture
        private Texture2D dialogueBox;

        //Misc
        private StreamReader reader = null;
        private List<Item> items;
        private double totalTime;
        private double currentTime;
        private int hour;
        private List<Rectangle> itemInvPos;
        private List<GameObject> gameObjects;
        private bool won;
        #endregion

        // ~~~ GAME LOOP STUFF ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

            //initialize dialogue boc
            dialogueBox = Content.Load<Texture2D>("dialogueBox");

            itemInvPos = new List<Rectangle>();
            //time per in-game hour
            totalTime = 120;
            //keep track of seconds
            currentTime = 0;
            //current hour
            hour = 6;
            //correctly guessed murderer
            won = false;

            items = new List<Item>();
            gameObjects = new List<GameObject>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            font = Content.Load<SpriteFont>("font");

            // Load in characters
            LoadCharacters();

            // Load in buttons
            LoadButtons();

            // Load In Items
            LoadItems();
        }

        protected override void Update(GameTime gameTime)
        {
            // Sets up our states
            kbState = Keyboard.GetState();
            mState = Mouse.GetState();

            // Updates the player animation
            player.UpdateAnim(gameTime);

            BeingDrawnSets();
            HoverLogic();

            #region FSM Switching
            switch (currentState)
            {
                case State.MainMenu:
                    //The Play Button
                    topButton.Update();

                    //Play Exit Button (currently does nothing)
                    bottomButton.Update();

                    //Process main menu
                    ProcessMainMenu(kbState);
                    break;
                case State.Instructions:
                    //the continue button
                    bottomButton.Update();

                    //Process instruction menu
                    ProcessInstructions(kbState);
                    break;
                case State.Game:

                    //the inventory
                    inventoryButton.Update();

                    //the pause button
                    pauseButton.Update();

                    //stair button
                    testStairsButton.Update();

                    //accuse button
                    accuseButton.Update();

                    //process timer and game state
                    ProcessTimer(gameTime);
                    ProcessGame(kbState);
                    break;
                case State.Inventory:
                    //the return button (x)
                    exitButton.Update();

                    //process inventory state
                    ProcessInventory(kbState);
                    break;
                case State.EndMenu:
                    //main menu button
                    bottomButton.Update();

                    //process end screen state
                    ProcessEndMenu(kbState);
                    break;
                case State.PauseMenu:
                    //The return button
                    topButton.Update();

                    //the exit to main menu button
                    bottomButton.Update();

                    //process pause menu state
                    ProcessPauseMenu(kbState);
                    break;
                case State.ExitGame:
                    //close game
                    Exit();
                    break;
                default:
                    break;
            }
            #endregion

            // Retrieve the previous states
            prevKbState = kbState;
            prevMState = mState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            ShapeBatch.Begin(GraphicsDevice);
            _spriteBatch.Begin();
            #region FSM Switching
            switch (currentState)
            {
                case State.MainMenu:
                    #region Main Menu
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
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "EXIT", new Vector2(370, 230), Color.Black);

                    break;
                #endregion
                case State.Instructions:
                    #region Instructions
                    // Instructions background
                    GraphicsDevice.Clear(Color.Pink);

                    // Instructions text
                    _spriteBatch.DrawString(font, "Instructions\nUse A and D to move\nClick on NPCs to talk and use spacebar to advance text\n" +
                        "Click on items to pick them up\nPress P to enter the game or click the continue button to proceed", new Vector2(0, 0), Color.White);

                    //draw button
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "CONTINUE", new Vector2(340,230), Color.Black);
           
                    break;
                #endregion
                case State.Game:
                    #region Game
                    // Debug Text
                    _spriteBatch.DrawString(font, $"{hour} PM", new Vector2(0, 0), Color.White);

                    //Draw the pause button and inventory button before switch so that it appears on all screens
                    pauseButton.Draw(_spriteBatch, "");
                    inventoryButton.Draw(_spriteBatch, "");


                    // When in game state, check for what room state you are in
                    switch (currentRoom)
                    {
                        case Rooms.Room1:
                            GraphicsDevice.Clear(Color.Navy);

                            //draw npc
                            NPC1.Draw(_spriteBatch);

                            //show npc dialogue
                            if (NPC1.IsTalking)
                            {
                                NPC1.Speak(_spriteBatch, font, dialogueBox);

                                if (items[0].PickedUp)
                                {
                                    accuseButton.Draw(_spriteBatch);
                                }
                            }

                            //draw player
                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room2:

                            GraphicsDevice.Clear(Color.DarkOliveGreen);
                            
                            //draw stairs and npc
                            testStairsButton.Draw(_spriteBatch);
                            testStairsButton.Position = new Rectangle(0, 25, 114, 309);
                            NPC2.Draw(_spriteBatch);

                            //show npc dialogue
                            if (NPC2.IsTalking)
                            {
                                NPC2.Speak(_spriteBatch, font, dialogueBox);

                                if (items[0].PickedUp)
                                {
                                    accuseButton.Draw(_spriteBatch);
                                }
                            }



                            //draw player
                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room3:

                            GraphicsDevice.Clear(Color.Gray);

                            //draw npc
                            NPC3.Draw(_spriteBatch);
                            

                            //draw npc dialogue
                            if (NPC3.IsTalking)
                            {
                                NPC3.Speak(_spriteBatch, font, dialogueBox);

                                if (items[0].PickedUp)
                                {
                                    accuseButton.Draw(_spriteBatch);
                                }
                            }

                            

                            //draw player
                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room4:
                            GraphicsDevice.Clear(Color.Black);  
                            
                            //draw stairs and npc
                            testStairsButton.Draw(_spriteBatch);
                            testStairsButton.Position = new Rectangle(0, 400, 114, 309);

                            NPC4.Draw(_spriteBatch);

                            //draw npc dialogue
                            if (NPC4.IsTalking)
                            {
                                NPC4.Speak(_spriteBatch, font, dialogueBox);

                                if (items[0].PickedUp)
                                {
                                    accuseButton.Draw(_spriteBatch);
                                }
                            }

                            // If the key is not picked up, draw it
                            if (!items[1].PickedUp)
                            {
                                items[1].Draw(_spriteBatch);
                            }

                            //draw player
                            player.Draw(_spriteBatch);

                            break;                   
                        case Rooms.Room5:
                            GraphicsDevice.Clear(Color.DarkSlateGray);

                            //draw npc
                            NPC5.Draw(_spriteBatch);

                            //draw npc dialogue
                            if (NPC5.IsTalking)
                            {
                                NPC5.Speak(_spriteBatch, font, dialogueBox);

                                if (items[0].PickedUp)
                                {
                                    accuseButton.Draw(_spriteBatch);
                                }
                            }

                            // If the knife is not picked up, draw it
                            if (!items[0].PickedUp)
                            {
                                items[0].Draw(_spriteBatch);
                            }

                            //draw player
                            player.Draw(_spriteBatch);

                            break;
                        case Rooms.Room6:
                            GraphicsDevice.Clear(Color.DarkViolet);

                            //draw npc
                            NPC6.Draw(_spriteBatch);

                            //draw npc dialogue
                            if (NPC6.IsTalking)
                            {
                                NPC6.Speak(_spriteBatch, font, dialogueBox);

                                if (items[0].PickedUp)
                                {
                                    accuseButton.Draw(_spriteBatch);
                                }
                            }

                            //draw player
                            player.Draw(_spriteBatch);

                            break;
                        default:
                            break;
                    }

                    break;
                #endregion
                case State.Inventory:
                    #region Inventory
                    Vector2 startPoint = new Vector2(windowWidth / 8, windowHeight / 8); // Start Point for displaying items
                    //inventory text
                    GraphicsDevice.Clear(Color.Green);
                    _spriteBatch.DrawString(font, "You are now in the inventory.\nPress I to go back to the game.", new Vector2(0, 0), Color.White);

                    //draw exit button
                    exitButton.Draw(_spriteBatch, "");

                    // Draws the inventory items
                    // Display Inventory -----
                    // Should format the inventory into a small arrangement of Item objects
                    for(int i = 0; i < player.Inventory.Count; i++) 
                    {
                        // Label what to use
                        Item thisItem = player.Inventory[i];
                        int itemBoxLength = windowWidth / 12;
                        Vector2 formatPos = startPoint + new Vector2(0, itemBoxLength + 10);
                        thisItem.Position = new Rectangle((int) formatPos.X +(itemBoxLength - thisItem.Position.Width)/2,
                            (int) formatPos.Y + (itemBoxLength - thisItem.Position.Height) / 2,
                            thisItem.Position.Width,
                            thisItem.Position.Height);

                        // Draw the Boxes (To be overlapped)
                        ShapeBatch.Box(formatPos.X + itemBoxLength + 10, //TextBox
                            formatPos.Y,
                            windowWidth * 77/100,
                            itemBoxLength,
                            Color.Black); 
                        _spriteBatch.Draw(dialogueBox, //ItemBox
                            new Rectangle(
                                (int)formatPos.X,
                                (int)formatPos.Y,
                                itemBoxLength,
                                itemBoxLength),
                            Color.White);
                        // Draw the Item and write the description.
                        thisItem.Draw(_spriteBatch);
                        _spriteBatch.DrawString(font,
                            thisItem.ToString(),
                            formatPos + new Vector2(itemBoxLength + 14, (itemBoxLength - font.LineSpacing)/2),
                            Color.White);
                        startPoint = formatPos;
                    }
                    #endregion
                    break;
                case State.PauseMenu:
                    #region Pause
                    //pause menu text
                    _spriteBatch.DrawString(font, "You are now paused.\nPress ESC to go back to the game.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Cyan);

                    //Continue
                    topButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "CONTINUE", new Vector2(340, 150), Color.Black);

                    //Return to the main menu
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "MAIN MENU", new Vector2(330, 230), Color.Black);

                    break;
                #endregion
                case State.EndMenu:
                    #region End Screen

                    //if guessed correctly
                    if (won)
                    {
                        _spriteBatch.DrawString(font, "You correctly figured out the murderer!\nPress ESC to go back to the main menu.", new Vector2(0, 0), Color.White);
                    }
                    //did not guess correctly, or ran out of time
                    else
                    {
                        _spriteBatch.DrawString(font, "You ran out of time or accused an innocent!\nPress ESC to go back to the main menu.", new Vector2(0, 0), Color.White);
                    }
                    

                    //Return to the main menu
                    bottomButton.Draw(_spriteBatch, "");

                    _spriteBatch.DrawString(font, "MAIN MENU", new Vector2(330, 230), Color.Black);
                    GraphicsDevice.Clear(Color.DarkMagenta);
                    break;
                    #endregion
                default:
                    break;
            }
            #endregion
            ShapeBatch.End();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        // ~~~ INPUT CAPTURES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

        /// <summary>
        /// If mouse is held down inside of npc, return true
        /// </summary>
        /// <param name="gameObject">npc object</param>
        /// <returns>returns true or false depending on if mouse is clicking on object</returns>
        private bool Clicked(GameObject gameObject)
        {
            // If mouse intersects the obj
            if (mState.X > gameObject.Position.Left &&
                mState.X < gameObject.Position.Right &&
                mState.Y > gameObject.Position.Top &&
                mState.Y < gameObject.Position.Bottom)
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
        /// Hover logic to change mouse cursor
        /// </summary>
        private void HoverLogic()
        {
            // Sets our flag
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
        }
        #endregion

        // ~~~ GAME LOGIC ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

            #region Talking To NPCS
            
            //progress through npc dialogue
            if (NPC1.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC1.DialogueNum++;
            }

            //progress through npc dialogue
            if (NPC2.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC2.DialogueNum++;
            }

            //progress through npc dialogue
            if (NPC3.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC3.DialogueNum++;
            }

            //progress through npc dialogue
            if (NPC4.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC4.DialogueNum++;
            }

            //progress through npc dialogue
            if (NPC5.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC5.DialogueNum++;
            }

            //progress through npc dialogue
            if (NPC6.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                NPC6.DialogueNum++;
            }
            #endregion

            #region FSM Switching
            // Simulate room movement 
            switch (currentRoom)
            {
                case Rooms.Room1:

                    player.Move(kbState, currentRoom);

                    //if you walk to the left, go to room 2
                    if (player.Position.X < 0)
                    {
                        // Change Room
                        currentRoom = Rooms.Room2;
                        // Re-Orient Player
                        player.Right();

                        NPC1.IsTalking = false;
                    }

                    //if you walk to the right, go to room 3
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room3;
                        player.Left();

                        NPC1.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(NPC1))
                    {
                        NPC1.IsTalking = !NPC1.IsTalking;
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked)
                    {
                        //if they are murderer, win
                        if (NPC1.IsMurderer) 
                        {
                            won = true;
                        }

                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                case Rooms.Room2:

                    player.Move(kbState, currentRoom);

                    //if you walk right, go back to room 1
                    if (player.Position.X > windowWidth)
                    {
                        NPC2.IsTalking = false;
                        currentRoom = Rooms.Room1;
                        player.Left();
                    }

                    //if you click on stairs, move to room 4
                    if (testStairsButton.BeenClicked)
                    {
                        currentRoom = Rooms.Room4;
                        NPC2.IsTalking = false;
                        player.Left();
                    }

                    //toggle npc talking state
                    if (Clicked(NPC2))
                    {
                        NPC2.IsTalking = !NPC2.IsTalking;
                    }

                    //stop user from walking off screen
                    if (player.Position.X < 0)
                    {
                        player.Position = new Vector2(0, player.Position.Y);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked)
                    {
                        //if they are murderer, win
                        if (NPC2.IsMurderer)
                        {
                            won = true;
                        }
                        
                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                case Rooms.Room3:

                    player.Move(kbState, currentRoom);

                    //stop user from walking off screen
                    if (player.Position.X > windowWidth - 45)
                    {
                        player.Position = new Vector2(windowWidth - 45, player.Position.Y);
                    }

                    //if you walk left, go back to room 1
                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room1;
                        NPC3.IsTalking = false;
                        player.Right();
                    }


                    //toggle npc talking state
                    if (Clicked(NPC3))
                    {
                        NPC3.IsTalking = !NPC3.IsTalking;
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked)
                    {
                        //if they are muderer, win
                        if (NPC3.IsMurderer)
                        {
                            won = true;
                        }

                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                case Rooms.Room4:

                    player.Move(kbState, currentRoom);

                    //if you click on stairs, move to room 2
                    if (testStairsButton.BeenClicked)
                    {
                        currentRoom = Rooms.Room2;
                        NPC4.IsTalking = false;
                        player.Left();
                    }

                    //if you move to right, go to room 5
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room5;
                        NPC4.IsTalking = false;
                        player.Left();
                    }

                    //toggle npc talking state
                    if (Clicked(NPC4))
                    {
                        NPC4.IsTalking = !NPC4.IsTalking;
                    }

                    // Clicked on item
                    if (Clicked(items[1]))
                    {
                        items[1].PickedUp = true;
                        player.Inventory.Add(items[1]);
                    }

                    //stop user from walking off screen
                    if (player.Position.X < 0)
                    {
                        player.Position = new Vector2(0, player.Position.Y);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked)
                    {
                        //if they are the murderer, win
                        if (NPC4.IsMurderer)
                        {
                            won = true;
                        }

                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                case Rooms.Room5:

                    player.Move(kbState, currentRoom);

                    //if you move to left, go to room 4
                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room4;
                        NPC5.IsTalking = false;
                        player.Right();
                    }

                    //if you move to right, go to room 6
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room6;
                        NPC5.IsTalking = false;
                        player.Left();
                    }

                    //toggle npc talking state
                    if (Clicked(NPC5))
                    {
                        NPC5.IsTalking = !NPC5.IsTalking;
                    }

                    // If you click on the knife, toggle it
                    if (Clicked(items[0]))
                    {
                        items[0].PickedUp = true;
                        player.Inventory.Add(items[0]);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked)
                    {
                        //if they are the murderer, win
                        if (NPC5.IsMurderer)
                        {
                            won = true;
                        }

                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                case Rooms.Room6:

                    player.Move(kbState, currentRoom);

                    //if you move to left, go to room 5
                    if (player.Position.X < 0)
                    {
                        currentRoom = Rooms.Room5;
                        NPC6.IsTalking = false;
                        player.Right();
                    }

                    //toggle npc talking state
                    if (Clicked(NPC6))
                    {
                        NPC6.IsTalking = !NPC6.IsTalking;
                    }

                    //stop user from walking off screen
                    if (player.Position.X > windowWidth - 45)
                    {
                        player.Position = new Vector2(windowWidth - 45, player.Position.Y);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked)
                    {
                        //if they are the murderer, win
                        if (NPC6.IsMurderer)
                        {
                            won = true;
                        }

                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                default:
                    break;
            }
            #endregion
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

        /// <summary>
        /// Sets whether an item is being drawn for hover logic
        /// </summary>
        private void BeingDrawnSets()
        {
            switch (currentState)
            {
                case State.MainMenu:

                    topButton.BeingDrawn = true;
                    bottomButton.BeingDrawn = true;
                    inventoryButton.BeingDrawn = false;
                    pauseButton.BeingDrawn = false;
                    exitButton.BeingDrawn = false;
                    testStairsButton.BeingDrawn = false;
                    accuseButton.BeingDrawn = false;

                    NPC1.BeingDrawn = false;
                    NPC2.BeingDrawn = false;
                    NPC3.BeingDrawn = false;
                    NPC4.BeingDrawn = false;
                    NPC5.BeingDrawn = false;
                    NPC6.BeingDrawn = false;

                    items[0].BeingDrawn = false;
                    items[1].BeingDrawn = false;

                    break;
                case State.Instructions:

                    topButton.BeingDrawn = false;
                    bottomButton.BeingDrawn = true;
                    inventoryButton.BeingDrawn = false;
                    pauseButton.BeingDrawn = false;
                    exitButton.BeingDrawn = false;
                    testStairsButton.BeingDrawn = false;
                    accuseButton.BeingDrawn = false;

                    NPC1.BeingDrawn = false;
                    NPC2.BeingDrawn = false;
                    NPC3.BeingDrawn = false;
                    NPC4.BeingDrawn = false;
                    NPC5.BeingDrawn = false;
                    NPC6.BeingDrawn = false;

                    items[0].BeingDrawn = false;
                    items[1].BeingDrawn = false;

                    break;
                case State.Game:

                    topButton.BeingDrawn = false;
                    bottomButton.BeingDrawn = false;
                    inventoryButton.BeingDrawn = true;
                    exitButton.BeingDrawn = false;
                    pauseButton.BeingDrawn = true;
                    
                    switch (currentRoom)
                    {
                        case Rooms.Room1:
                            NPC1.BeingDrawn = true;
                            NPC2.BeingDrawn = false;
                            NPC3.BeingDrawn = false;
                            NPC4.BeingDrawn = false;
                            NPC5.BeingDrawn = false;
                            NPC6.BeingDrawn = false;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            break;

                        case Rooms.Room2:
                            NPC1.BeingDrawn = false;
                            NPC2.BeingDrawn = true;
                            NPC3.BeingDrawn = false;
                            NPC4.BeingDrawn = false;
                            NPC5.BeingDrawn = false;
                            NPC6.BeingDrawn = false;
                            testStairsButton.BeingDrawn = true;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            break;

                        case Rooms.Room3:
                            NPC1.BeingDrawn = false;
                            NPC2.BeingDrawn = false;
                            NPC3.BeingDrawn = true;
                            NPC4.BeingDrawn = false;
                            NPC5.BeingDrawn = false;
                            NPC6.BeingDrawn = false;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            break;

                        case Rooms.Room4:
                            NPC1.BeingDrawn = false;
                            NPC2.BeingDrawn = false;
                            NPC3.BeingDrawn = false;
                            NPC4.BeingDrawn = true;
                            NPC5.BeingDrawn = false;
                            NPC6.BeingDrawn = false;
                            testStairsButton.BeingDrawn = true;
                            accuseButton.BeingDrawn = false;
                            if (!items[1].PickedUp)
                            {
                                items[1].BeingDrawn = true;
                            }
                            else
                            {
                                items[1].BeingDrawn = false;
                            }
                            break;

                        case Rooms.Room5:
                            NPC1.BeingDrawn = false;
                            NPC2.BeingDrawn = false;
                            NPC3.BeingDrawn = false;
                            NPC4.BeingDrawn = false;
                            NPC5.BeingDrawn = true;
                            NPC6.BeingDrawn = false;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            if (!items[0].PickedUp)
                            {
                                items[0].BeingDrawn = true;
                            }
                            else
                            {
                                items[0].BeingDrawn = false;
                            }
                            break;

                        case Rooms.Room6:
                            NPC1.BeingDrawn = false;
                            NPC2.BeingDrawn = false;
                            NPC3.BeingDrawn = false;
                            NPC4.BeingDrawn = false;
                            NPC5.BeingDrawn = false;
                            NPC6.BeingDrawn = true;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            break;
                    }

                    break;
                case State.Inventory:
                    topButton.BeingDrawn = false;
                    bottomButton.BeingDrawn = false;
                    inventoryButton.BeingDrawn = false;
                    pauseButton.BeingDrawn = false;
                    exitButton.BeingDrawn = true;
                    testStairsButton.BeingDrawn = false;
                    accuseButton.BeingDrawn = false;

                    NPC1.BeingDrawn = false;
                    NPC2.BeingDrawn = false;
                    NPC3.BeingDrawn = false;
                    NPC4.BeingDrawn = false;
                    NPC5.BeingDrawn = false;
                    NPC6.BeingDrawn = false;

                    if (items[0].PickedUp)
                    {
                        items[0].BeingDrawn = true;
                    }
                    else
                    {
                        items[0].BeingDrawn = false;
                    }
                    if (items[1].PickedUp)
                    {
                        items[1].BeingDrawn = true;
                    }
                    else
                    {
                        items[1].BeingDrawn = false;
                    }

                    break;
                case State.EndMenu:

                    topButton.BeingDrawn = false;
                    bottomButton.BeingDrawn = true;
                    inventoryButton.BeingDrawn = false;
                    pauseButton.BeingDrawn = false;
                    exitButton.BeingDrawn = false;
                    testStairsButton.BeingDrawn = false;
                    accuseButton.BeingDrawn = false;

                    NPC1.BeingDrawn = false;
                    NPC2.BeingDrawn = false;
                    NPC3.BeingDrawn = false;
                    NPC4.BeingDrawn = false;
                    NPC5.BeingDrawn = false;
                    NPC6.BeingDrawn = false;

                    items[0].BeingDrawn = false;

                    break;
                case State.PauseMenu:

                    topButton.BeingDrawn = true;
                    bottomButton.BeingDrawn = true;
                    inventoryButton.BeingDrawn = false;
                    pauseButton.BeingDrawn = false;
                    exitButton.BeingDrawn = false;
                    testStairsButton.BeingDrawn = false;
                    accuseButton.BeingDrawn = false;

                    NPC1.BeingDrawn = false;
                    NPC2.BeingDrawn = false;
                    NPC3.BeingDrawn = false;
                    NPC4.BeingDrawn = false;
                    NPC5.BeingDrawn = false;
                    NPC6.BeingDrawn = false;

                    items[0].BeingDrawn = false;

                    break;
                case State.ExitGame:
                default:
                    break;
            }
        }
        #endregion

        // ~~~ MISC METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Misc Methods
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
                    // Adds the item to the gameObjects list
                    gameObjects.Add(items[items.Count - 1]);
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
        /// Loads in the characters and player
        /// </summary>
        private void LoadCharacters()
        {
            // Loads the textures
            playerTexture = Content.Load<Texture2D>("PlayerSprite");
            claraFarleyTexture = Content.Load<Texture2D>("ClaraSprite");
            edithEspinozaTexture = Content.Load<Texture2D>("EdithSprite");
            elizabethMaxwellTexture = Content.Load<Texture2D>("ElizabethSprite");
            summerHinesTexture = Content.Load<Texture2D>("SummerSprite");
            testNPCTexture = Content.Load<Texture2D>("npc");

            // Initializes the characters and adds them to the gameObjects list
            player = new Player("Char", playerPos, playerTexture, windowHeight, windowWidth);
            NPC1 = new NPC("Clara Farley", false, false, new Rectangle(400, 200, 40, 107), claraFarleyTexture);
            gameObjects.Add(NPC1);
            NPC2 = new NPC("Edith Espinoza", false, false, new Rectangle(400, 200, 40, 109), edithEspinozaTexture);
            gameObjects.Add(NPC2);
            NPC3 = new NPC("Elizabeth Maxwell", false, false, new Rectangle(400, 200, 40, 107), elizabethMaxwellTexture);
            gameObjects.Add(NPC3);
            NPC4 = new NPC("Summer Hines", true, false, new Rectangle(400, 200, 40, 107), summerHinesTexture);
            gameObjects.Add(NPC4);
            NPC5 = new NPC("test5", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            gameObjects.Add(NPC5);
            NPC6 = new NPC("test6", false, false, new Rectangle(400, 0, 100, 100), testNPCTexture);
            gameObjects.Add(NPC6);
        }

        /// <summary>
        /// Loads in the buttons
        /// </summary>
        private void LoadButtons()
        {
            // Loads in the textures of the buttons
            menuButtonTexture = Content.Load<Texture2D>("MenuBox");
            pauseTexture = Content.Load<Texture2D>("PauseButton");
            inventoryTexture = Content.Load<Texture2D>("InventorySingle");
            exitTexture = Content.Load<Texture2D>("ExitBox");
            testStairs = Content.Load<Texture2D>("SpiralStaircase");
            accuseTexture = Content.Load<Texture2D>("accuseButtonTexture");

            // Initializes the buttons
            topButton = new Button("Menu", menuButtonTexture, font,
                new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                GraphicsDevice.Viewport.Height / 3 - menuButtonTexture.Height / 2
                , menuButtonTexture.Width, menuButtonTexture.Height));
            gameObjects.Add(topButton);

            //Positioned in upper right corner
            pauseButton = new Button("Pause", pauseTexture, font,
                new Rectangle(GraphicsDevice.Viewport.Width - pauseTexture.Width / 4, 10, pauseTexture.Width / 4, pauseTexture.Height / 4));
            gameObjects.Add(pauseButton);

            inventoryButton = new Button("Inventory", inventoryTexture, font,
            new Rectangle(GraphicsDevice.Viewport.Width - inventoryTexture.Width - 10, 10, inventoryTexture.Width / 2, inventoryTexture.Height / 2));
            gameObjects.Add(inventoryButton);

            bottomButton = new Button("Menu", menuButtonTexture, font,
                new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                GraphicsDevice.Viewport.Height / 2 - menuButtonTexture.Height / 2
                , menuButtonTexture.Width, menuButtonTexture.Height));
            gameObjects.Add(bottomButton);

            exitButton = new Button("Exit", exitTexture, font,
                new Rectangle(GraphicsDevice.Viewport.Width - exitTexture.Width / 3, 10, exitTexture.Width / 3, exitTexture.Height / 3));
            gameObjects.Add(exitButton);

            testStairsButton = new Button("", testStairs, font,
                new Rectangle(0, 25, 114, 309));
            gameObjects.Add(testStairsButton);

            accuseButton = new Button("", accuseTexture, font,
                new Rectangle(630, 250, 150, 50));
            gameObjects.Add(accuseButton);
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

            //reset dialogue positions of all npcs
            NPC1.IsTalking = false;
            NPC2.IsTalking = false;
            NPC3.IsTalking = false;
            NPC4.IsTalking = false;
            NPC5.IsTalking = false;
            NPC6.IsTalking = false;
            NPC1.DialogueNum = 0;
            NPC2.DialogueNum = 0;
            NPC3.DialogueNum = 0;
            NPC4.DialogueNum = 0;
            NPC5.DialogueNum = 0;
            NPC6.DialogueNum = 0;
        }
        #endregion
    }
}