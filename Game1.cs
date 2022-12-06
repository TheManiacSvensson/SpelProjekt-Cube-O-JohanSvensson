using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Threading;


namespace Cubo_o_n_anti_cube
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Background
        Texture2D Background;
        //Sprites
        Texture2D MainCube;
        Texture2D AntiCube;
        Texture2D Skully;
        Rectangle MainCubeRectangle;

        Vector2 MainCubePlacement = new Vector2(0, 440);
        Vector2 AnticubePlacement = new Vector2(760, 440);

        //Input
        KeyboardState OldKeyboardInput = Keyboard.GetState();
        KeyboardState Keyboardinput = Keyboard.GetState();

        //Ints & floats
        int Health = 1;
        int TimeTheNumber = 0;
        int Lifetimer = 0;
        int Difficulty = 0;
        int Scene = 0;
        int Score = 0;
        float AntiCubeSpeed = 0.040f;

        //Soundeffects & Music
        SoundEffect SpottedAlert;

        //Message placement & fonts
        SpriteFont SideMessages;
        SpriteFont AnyButtonFont;
        Vector2 TopleftSide = new Vector2(0, 0);
        Vector2 TimerPlacement = new Vector2(70, 0);
        Vector2 TimeTheStringPlacement = new Vector2(18, 0);
        Vector2 ScorePlacement = new Vector2(90, 30);
        Vector2 ScoreTheStringPlacement = new Vector2(18, 30);
        Vector2 DifficultyPlacement = new Vector2(96, 21);
        Vector2 DifficultyTheStringPlacement = new Vector2(10, 20);
        Vector2 DifficultyPromptPlacement = new Vector2(160, 22);
        Vector2 OhnoMessagePlacement = new Vector2(0, 400);
        Vector2 RulePlacement = new Vector2(100, 0);
        Vector2 WinMessagePlacement = new Vector2(380, 42);
        Vector2 EndTimerPlacement = new Vector2(380, 60);
        Vector2 AnybuttonPlacement = new Vector2(240, 240);


        //Messages
        string AnybuttonMessage = ("PRESS SPACE TO LAUNCH GAME!");
        string TheRules = ("The rules are simple... Survive as long as you can and don't get caught");
        string TimeTheString = ("Time:");
        string OHNOmessage = ("Oh no... Cluster saw me, i have to run!");
        string Looser = ("You lost");
        string Escapeforexit = ("Press esc to ex it the game");
        string GameDifficultyPrompt = ("To increase the difficulty press B, N to lower(there is 5 difficulties)");
        string DifficultyTheString = ("Difficulty:");
        string ScoreTheString = ("Score:");
        

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            //Sprite rectangles
            MainCubeRectangle = new Rectangle(0, 440, 40, 40);

            MediaPlayer.IsRepeating = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Sprites & Fonts
            MainCube = Content.Load<Texture2D>("Cube-O");
            AntiCube = Content.Load<Texture2D>("Anti-cube");
            SideMessages = Content.Load<SpriteFont>("File");
            AnyButtonFont = Content.Load<SpriteFont>("Arial");
            Skully = Content.Load<Texture2D>("Dryskully");
            Background = Content.Load <Texture2D>("Grassfieldgreen");

            //Music Credit: SoundImage:org
            SpottedAlert = Content.Load<SoundEffect>("tindeck_1");
        }

        protected override void Update(GameTime gameTime)
        {
            //Exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Input refresh
            OldKeyboardInput = Keyboardinput;
            Keyboardinput = Keyboard.GetState();
            //Stop timer update in main menu
            if (Scene == 0)
            {
                TimeTheNumber = 0;
                Lifetimer = 0;

                DifficultySettings();
            }
            //Gamerules
            if (Scene == 1)
            {

                AiMovement();

                PlayerMovement();

                GameBorder();


            }
            //Timer & health
            if (TimeTheNumber > 1)
            {
               Lifetimer = TimeTheNumber/60;
            }
            if (TimeTheNumber > -1)
            {
                TimeTheNumber++;
            }
            if (MainCubeRectangle.Contains(AnticubePlacement))
            {
                Health--;
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            //Change scene
            if (Scene == 0)
            {
                DrawMenu();
            }
            switch (Scene)
            {
                case 0:
                    DrawMenu();
                    break;
                case 1:
                    DrawGame();
                    break;
                case 2:
                    DrawEndScreen();
                    break;
                default:
                    break;
            }
            //Lore text & Death
            spriteBatch.Begin();
            if (Lifetimer < 6 && Scene == 1)
            {
                spriteBatch.DrawString(SideMessages, OHNOmessage, new Vector2(MainCubeRectangle.X, MainCubeRectangle.Y - 50), Color.Black);
            }
            if (Health <= 0)
            {
                spriteBatch.Draw(Skully, MainCubeRectangle, Color.White);
                spriteBatch.DrawString(SideMessages, Looser, WinMessagePlacement, Color.Black);
                spriteBatch.DrawString(SideMessages, Escapeforexit, EndTimerPlacement, Color.Black);
                TimeTheNumber = -11;
                ChangeSceneMethod(2);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        void DrawMenu()
        {
            //Draw the main menu
            GraphicsDevice.Clear(Color.Aquamarine);
            spriteBatch.Begin();
            spriteBatch.DrawString(AnyButtonFont, AnybuttonMessage, AnybuttonPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, GameDifficultyPrompt, DifficultyPromptPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, Difficulty.ToString(), DifficultyPlacement, Color.Red);
            spriteBatch.DrawString(SideMessages, DifficultyTheString, DifficultyTheStringPlacement, Color.Red);
            spriteBatch.DrawString(SideMessages, TheRules, RulePlacement, Color.Black);
            if (SpacePressed())
            {
                ChangeSceneMethod(1);
                SpottedAlert.Play();
            }
            spriteBatch.End();
        }
        void DrawGame()
        {
            GraphicsDevice.Clear(Color.LightGreen);

            //Load up all the sprites and messages when starting the game
            spriteBatch.Begin();
            spriteBatch.Draw(Background, TopleftSide, Color.White);
            spriteBatch.DrawString(SideMessages, TimeTheString, TimeTheStringPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, Lifetimer.ToString(), TimerPlacement, Color.Black);
            spriteBatch.Draw(MainCube, MainCubeRectangle, Color.White);
            spriteBatch.Draw(AntiCube, AnticubePlacement, Color.White);
            spriteBatch.End();
        }
        void DrawEndScreen()
        {
            //End screen
            Score = Difficulty * Lifetimer;
            GraphicsDevice.Clear(Color.MediumAquamarine);
            spriteBatch.Begin();
            spriteBatch.DrawString(SideMessages, Lifetimer.ToString(), TimerPlacement, Color.DarkGreen);
            spriteBatch.DrawString(SideMessages, TimeTheString, TimeTheStringPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, Score.ToString(), ScorePlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, ScoreTheString, ScoreTheStringPlacement, Color.Black);
            spriteBatch.End();
        }
        void ChangeSceneMethod(int ChangeSceneTo)
        {
            //Change the scene
            Scene = ChangeSceneTo;
        }
        void DifficultySettings ()
        {
            //Difficulty setting (Speed for the enemy)
            if (Keyboardinput.IsKeyDown(Keys.B) && OldKeyboardInput.IsKeyUp(Keys.B) && Scene == 0)
            {
                Difficulty++;
            }
            switch (Difficulty)
            {
                case 0:
                    AntiCubeSpeed = 0.04f;
                        break;
                    case 1:
                    AntiCubeSpeed = 0.06f;
                    break;
                        case 2:
                    AntiCubeSpeed = 0.068f;
                    break;
                            case 3:
                    AntiCubeSpeed = 0.071f;
                    break;
                                case 4:
                    AntiCubeSpeed = 0.0725f;
                    break;
                                    case 5:
                    AntiCubeSpeed = 0.0735f;
                    break;
                                        case 6:
                    Difficulty = 5;
                    break;


                default:
                    break;
            }
            if (Keyboardinput.IsKeyDown(Keys.N) && OldKeyboardInput.IsKeyUp(Keys.N) && Scene == 0)
            {
                Difficulty--;
            }
            if (Difficulty < 0)
            {
                Difficulty = 0;
            }
        }
        bool SpacePressed()
        {
            //Checking if space is pressed when starting the game in the main menu
            if (Keyboardinput.IsKeyDown(Keys.Space) && OldKeyboardInput.IsKeyDown(Keys.Space))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void AiMovement()
        {
            //Ai movement
            float DifferenceX = MainCubeRectangle.X - (AnticubePlacement.X + (AntiCube.Width - MainCube.Width));
            float DifferenceY = MainCubeRectangle.Y - (AnticubePlacement.Y + (AntiCube.Height - MainCube.Height));
            if (AnticubePlacement.X > MainCubeRectangle.X)
            {
                AnticubePlacement.X -= 3;
            }
            if (Lifetimer >= 4)
            {
                AnticubePlacement.X += DifferenceX * AntiCubeSpeed;
                AnticubePlacement.Y += DifferenceY * AntiCubeSpeed;
            }
            if (AntiCubeSpeed <= 0.0735f)
            {
                AntiCubeSpeed += 0.0001f / 120f;
            }
        }
        void PlayerMovement()
        {
            //Movement
            if (Keyboardinput.IsKeyDown(Keys.W) || Keyboardinput.IsKeyDown(Keys.Up))
            {
                MainCubeRectangle.Y -= 3;
            }
            if (Keyboardinput.IsKeyDown(Keys.A) || Keyboardinput.IsKeyDown(Keys.Left))
            {
                MainCubeRectangle.X -= 3;
            }
            if (Keyboardinput.IsKeyDown(Keys.S) || Keyboardinput.IsKeyDown(Keys.Down))
            {
                MainCubeRectangle.Y += 3;
            }
            if (Keyboardinput.IsKeyDown(Keys.D) || Keyboardinput.IsKeyDown(Keys.Right))
            {
                MainCubeRectangle.X += 3;
            }
        }
        void GameBorder()
        {
            //Player Border
            if (MainCubeRectangle.X < 1)
            {
                MainCubeRectangle.X = 0;
            }
            if (MainCubeRectangle.Y < 1)
            {
                MainCubeRectangle.Y = 0;
            }
            if (MainCubeRectangle.X >= 760)
            {
                MainCubeRectangle.X = 760;
            }
            if (MainCubeRectangle.Y >= 440)
            {
                MainCubeRectangle.Y = 440;
            }
        }
    }
}