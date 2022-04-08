using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        Room6,
        Room7
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
        private SpriteFont titleFont;
        private Player player;

        #region NPCs
        private NPC clara;
        private NPC edith;
        private NPC elizabeth;
        private NPC summer;
        private NPC edward;
        private NPC frank;
        private NPC james;
        private NPC ernest;
        private NPC document;
        #endregion

        #region Buttons
        private Button topButton;
        private Button pauseButton;
        private Button bottomButton;
        private Button continueButton;
        private Button inventoryButton;
        private Button exitButton;
        private Button testStairsButton;
        private Button accuseButton;
        private Button doorButton;
        #endregion

        //Configs
        private int windowWidth;
        private int windowHeight;
        private Vector2 playerPos;

        //Textures
        #region Character Textures
        private Texture2D playerTexture;
       // private Texture2D testNPCTexture;
        private Texture2D claraFarleyTexture;
        private Texture2D edithEspinozaTexture;
        private Texture2D elizabethMaxwellTexture;
        private Texture2D summerHinesTexture;
        private Texture2D corpseTexture;
        private Texture2D jamesAtkinsTexture;
        private Texture2D edwardCampellTexture;
        private Texture2D ernestBoydTexture;
        private Texture2D frankEspinozaTexture;
        private Texture2D documentTexture;
        #endregion

        #region Button Textures 
        private Texture2D menuButtonTexture;
        private Texture2D pauseTexture;
        private Texture2D inventoryTexture;
        private Texture2D exitTexture;
        private Texture2D testStairs;
        private Texture2D accuseTexture;
        private Texture2D doorTexture;
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
        private Texture2D desk;
        private Song backgroundMusic;
        private Song partyMusic;
        bool musicChanged;
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
            currentTime = 60;
            //current hour
            hour = 6;
            //correctly guessed murderer
            won = false;

            musicChanged = false;

            items = new List<Item>();
            gameObjects = new List<GameObject>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            font = Content.Load<SpriteFont>("font");
            titleFont = Content.Load<SpriteFont>("titleFont");

            // Load music
            backgroundMusic = Content.Load<Song>("Illusory-Realm-MP3");
            partyMusic = Content.Load<Song>("highlanders");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.05f;

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
                    continueButton.Update();

                    //Process instruction menu
                    ProcessInstructions(kbState);
                    break;
                case State.Game:

                    // Plays party music during hour 5
                    if (hour == 5 && !musicChanged)
                    {
                        MediaPlayer.Play(partyMusic);
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = 0.05f;
                        musicChanged = true;
                    }

                    // During the first death, music cuts out
                    if (currentTime >= 115 && hour == 5)
                    {
                        MediaPlayer.Stop();
                    }    

                    // Plays mystery music after the first death
                    if (hour == 6 && musicChanged)
                    {
                        MediaPlayer.Play(backgroundMusic);
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = 0.05f;
                        musicChanged = false;
                    }

                    //the inventory
                    inventoryButton.Update();

                    //the pause button
                    pauseButton.Update();

                    //stair button
                    testStairsButton.Update();

                    //door button
                    doorButton.Update();

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
                    
                    // Main Menu Background
                    GraphicsDevice.Clear(Color.Red);

                    // Sets the bottom button's new position
                    bottomButton.Position = new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                    330
                    , menuButtonTexture.Width, menuButtonTexture.Height);

                    // Sets the top button's new position
                    topButton.Position = new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                    250
                    , menuButtonTexture.Width, menuButtonTexture.Height);

                    //Main Menu Play Button
                    topButton.Draw(_spriteBatch, "");

                    // Prints the title
                    _spriteBatch.DrawString(titleFont, "Project Silver", new Vector2(120, 100), Color.Black);

                    //Hardcoded to center currently, will patch in future
                    _spriteBatch.DrawString(font, "PLAY", new Vector2(370, 270), Color.Black);

                    //Exit game
                    bottomButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "EXIT", new Vector2(370, 350), Color.Black);

                    break;
                #endregion
                case State.Instructions:
                    #region Instructions
                    // Instructions background
                    GraphicsDevice.Clear(Color.DarkCyan);

                    // Sets the bottom button's new position
                    bottomButton.Position = new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                        398,
                        bottomButton.Position.Width, bottomButton.Position.Height);

                    // Instructions text
                    _spriteBatch.DrawString(font, "Story Overview\nThe night is still young when you " +
                        "are invited to a celebratory party by\nthe esteemed James Atkins. You, amid " +
                        "his colleagues, are there\ncelebrating the discovery of a new star system, but" +
                        " perhaps that has\nsparked some jealously among them? Once the party goes awry," +
                        " it is\nup to you to answer the classic question... who dunnit?" +
                        "\n\nControls and Interface\nUse A and D to move left or right\nClick on NPCs " +
                        "to talk to them and use the spacebar to advance the text\nwhen the dialogue box" +
                        " is open.\nClick on items to pick them up and check on them in your inventory\nin " +
                        "the upper right.\nTo pause the game, click on the button next to the inventory" +
                        " icon\nOnce certain conditions are met, click the accuse button to blame an\nNPC.", new Vector2(0, 0), Color.White);

                    //draw button
                    continueButton.Draw(_spriteBatch, "");
                    _spriteBatch.DrawString(font, "CONTINUE", new Vector2(330,420), Color.Black);
          
                    break;
                #endregion
                case State.Game:
                    #region Game
                    // Debug Text
                    _spriteBatch.DrawString(font, $"{hour} PM", new Vector2(0, 0), Color.White);

                    //Draw the pause button and inventory button before switch so that it appears on all screens
                    pauseButton.Draw(_spriteBatch, "");
                    inventoryButton.Draw(_spriteBatch, "");

                    if (hour == 5)
                    {
                        switch (currentRoom)
                        {
                            case Rooms.Room1:
                                GraphicsDevice.Clear(Color.Navy);

                                if (currentTime < 115)
                                {
                                    clara.Draw(_spriteBatch);
                                    edith.Draw(_spriteBatch);
                                    elizabeth.Draw(_spriteBatch);
                                    summer.Draw(_spriteBatch);
                                    edward.Draw(_spriteBatch);
                                    frank.Draw(_spriteBatch);
                                    james.Draw(_spriteBatch);
                                    ernest.Draw(_spriteBatch);
                                    player.Draw(_spriteBatch);

                                    //show npc dialogue
                                    if (clara.IsTalking)
                                    {
                                        clara.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (edith.IsTalking)
                                    {
                                        edith.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (elizabeth.IsTalking)
                                    {
                                        elizabeth.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (summer.IsTalking)
                                    {
                                        summer.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (edward.IsTalking)
                                    {
                                        edward.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (frank.IsTalking)
                                    {
                                        frank.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (james.IsTalking)
                                    {
                                        james.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }

                                    //show npc dialogue
                                    if (ernest.IsTalking)
                                    {
                                        ernest.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                    }
                                }
                                else
                                {
                                    GraphicsDevice.Clear(Color.Black);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // When in game state, check for what room state you are in
                        switch (currentRoom)
                        {
                            case Rooms.Room1:
                                GraphicsDevice.Clear(Color.Navy);

                                // Clara dies at 8
                                if (hour < 8)
                                {
                                    clara.Draw(_spriteBatch);
                                }
                                // Earnest dies at 7
                                if (hour < 7)
                                {
                                    ernest.Draw(_spriteBatch);
                                }
                                
                                _spriteBatch.Draw(corpseTexture, new Rectangle(windowWidth / 2 - 56, windowHeight / 2 + 40, 112, 20), Color.White);

                                //draw player
                                player.Draw(_spriteBatch);

                                //show npc dialogue
                                if (clara.IsTalking && clara.BeingDrawn)
                                {
                                    clara.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

                                    if (items[0].PickedUp)
                                    {
                                        accuseButton.Draw(_spriteBatch);
                                    }
                                }

                                //show npc dialogue
                                if (ernest.IsTalking && ernest.BeingDrawn)
                                {
                                    ernest.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

                                    if (items[0].PickedUp)
                                    {
                                        accuseButton.Draw(_spriteBatch);
                                    }
                                }

                                break;
                            case Rooms.Room2:

                                GraphicsDevice.Clear(Color.DarkOliveGreen);

                                //draw stairs and npc
                                testStairsButton.Draw(_spriteBatch);
                                testStairsButton.Position = new Rectangle(0, 25, 114, 309);
                                edith.Draw(_spriteBatch);

                                //draw player
                                player.Draw(_spriteBatch);

                                //show npc dialogue
                                if (edith.IsTalking && edith.BeingDrawn)
                                {
                                    edith.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

                                    if (items[0].PickedUp)
                                    {
                                        accuseButton.Draw(_spriteBatch);
                                    }
                                }



                                break;
                            case Rooms.Room3:

                                GraphicsDevice.Clear(Color.Gray);

                                // Elizabeth dies at 9
                                if (hour < 9)
                                {
                                    elizabeth.Draw(_spriteBatch);
                                }
                                
                                //draw player
                                player.Draw(_spriteBatch);

                                //draw npc dialogue
                                if (elizabeth.IsTalking && elizabeth.BeingDrawn)
                                {
                                    elizabeth.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

                                    if (items[0].PickedUp)
                                    {
                                        accuseButton.Draw(_spriteBatch);
                                    }
                                }

                                doorButton.Draw(_spriteBatch);



                                break;
                            case Rooms.Room4:
                                GraphicsDevice.Clear(Color.Black);

                                //draw stairs and npc
                                testStairsButton.Draw(_spriteBatch);
                                testStairsButton.Position = new Rectangle(0, 400, 114, 309);

                                summer.Draw(_spriteBatch);

                                //draw player
                                player.Draw(_spriteBatch);

                                //draw npc dialogue
                                if (summer.IsTalking && summer.BeingDrawn)
                                {
                                    summer.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

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



                                break;
                            case Rooms.Room5:
                                GraphicsDevice.Clear(Color.DarkSlateGray);

                                // Edward dies at 10
                                if (hour < 10)
                                {
                                    edward.Draw(_spriteBatch);
                                }
                                
                                //draw player
                                player.Draw(_spriteBatch);

                                //draw npc dialogue
                                if (edward.IsTalking && edward.BeingDrawn)
                                {
                                    edward.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

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



                                break;
                            case Rooms.Room6:
                                GraphicsDevice.Clear(Color.DarkViolet);

                                // Frank dies at 11
                                if (hour < 11)
                                {
                                    frank.Draw(_spriteBatch);
                                }

                                //draw player
                                player.Draw(_spriteBatch);

                                //draw npc dialogue
                                if (frank.IsTalking && frank.BeingDrawn)
                                {
                                    frank.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);

                                    if (items[0].PickedUp)
                                    {
                                        accuseButton.Draw(_spriteBatch);
                                    }
                                }



                                break;
                            case Rooms.Room7:
                                GraphicsDevice.Clear(Color.Khaki);
                                _spriteBatch.Draw(desk, new Rectangle(windowWidth / 2 - 156, windowHeight / 2 - 110, 156, 110), Color.White);
                                doorButton.Draw(_spriteBatch);

                                //draw npc
                                document.Draw(_spriteBatch);

                                player.Draw(_spriteBatch);

                                //show npc dialogue
                                if (document.IsTalking)
                                {
                                    document.Speak(_spriteBatch, font, dialogueBox, player, hour, items[0]);
                                }


                                break;
                            default:
                                break;
                        }
                    }

                    

                    break;
                #endregion
                case State.Inventory:
                    #region Inventory
                    Vector2 startPoint = new Vector2(windowWidth / 8, windowHeight / 8); // Start Point for displaying items
                    //inventory text
                    GraphicsDevice.Clear(Color.Green);
                    _spriteBatch.DrawString(font, "You are now in the inventory", new Vector2(0, 0), Color.White);

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

                    // Sets the bottom button's new position
                    bottomButton.Position = new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                    208
                    , menuButtonTexture.Width, menuButtonTexture.Height);

                    // Sets the top button's new position
                    topButton.Position = new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                    129
                    , menuButtonTexture.Width, menuButtonTexture.Height);

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
                        _spriteBatch.DrawString(font, "You correctly figured out the murderer!", new Vector2(160, 140), Color.White);
                    }
                    //did not guess correctly, or ran out of time
                    else
                    {
                        _spriteBatch.DrawString(font, "You ran out of time or accused an innocent!", new Vector2(150, 140), Color.White);
                    }

                    // Sets the bottom button's new position
                    bottomButton.Position = new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                    210
                    , menuButtonTexture.Width, menuButtonTexture.Height);

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
                return (SingleMousePress(mState) && HoverLogic());
            }
            else // Mouse is not intersecting obj
            {
                return false;
            }
        }

        /// <summary>
        /// Hover logic to change mouse cursor
        /// </summary>
        private bool HoverLogic()
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
            

            
            return hoveredOver;
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
            if (SingleKeyPress(Keys.P, kbState) || continueButton.BeenClicked)
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
            if (clara.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                clara.DialogueNum++;
            }

            //progress through npc dialogue
            if (edith.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                edith.DialogueNum++;
            }

            //progress through npc dialogue
            if (elizabeth.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                elizabeth.DialogueNum++;
            }

            //progress through npc dialogue
            if (summer.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                summer.DialogueNum++;
            }

            //progress through npc dialogue
            if (edward.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                edward.DialogueNum++;
            }

            //progress through npc dialogue
            if (frank.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                frank.DialogueNum++;
            }

            //progress through npc dialogue
            if (james.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                james.DialogueNum++;
            }

            //progress through npc dialogue
            if (ernest.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                ernest.DialogueNum++;
            }

            //progress through document
            if (document.IsTalking && SingleKeyPress(Keys.Space, kbState))
            {
                document.DialogueNum++;
            }
            #endregion

            #region FSM Switching
            // Simulate room movement 
            switch (currentRoom)
            {
                case Rooms.Room1:

                    player.Move(kbState, currentRoom);

                    if (hour == 5)
                    {
                        //stop user from walking off screen
                        if (player.Position.X < 0)
                        {
                            player.Position = new Vector2(0, player.Position.Y);
                        }
                        if (player.Position.X > windowWidth - 50)
                        {
                            player.Position = new Vector2(windowWidth - 50, player.Position.Y);
                        }
                    }

                    //if you walk to the left, go to room 2
                    else if (player.Position.X < 0)
                    {
                        // Change Room
                        currentRoom = Rooms.Room2;
                        // Re-Orient Player
                        player.Right();

                        clara.IsTalking = false;
                    }

                    //if you walk to the right, go to room 3
                    else if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room3;
                        player.Left();

                        clara.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(clara))
                    {
                        clara.IsTalking = !clara.IsTalking;
                        edith.IsTalking = false;
                        elizabeth.IsTalking = false;
                        summer.IsTalking = false;
                        edward.IsTalking = false;
                        frank.IsTalking = false;
                        james.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(edith))
                    {
                        edith.IsTalking = !edith.IsTalking;
                        clara.IsTalking = false;
                        elizabeth.IsTalking = false;
                        summer.IsTalking = false;
                        edward.IsTalking = false;
                        frank.IsTalking = false;
                        james.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(elizabeth))
                    {
                        elizabeth.IsTalking = !elizabeth.IsTalking;
                        edith.IsTalking = false;
                        clara.IsTalking = false;
                        summer.IsTalking = false;
                        edward.IsTalking = false;
                        frank.IsTalking = false;
                        james.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(summer))
                    {
                        summer.IsTalking = !summer.IsTalking;
                        edith.IsTalking = false;
                        elizabeth.IsTalking = false;
                        clara.IsTalking = false;
                        edward.IsTalking = false;
                        frank.IsTalking = false;
                        james.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(edward))
                    {
                        edward.IsTalking = !edward.IsTalking;
                        edith.IsTalking = false;
                        elizabeth.IsTalking = false;
                        summer.IsTalking = false;
                        clara.IsTalking = false;
                        frank.IsTalking = false;
                        james.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(frank))
                    {
                        frank.IsTalking = !frank.IsTalking;
                        edith.IsTalking = false;
                        elizabeth.IsTalking = false;
                        summer.IsTalking = false;
                        edward.IsTalking = false;
                        clara.IsTalking = false;
                        james.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(james))
                    {
                        james.IsTalking = !james.IsTalking;
                        edith.IsTalking = false;
                        elizabeth.IsTalking = false;
                        summer.IsTalking = false;
                        edward.IsTalking = false;
                        frank.IsTalking = false;
                        clara.IsTalking = false;
                        ernest.IsTalking = false;
                    }

                    //toggle npc talking state
                    if (Clicked(ernest))
                    {
                        ernest.IsTalking = !ernest.IsTalking;
                        edith.IsTalking = false;
                        elizabeth.IsTalking = false;
                        summer.IsTalking = false;
                        edward.IsTalking = false;
                        frank.IsTalking = false;
                        james.IsTalking = false;
                        clara.IsTalking = false;
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked && items[0].PickedUp && (clara.IsTalking
                        || ernest.IsTalking))
                    {
                        //if they are murderer, win
                        if (clara.IsMurderer) 
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
                        edith.IsTalking = false;
                        currentRoom = Rooms.Room1;
                        player.Left();
                    }

                    //if you click on stairs, move to room 4
                    if (testStairsButton.BeenClicked)
                    {
                        currentRoom = Rooms.Room4;
                        edith.IsTalking = false;
                        player.Left();
                    }

                    //toggle npc talking state
                    if (Clicked(edith))
                    {
                        edith.IsTalking = !edith.IsTalking;
                    }

                    //stop user from walking off screen
                    if (player.Position.X < 0)
                    {
                        player.Position = new Vector2(0, player.Position.Y);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked && items[0].PickedUp && edith.IsTalking)
                    {
                        //if they are murderer, win
                        if (edith.IsMurderer)
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
                        elizabeth.IsTalking = false;
                        player.Right();
                    }


                    //toggle npc talking state
                    if (Clicked(elizabeth))
                    {
                        elizabeth.IsTalking = !elizabeth.IsTalking;
                    }

                    if (doorButton.BeenClicked && items[1].PickedUp)
                    {
                        currentRoom = Rooms.Room7;
                        player.Center();
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked && items[0].PickedUp && elizabeth.IsTalking)
                    {
                        //if they are muderer, win
                        if (elizabeth.IsMurderer)
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
                        summer.IsTalking = false;
                        player.Left();
                    }

                    //if you move to right, go to room 5
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room5;
                        summer.IsTalking = false;
                        player.Left();
                    }

                    //toggle npc talking state
                    if (Clicked(summer))
                    {
                        summer.IsTalking = !summer.IsTalking;
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
                    if (accuseButton.BeenClicked && items[0].PickedUp && summer.IsTalking)
                    {
                        //if they are the murderer, win
                        if (summer.IsMurderer)
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
                        edward.IsTalking = false;
                        player.Right();
                    }

                    //if you move to right, go to room 6
                    if (player.Position.X > windowWidth)
                    {
                        currentRoom = Rooms.Room6;
                        edward.IsTalking = false;
                        player.Left();
                    }

                    //toggle npc talking state
                    if (Clicked(edward))
                    {
                        edward.IsTalking = !edward.IsTalking;
                    }

                    // If you click on the knife, toggle it
                    if (Clicked(items[0]))
                    {
                        items[0].PickedUp = true;
                        player.Inventory.Add(items[0]);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked && items[0].PickedUp && edward.IsTalking)
                    {
                        //if they are the murderer, win
                        if (edward.IsMurderer)
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
                        frank.IsTalking = false;
                        player.Right();
                    }

                    //toggle npc talking state
                    if (Clicked(frank))
                    {
                        frank.IsTalking = !frank.IsTalking;
                    }

                    //stop user from walking off screen
                    if (player.Position.X > windowWidth - 45)
                    {
                        player.Position = new Vector2(windowWidth - 45, player.Position.Y);
                    }

                    //if you accuse someone
                    if (accuseButton.BeenClicked && items[0].PickedUp && frank.IsTalking)
                    {
                        //if they are the murderer, win
                        if (frank.IsMurderer)
                        {
                            won = true;
                        }

                        //go to end menu
                        currentState = State.EndMenu;
                    }

                    break;
                case Rooms.Room7:
                    player.Move(kbState, currentRoom);

                    if (doorButton.BeenClicked && items[1].PickedUp)
                    {
                        currentRoom = Rooms.Room3;
                        document.IsTalking = false;
                        player.Center();
                    }

                    if (Clicked(document))
                    {
                        document.IsTalking = !document.IsTalking;
                    }
                    //stop user from walking off screen
                    if (player.Position.X < 0)
                    {
                        player.Position = new Vector2(0, player.Position.Y);
                    }
                    if (player.Position.X > windowWidth - 50)
                    {
                        player.Position = new Vector2(windowWidth - 50, player.Position.Y);
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
                    doorButton.BeingDrawn = false;

                    clara.BeingDrawn = false;
                    edith.BeingDrawn = false;
                    elizabeth.BeingDrawn = false;
                    summer.BeingDrawn = false;
                    edward.BeingDrawn = false;
                    frank.BeingDrawn = false;
                    james.BeingDrawn = false;
                    ernest.BeingDrawn = false;
                    document.BeingDrawn = false;

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
                    doorButton.BeingDrawn = false;

                    clara.BeingDrawn = false;
                    edith.BeingDrawn = false;
                    elizabeth.BeingDrawn = false;
                    summer.BeingDrawn = false;
                    edward.BeingDrawn = false;
                    frank.BeingDrawn = false;
                    james.BeingDrawn = false;
                    ernest.BeingDrawn = false;
                    document.BeingDrawn = false;

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
                            if (hour == 5)
                            {
                                clara.BeingDrawn = true;
                                edith.BeingDrawn = true;
                                elizabeth.BeingDrawn = true;
                                summer.BeingDrawn = true;
                                edward.BeingDrawn = true;
                                frank.BeingDrawn = true;
                                james.BeingDrawn = true;
                                ernest.BeingDrawn = true;
                                document.BeingDrawn = false;
                                testStairsButton.BeingDrawn = false;
                                accuseButton.BeingDrawn = false;
                                items[0].BeingDrawn = false;
                                items[1].BeingDrawn = false;
                                doorButton.BeingDrawn = false;
                                // During the first death, no one can be clicked
                                if (currentTime >= 115)
                                {
                                    clara.BeingDrawn = false;
                                    edith.BeingDrawn = false;
                                    elizabeth.BeingDrawn = false;
                                    summer.BeingDrawn = false;
                                    edward.BeingDrawn = false;
                                    frank.BeingDrawn = false;
                                    james.BeingDrawn = false;
                                    ernest.BeingDrawn = false;
                                    document.BeingDrawn = false;
                                    testStairsButton.BeingDrawn = false;
                                    accuseButton.BeingDrawn = false;
                                    items[0].BeingDrawn = false;
                                    items[1].BeingDrawn = false;
                                    doorButton.BeingDrawn = false;
                                }
                            }
                            else
                            {
                                if (hour < 8)
                                {
                                    clara.BeingDrawn = true;
                                }
                                else
                                {
                                    clara.BeingDrawn = false;
                                }
                                edith.BeingDrawn = false;
                                elizabeth.BeingDrawn = false;
                                summer.BeingDrawn = false;
                                edward.BeingDrawn = false;
                                frank.BeingDrawn = false;
                                james.BeingDrawn = false;
                                if (hour < 7)
                                {
                                    ernest.BeingDrawn = true;
                                }
                                else
                                {
                                    ernest.BeingDrawn = false;
                                }
                                document.BeingDrawn = false;
                                testStairsButton.BeingDrawn = false;
                                accuseButton.BeingDrawn = false;
                                items[0].BeingDrawn = false;
                                items[1].BeingDrawn = false;
                                doorButton.BeingDrawn = false;
                            }                           
                            break;

                        case Rooms.Room2:
                            clara.BeingDrawn = false;
                            edith.BeingDrawn = true;
                            elizabeth.BeingDrawn = false;
                            summer.BeingDrawn = false;
                            edward.BeingDrawn = false;
                            frank.BeingDrawn = false;
                            james.BeingDrawn = false;
                            ernest.BeingDrawn = false;
                            document.BeingDrawn = false;
                            testStairsButton.BeingDrawn = true;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            doorButton.BeingDrawn = false;
                            break;

                        case Rooms.Room3:
                            clara.BeingDrawn = false;
                            edith.BeingDrawn = false;
                            if (hour < 9)
                            {
                                elizabeth.BeingDrawn = true;
                            }
                            else
                            {
                                elizabeth.BeingDrawn = false;
                            }
                            summer.BeingDrawn = false;
                            edward.BeingDrawn = false;
                            frank.BeingDrawn = false;
                            james.BeingDrawn = false;
                            ernest.BeingDrawn = false;
                            document.BeingDrawn = false;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            doorButton.BeingDrawn = true;
                            break;

                        case Rooms.Room4:
                            clara.BeingDrawn = false;
                            edith.BeingDrawn = false;
                            elizabeth.BeingDrawn = false;
                            summer.BeingDrawn = true;
                            edward.BeingDrawn = false;
                            frank.BeingDrawn = false;
                            james.BeingDrawn = false;
                            ernest.BeingDrawn = false;
                            document.BeingDrawn = false;
                            testStairsButton.BeingDrawn = true;
                            accuseButton.BeingDrawn = false;
                            doorButton.BeingDrawn = false;
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
                            clara.BeingDrawn = false;
                            edith.BeingDrawn = false;
                            elizabeth.BeingDrawn = false;
                            summer.BeingDrawn = false;
                            if (hour < 10)
                            {
                               edward.BeingDrawn = true;
                            }
                            else
                            {
                               edward.BeingDrawn = false;
                            }
                            frank.BeingDrawn = false;
                            james.BeingDrawn = false;
                            ernest.BeingDrawn = false;
                            document.BeingDrawn = false;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            doorButton.BeingDrawn = false;
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
                            clara.BeingDrawn = false;
                            edith.BeingDrawn = false;
                            elizabeth.BeingDrawn = false;
                            summer.BeingDrawn = false;
                            edward.BeingDrawn = false;
                            if (hour < 11)
                            {
                                frank.BeingDrawn = true;
                            }
                            else
                            {
                                frank.BeingDrawn = false;
                            }
                            james.BeingDrawn = false;
                            ernest.BeingDrawn = false;
                            document.BeingDrawn = false;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            doorButton.BeingDrawn = false;
                            break;
                        case Rooms.Room7:
                            clara.BeingDrawn = false;
                            edith.BeingDrawn = false;
                            elizabeth.BeingDrawn = false;
                            summer.BeingDrawn = false;
                            edward.BeingDrawn = false;
                            frank.BeingDrawn = false;
                            james.BeingDrawn = false;
                            ernest.BeingDrawn = false;
                            document.BeingDrawn = true;
                            testStairsButton.BeingDrawn = false;
                            accuseButton.BeingDrawn = false;
                            items[0].BeingDrawn = false;
                            items[1].BeingDrawn = false;
                            doorButton.BeingDrawn = true;
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
                    doorButton.BeingDrawn = false;

                    clara.BeingDrawn = false;
                    edith.BeingDrawn = false;
                    elizabeth.BeingDrawn = false;
                    summer.BeingDrawn = false;
                    edward.BeingDrawn = false;
                    frank.BeingDrawn = false;
                    james.BeingDrawn = false;
                    ernest.BeingDrawn = false;
                    document.BeingDrawn = false;

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
                    doorButton.BeingDrawn = false;

                    clara.BeingDrawn = false;
                    edith.BeingDrawn = false;
                    elizabeth.BeingDrawn = false;
                    summer.BeingDrawn = false;
                    edward.BeingDrawn = false;
                    frank.BeingDrawn = false;
                    james.BeingDrawn = false;
                    ernest.BeingDrawn = false;
                    document.BeingDrawn = false;

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
                    doorButton.BeingDrawn = false;

                    clara.BeingDrawn = false;
                    edith.BeingDrawn = false;
                    elizabeth.BeingDrawn = false;
                    summer.BeingDrawn = false;
                    edward.BeingDrawn = false;
                    frank.BeingDrawn = false;
                    james.BeingDrawn = false;
                    ernest.BeingDrawn = false;
                    document.BeingDrawn = false;

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
            corpseTexture = Content.Load<Texture2D>("JamesAtkinsCorpse");
            ernestBoydTexture = Content.Load<Texture2D>("ErnestBoyd");
            edwardCampellTexture = Content.Load<Texture2D>("EdwardCampbell");
            frankEspinozaTexture = Content.Load<Texture2D>("FrankEspinoza");
            jamesAtkinsTexture = Content.Load<Texture2D>("JamesAtkinsAlive");
            documentTexture = Content.Load<Texture2D>("document");
            // testNPCTexture = Content.Load<Texture2D>("npc");

            // Initializes the characters and adds them to the gameObjects list
            player = new Player("Char", playerPos, playerTexture, windowHeight, windowWidth);
            clara = new NPC("Clara Farley", false, false, new Rectangle(550, 200, 40, 107), claraFarleyTexture);
            gameObjects.Add(clara);
            edith = new NPC("Edith Espinoza", false, false, new Rectangle(500, 200, 40, 109), edithEspinozaTexture);
            gameObjects.Add(edith);
            elizabeth = new NPC("Elizabeth Maxwell", false, false, new Rectangle(450, 200, 40, 107), elizabethMaxwellTexture);
            gameObjects.Add(elizabeth);
            summer = new NPC("Summer Hines", true, false, new Rectangle(400, 200, 40, 107), summerHinesTexture);
            gameObjects.Add(summer);
            edward = new NPC("Edward Campbell", false, false, new Rectangle(350, 200, 40, 107), edwardCampellTexture);
            gameObjects.Add(edward);
            frank = new NPC("Frank Espinoza", false, false, new Rectangle(300, 200, 40, 107), frankEspinozaTexture);
            gameObjects.Add(frank);
            ernest = new NPC("Ernest Boyd", false, false, new Rectangle(200, 200, 40, 107), ernestBoydTexture);
            gameObjects.Add(ernest);
            james = new NPC("James Atkins", false, false, new Rectangle(250, 200, 40, 107), jamesAtkinsTexture);
            gameObjects.Add(james);
            document = new NPC("Document", false, false, new Rectangle(windowWidth / 2 - 50, windowHeight / 2 - 93, 29, 39), documentTexture);
            gameObjects.Add(document);

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
            doorTexture = Content.Load<Texture2D>("door");
            desk = Content.Load<Texture2D>("desk");

            // Initializes the buttons
            topButton = new Button("Menu", menuButtonTexture, font,
                new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2,
                250
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
                330
                , menuButtonTexture.Width, menuButtonTexture.Height));
            gameObjects.Add(bottomButton);

            continueButton = new Button("Menu", menuButtonTexture, font,
                new Rectangle((GraphicsDevice.Viewport.Width / 2) - menuButtonTexture.Width / 2, 400,
                menuButtonTexture.Width, menuButtonTexture.Height));
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

            doorButton = new Button("", doorTexture, font,
                new Rectangle(100, 167, 92, 142));
            gameObjects.Add(doorButton);

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
            clara.IsTalking = false;
            edith.IsTalking = false;
            elizabeth.IsTalking = false;
            summer.IsTalking = false;
            edward.IsTalking = false;
            frank.IsTalking = false;
            james.IsTalking = false;
            ernest.IsTalking = false;
            document.IsTalking = false;
            clara.DialogueNum = 0;
            edith.DialogueNum = 0;
            elizabeth.DialogueNum = 0;
            summer.DialogueNum = 0;
            edward.DialogueNum = 0;
            frank.DialogueNum = 0;
            james.DialogueNum = 0;
            ernest.DialogueNum = 0;
            document.DialogueNum = 0;
        }
        #endregion
    }
}