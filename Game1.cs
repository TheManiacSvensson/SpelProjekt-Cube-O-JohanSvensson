using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

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
        Texture2D Speedboost;
        //SpritePlacement & rectangles
        Rectangle MainCubeRectangle;
        Vector2 MainCubePlacement = new Vector2(0, 440);
        Rectangle MainCubeStartPlacement;
        Vector2 AnticubePlacement = new Vector2(760, 440);
        Vector2 AntiCubeStartPlacement;
        Rectangle SpeedBoostRectanglePlacement;
        List<Rectangle> SpeedBoostList = new List<Rectangle>();

        //Input
        KeyboardState OldKeyboardInput = Keyboard.GetState();
        KeyboardState Keyboardinput = Keyboard.GetState();

        //Ints, floats, Randoms & lists
        int Health = 1;
        int TimeTheNumber = 0;
        int Lifetimer = 0;
        int Difficulty = 0;
        int Scene = 0;
        int Score = 0;
        int UpdatesBetweenNewBoost = 660;
        int UpdatesUntilNextBoost = 0;
        int highscore;
        float AntiCubeSpeed = 0.040f;
        Random Chance = new Random(133780085);

        //Soundeffects & Music
        SoundEffect SpottedAlert;

        //Message placement & fonts
        SpriteFont SideMessages;
        SpriteFont AnyButtonFont;
        Vector2 BackgroundVector = new Vector2(0, 10);
        Vector2 TimerPlacement = new Vector2(70, 0);
        Vector2 TimeTheStringPlacement = new Vector2(18, 0);
        Vector2 ScorePlacement = new Vector2(90, 30);
        Vector2 ScoreTheStringPlacement = new Vector2(18, 30);
        Vector2 HighscorePlacement = new Vector2(116, 50);
        Vector2 HighscoretheStringPlacement = new Vector2(18, 50);
        Vector2 DifficultyPlacement = new Vector2(96, 21);
        Vector2 DifficultyTheStringPlacement = new Vector2(10, 20);
        Vector2 DifficultyPromptPlacement = new Vector2(160, 22);
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
        string SpaceToTryAgain = ("Press space to try again");
        string GameDifficultyPrompt = ("To increase the difficulty press B, N to lower");
        string DifficultyTheString = ("Difficulty:");
        string ScoreTheString = ("Score:");
        string HighscoreTheString = ("Highscore:");
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            //Sprite rectangles decleration & Start placement declaration
            MainCubeRectangle = new Rectangle(0, 440, 40, 40);
            AntiCubeStartPlacement = AnticubePlacement;
            MainCubeStartPlacement = MainCubeRectangle;
            UpdatesUntilNextBoost = 600;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Sprites, Fonts & Background loading
            MainCube = Content.Load<Texture2D>("Cube-O");
            AntiCube = Content.Load<Texture2D>("Anti-cube");
            SideMessages = Content.Load<SpriteFont>("File");
            Skully = Content.Load<Texture2D>("Dryskully");
            Speedboost = Content.Load<Texture2D>("Speedboost");
            AnyButtonFont = Content.Load<SpriteFont>("Arial");
            Background = Content.Load <Texture2D>("Grassfieldgreen");
            //Sound effect Credit: https://www.myinstants.com/en/instant/metal-gear-solid-alert/
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
            //Stop timer updates in main menu & fixes a bug with the boost spawning earlier depending on the earlier run
            if (Scene == 0)
            {
                TimeTheNumber = 0;
                Lifetimer = 0;
                UpdatesUntilNextBoost = 600;
                DifficultySettings();
                SpeedBoostReset();
            }
            //Gamerules
            if (Scene == 1)
            {
                SpeedBoostUpdater();
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
            switch (Scene)
            {
                case 0:
                    DrawMenu();
                    break;
                case 1:
                    DrawGame();
                    break;
                case 2:
                    GameScoreSystem();
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
            if (highscore > 0)
            {
                spriteBatch.DrawString(SideMessages, highscore.ToString(), HighscorePlacement, Color.Black);
                spriteBatch.DrawString(SideMessages, HighscoreTheString, HighscoretheStringPlacement, Color.Black);
            }
            spriteBatch.End();
        }
        void DrawGame()
        {
            GraphicsDevice.Clear(Color.LightGreen);
            //Load up all the sprites and messages when starting the game
            spriteBatch.Begin();
            spriteBatch.Draw(Background, BackgroundVector, Color.White);
            spriteBatch.DrawString(SideMessages, TimeTheString, TimeTheStringPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, Lifetimer.ToString(), TimerPlacement, Color.Black);
            spriteBatch.Draw(MainCube, MainCubeRectangle, Color.White);
            spriteBatch.Draw(AntiCube, AnticubePlacement, Color.White);
            if (SpeedBoostList.Count != 0 && UpdatesUntilNextBoost <= 600)
            {
                spriteBatch.Draw(Speedboost, SpeedBoostRectanglePlacement, Color.White);
            }
            spriteBatch.End();
        }
        void DrawEndScreen()
        {
            //End screen
            GraphicsDevice.Clear(Color.MediumAquamarine);
            spriteBatch.Begin();
            spriteBatch.DrawString(AnyButtonFont, SpaceToTryAgain, AnybuttonPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, Lifetimer.ToString(), TimerPlacement, Color.DarkGreen);
            spriteBatch.DrawString(SideMessages, TimeTheString, TimeTheStringPlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, Score.ToString(), ScorePlacement, Color.Black);
            spriteBatch.DrawString(SideMessages, ScoreTheString, ScoreTheStringPlacement, Color.Black);
            if (highscore >= Score && highscore > 0)
            {
                spriteBatch.DrawString(SideMessages, highscore.ToString(), HighscorePlacement, Color.Black);
                spriteBatch.DrawString(SideMessages, HighscoreTheString, HighscoretheStringPlacement, Color.Black);
            }
            if (SpacePressed())
            {
                Health = 1;
                AnticubePlacement = AntiCubeStartPlacement;
                MainCubeRectangle = MainCubeStartPlacement;
                ChangeSceneMethod(0);
            }
            spriteBatch.End();
        }
        void GameScoreSystem()
        {
            Score = Difficulty * Lifetimer;
            if (highscore < Score)
            {
                highscore = Score;
            }
        }
        void ChangeSceneMethod(int ChangeSceneTo)
        {
            //Change the scene
            Scene = ChangeSceneTo;
        }
        void SpeedBoostSpawner()
        {
            int NextSpeedBoostX = Chance.Next(0, 800 - MainCube.Width);
            int NextSpeedBoostY = Chance.Next(0, 480 - MainCube.Height);
            SpeedBoostRectanglePlacement = new Rectangle(NextSpeedBoostX, NextSpeedBoostY, MainCube.Width, MainCube.Height);
            SpeedBoostList.Add(SpeedBoostRectanglePlacement);

        }
        void SpeedBoostUpdater()
        {
            if (Scene == 1)
            {
                UpdatesUntilNextBoost--;
                if (UpdatesUntilNextBoost <= 0)
                {
                    UpdatesUntilNextBoost = UpdatesBetweenNewBoost;
                    SpeedBoostSpawner();
                }
            }
        }
        void SpeedBoostReset()
        {
            if (Scene != 1)
            {
                UpdatesUntilNextBoost = UpdatesBetweenNewBoost;
                if (SpeedBoostList.Count != 0)
                {
                    SpeedBoostList.Remove(SpeedBoostRectanglePlacement);
                }
            }
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
            if (Keyboardinput.IsKeyDown(Keys.Space) && OldKeyboardInput.IsKeyUp(Keys.Space))
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
                AnticubePlacement.X += DifferenceX * AntiCubeSpeed + 4/3;
                AnticubePlacement.Y += DifferenceY * AntiCubeSpeed;
            }
            if (AntiCubeSpeed <= 0.0735f)
            {
                AntiCubeSpeed += 0.0001f / 120f;
            }
        }
        void PlayerMovement()
        {
                if (SpeedBoostList.Count >= 1)
                {
                    if (MainCubeRectangle.Intersects(SpeedBoostRectanglePlacement))
                    {
                        for (int i = 0; i < 90; i++)
                        {
                            if (Keyboardinput.IsKeyDown(Keys.W) || Keyboardinput.IsKeyDown(Keys.Up))
                            {
                                MainCubeRectangle.Y -= 2;
                            }
                            if (Keyboardinput.IsKeyDown(Keys.A) || Keyboardinput.IsKeyDown(Keys.Left))
                            {
                                MainCubeRectangle.X -= 2;
                            }
                            if (Keyboardinput.IsKeyDown(Keys.S) || Keyboardinput.IsKeyDown(Keys.Down))
                            {
                                MainCubeRectangle.Y += 2;
                            }
                            if (Keyboardinput.IsKeyDown(Keys.D) || Keyboardinput.IsKeyDown(Keys.Right))
                            {
                                MainCubeRectangle.X += 2;
                            }
                        }
                        SpeedBoostList.Remove(SpeedBoostRectanglePlacement);
                        UpdatesUntilNextBoost = UpdatesBetweenNewBoost;
                    }
                }
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