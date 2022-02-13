using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private MouseState mState;
        private MouseState prevMState;
        private CurrentState state;
        private SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic herew
            //Game starts at the main menu by default
            state = CurrentState.MainMenu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
           // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
          //      Exit();

            kbState = Keyboard.GetState();
            mState = Mouse.GetState();

            switch (state)
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
            // TODO: Add your update logic here
            prevKbState = kbState;
            prevMState = mState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            switch (state)
            {
                case CurrentState.MainMenu:
                    _spriteBatch.DrawString(font, "Main Menu\nPress P to go to play state.", new Vector2(0, 0), Color.White);
                    GraphicsDevice.Clear(Color.Red);
                    break;

                case CurrentState.Game:
                    _spriteBatch.DrawString(font, "You are now playing the game.\nPress M to go back\nor I to go to inventory.", new Vector2(0, 0), Color.White);
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

            // TODO: Add your drawing code here
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
            else
            {
                return false;
            }
        }

        /// <summary>
        /// If the player inputs the "P" key, take them to the game
        /// </summary>
        /// <param name="kbState"></param>
        private void ProcessMainMenu(KeyboardState kbState)
        {
            if (SingleKeyPress(Keys.P, kbState))
            {
                state = CurrentState.Game;
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
                state = CurrentState.MainMenu;
            }

            if (SingleKeyPress(Keys.I, kbState))
            {
                state = CurrentState.Inventory;
            }

            if (SingleKeyPress(Keys.Escape, kbState))
            {
                state = CurrentState.PauseMenu;
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
                state = CurrentState.Game;
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
                state = CurrentState.Game;
            }

            if(SingleKeyPress(Keys.M, kbState))
            {
                state = CurrentState.MainMenu;
            }
        }
    }
}
