using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const String GAME_TITLE = "BLACK JACK";

        GraphicsDeviceManager graphics;
        internal SpriteBatch spriteBatch;

        float screenWidth;
        float screenHeight;
        //
        // Cap between cards on top of each other
        public static int CARD_CAP = 30;
        // trigger the action after a delay (in seconds)
        const float _delay = 1;
        //
        KeyboardState previousState;
        Texture2D startGameSplash;
        SpriteFont stateFont;
        SpriteFont fontScoreP1, fontScoreP2;
        Song music;

        int scoreP1, scoreP2;
        private bool gameStarted;
        private bool gameOver;
        private bool spaceDown;
        private bool isAskingToDraw;
        private bool p1HasDrawn;
        private bool firstHand;
        private bool showP2Score;

        String winner = "";
        const String P1_WINS_MSG = "Giocatore 1 VINCE.";
        const String P2_WINS_MSG = "Avversario VINCE.";
        const String P1_PNTS_MSG = "Punti Giocatore 1: ";
        const String P2_PNTS_MSG = "Punti Avversario: ";
        const String DRAWGAME_MSG = "E' UN PAREGGIO.";
        const String DRAWCARD_MSG = "Carta (S/N)?";
        const String NEWGAME_KEY_MSG = "Premi SPAZIO per cominciare una partita";
        const String MUTE_KEY_MSG = "M - Musica on/off";

        // List of cards
        List<Card> m_allCards = new List<Card>();
        List<Card> m_allCards_cpy = new List<Card>();

        // The deck
        Deck m_deck = new Deck(10);

        // List of all decks
        List<Deck> m_deckList = new List<Deck>();

        // For deserialize (read) deck data to
        BlackJackDataList m_dataList = null;

        BlackJackData bjData = null;

        Random m_random;

        DelayedAction cpuHandAction;

        // Active card that is under user moving
        //Card p_activeCard;

        // Players hands
        CardHand p1Hand, p2Hand;

        // Z order counter
        static int m_rootZ = 0;

        // Indicates if cpu has stopped drawing cards
        private bool cpuHasStopped;        

        // this view Rectangle
        //Rectangle m_rect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            screenHeight = (float)480;
            screenWidth = (float)800;

            previousState = Keyboard.GetState();

            IsMouseVisible = true;
            gameStarted = spaceDown = false;
            gameOver = true;
            isAskingToDraw = false;

            //p_activeCard = null;

            // Create the deck
            int cap = CARD_CAP;
            m_deck.Position = new Point(cap, 180);
            m_deckList.Add(m_deck);

            // the delayed action
            cpuHandAction =
                    new DelayedAction(TurnOpponentCards, _delay);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            startGameSplash = Content.Load<Texture2D>("start-splash");
            stateFont = Content.Load<SpriteFont>("GameState");
            fontScoreP1 = Content.Load<SpriteFont>("ScoreP1");
            fontScoreP2 = Content.Load<SpriteFont>("ScoreP2");
            music = Content.Load<Song>("entrtanr");
            
            //  Uncomment the following line will also loop the song
            //  MediaPlayer.IsRepeating = true;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.Play(music);

            //// Create all cards
            for (int i = 1; i < 14; i++)
                m_allCards.Add(new Card(Card.SuitEnum.EClubs, i, NextZ(), this.Content));

            for (int i = 1; i < 14; i++)
                m_allCards.Add(new Card(Card.SuitEnum.EDiamond, i, NextZ(), this.Content));

            for (int i = 1; i < 14; i++)
                m_allCards.Add(new Card(Card.SuitEnum.ESpade, i, NextZ(), this.Content));

            for (int i = 1; i < 14; i++)
                m_allCards.Add(new Card(Card.SuitEnum.EHeart, i, NextZ(), this.Content));

            m_deck.LoadBackground(this.Content);
            m_deck.Position = new Point((int)(4 * screenWidth / 5), (int)screenHeight / 2 - Card.CARD_HEIGHT / 2);

            m_allCards_cpy = m_allCards.ToList();        
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            MediaPlayer.MediaStateChanged -= MediaPlayer_MediaStateChanged;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (state.IsKeyDown(Keys.M)
                 & !previousState.IsKeyDown(Keys.M))
            {                
                MuteUnmuteSound();
                previousState = state;
                return;
            }

            // Start the game if Space is pressed.
            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space)
                    & !previousState.IsKeyDown(Keys.Space))
                {
                    PrepareNewGame();                    
                }
                previousState = state;
                return;
            }

            if (firstHand && (p1Hand.GetNumCardsInHand() == p2Hand.GetNumCardsInHand())
                && p1Hand.GetNumCardsInHand() == 2)
            {
                UpdateScore(p1Hand, "p1");
                UpdateScore(p2Hand, "p2");
                CheckForWinner();
                firstHand = false;
            }
            if (winner != "")
            {
                if (state.IsKeyDown(Keys.Space)
                    & !previousState.IsKeyDown(Keys.Space))
                {
                    int i;
                    // explicitly turn the cards to back face
                    for (i = 0; i < p1Hand.GetNumCardsInHand(); i++)
                        p1Hand.GetCard(i).setTurned(false);
                    for (i = 0; i < p2Hand.GetNumCardsInHand(); i++)
                        p2Hand.GetCard(i).setTurned(false);
                    
                    gameOver = true;
                    spaceDown = false;

                    PrepareNewGame();                    
                }
                else
                {
                    var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    cpuHandAction.Update(timer);
                }
                
                previousState = state;
                return;
            }
            if (isAskingToDraw)
            {
                if (state.IsKeyDown(Keys.S)
                    & !previousState.IsKeyDown(Keys.S))
                {
                    // P1: draw a new card from the deck
                    Card drawn = m_deck.RemoveLast();
                    drawn.setTurned(true);
                    p1Hand.AddCard(drawn);
                    p1HasDrawn = true;
                }
                if (state.IsKeyDown(Keys.N)
                    & !previousState.IsKeyDown(Keys.N))
                {
                    p1HasDrawn = false;
                    isAskingToDraw = false;
                }
            }

            // update the score
            UpdateScore(p1Hand, "p1");

            // the cpu could draw too
            if (isAskingToDraw && p1HasDrawn)
            {
                if (CpuHasToDraw())
                {
                    // CPU: draw a new card from the deck
                    Card drawn = m_deck.RemoveLast();                    
                    p2Hand.AddCard(drawn);
                }
                p1HasDrawn = false;
            }
            else
            {
                if (!isAskingToDraw && !cpuHasStopped)
                    if (CpuHasToDraw())
                    {
                        // CPU: draw a new card from the deck
                        Card drawn = m_deck.RemoveLast();
                        drawn.setTurned(false);
                        p2Hand.AddCard(drawn);
                    }
            }
            UpdateScore(p2Hand, "p2");
            if (!isAskingToDraw && ((scoreP2 >= scoreP1) || (scoreP1 >=20)))
                cpuHasStopped = true;

            CheckForWinner();
                        
            previousState = state;

            base.Update(gameTime);
        }
        
        private void StartGame()
        {
            scoreP1 = scoreP2 = 0;
            m_rootZ = 0;

            firstHand = true;
            showP2Score = false;
            isAskingToDraw = true;
            p1HasDrawn = false;
            cpuHasStopped = false;
            winner = "";

            p1Hand = new CardHand();
            p2Hand = new CardHand();

            m_dataList = new BlackJackDataList();            
            bjData = new BlackJackData();

            // prepare the deck
            m_deck.RemoveAllCards();
            
            m_allCards = m_allCards_cpy.ToList();
            m_random = new Random();

            // Source deck
            for (int i = 0; i < m_allCards.Count; i++)
            {
                m_deck.AddCard(GetRandomCard());
                i--;
            }

            // the card under the pile
            m_deck.GetCard(0).setTurned(false);

            m_dataList.AddDeckData(m_deck);
            
            Card drawn;
            // draw the first 2 cards
            for (int i = 0; i < 2; i++)
            {                
                drawn = m_deck.RemoveLast();
                drawn.setTurned(true);
                p1Hand.AddCard(drawn);
                //System.Diagnostics.Debug.WriteLine("Giocatore 1 ha pescato : " + drawn.CardId() + " " + drawn.Suit());
                
                drawn = m_deck.RemoveLast();
                drawn.setTurned((i % 2 == 0) ? true : false);                
                p2Hand.AddCard(drawn);
                //System.Diagnostics.Debug.WriteLine("Avversario ha pescato : " + drawn.CardId() + " " + drawn.Suit());
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (!gameStarted)
            {
                ShowSplashScreen();
            }
            else
            {
                // players points
                if (showP2Score)
                    spriteBatch.DrawString(fontScoreP2, P2_PNTS_MSG + scoreP2,
                        new Vector2(50, 20), Color.Red);
                spriteBatch.DrawString(fontScoreP1, P1_PNTS_MSG + scoreP1,
                    new Vector2(50, 430), Color.Blue);

                for (int i = 0; i < p1Hand.GetNumCardsInHand(); i++)
                {
                    // display player 1 hand
                    Card c = p1Hand.GetCard(i);
                    //c.setTurned(true);
                    c.CardRectangle = new Rectangle(50 + CARD_CAP * i, 310, Card.CARD_WIDTH, Card.CARD_HEIGHT);
                    c.Draw(spriteBatch);
                }
                for (int j = 0; j < p2Hand.GetNumCardsInHand(); j++)
                {
                    // display opponent hand
                    Card co = p2Hand.GetCard(j);
                    //if (j == 0)
                    //    co.setTurned(true);
                    co.CardRectangle = new Rectangle(50 + CARD_CAP * j, 60, Card.CARD_WIDTH, Card.CARD_HEIGHT);
                    co.Draw(spriteBatch);
                }

                if (winner != "")
                {
                    // we have a winner
                    spriteBatch.DrawString(fontScoreP1, winner,
                    new Vector2(screenWidth / 2 - fontScoreP1.Texture.Width / 2, screenHeight / 2), Color.Purple);
                }
                else
                {
                    // ask the player 1 to draw a card
                    spriteBatch.DrawString(fontScoreP1, DRAWCARD_MSG,
                    new Vector2(screenWidth / 2 - fontScoreP1.Texture.Width / 2, screenHeight / 2), Color.Purple);
                }

                // display the card under the pile                
                m_deck.GetCard(0).Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        private void ShowSplashScreen()
        {
            // Fill the screen with black before the game starts
            spriteBatch.Draw(startGameSplash, new Rectangle(0, 0,
                (int)screenWidth, (int)screenHeight), Color.White);

            String title = GAME_TITLE;
            String pressSpace = NEWGAME_KEY_MSG;
            String pressMute = MUTE_KEY_MSG;

            // Measure the size of text in the given font
            Vector2 titleSize = stateFont.MeasureString(title);
            Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);
            Vector2 pressMuteSize = fontScoreP1.MeasureString(pressMute);

            // Draw the text horizontally centered
            spriteBatch.DrawString(stateFont, title,
                new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3),
                Color.Purple);
            spriteBatch.DrawString(stateFont, pressSpace,
                new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2),
                Color.White);
            spriteBatch.DrawString(fontScoreP1, pressMute,
                new Vector2(screenWidth / 2 - pressMuteSize.X / 2, 2*screenHeight / 3),
                Color.Orange);
        }

        private void CheckForWinner()
        {
            if (firstHand)
            {
                // check if someone has already made 21
                if (scoreP1 == 21 || scoreP2 == 21)
                {
                    if (scoreP1 == scoreP2)
                    {
                        winner += DRAWGAME_MSG;
                        isAskingToDraw = false;
                    }
                    //else if (scoreP1 < scoreP2)
                    //{
                    //    winner += P2_WINS_MSG;
                    //    isAskingToDraw = false;
                    //}
                    //else
                    //{
                    //    winner += P1_WINS_MSG;
                    //    isAskingToDraw = false;
                    //}
                }
            }
            else
            {
                if (isAskingToDraw)
                {
                    if (scoreP1 > 21)
                    {
                        winner += P2_WINS_MSG;
                    }
                    if (scoreP2 > 21)
                    {
                        winner += P1_WINS_MSG;
                    }
                }
                else
                {
                    if (scoreP1 > 21)
                    {
                        winner += P2_WINS_MSG;
                    }
                    else if (scoreP2 > 21)
                    {
                        winner += P1_WINS_MSG;
                    }
                    else
                    {
                        if (scoreP1 == scoreP2)
                            winner += DRAWGAME_MSG;
                        else if (scoreP1 < scoreP2)
                        {
                            winner += P2_WINS_MSG;
                        }
                        else
                        {
                            winner += P1_WINS_MSG;
                        }
                    }
                }
            }
        }

        private bool CpuHasToDraw()
        {
            bool hasToDraw = false;

            if (scoreP2 <= scoreP1 && scoreP1 <= 21)
            {
                Random rd = new Random();
                //float genNum = scoreP1 - scoreP2;

                float randFact = (float)(1.0f / (float)scoreP2) * 2 * (float)(scoreP1 - scoreP2);

                if (scoreP2 <= 10)
                    hasToDraw = true;

                else if (scoreP2 > 10 && scoreP2 < 13)
                {
                    hasToDraw = rd.NextDouble() <= randFact;
                }
                else if (scoreP2 >= 13 && scoreP2 < 16)
                {
                    hasToDraw = rd.NextDouble() <= randFact;
                }
                else if (scoreP2 >= 16 && scoreP2 < 20)
                {
                    hasToDraw = rd.NextDouble() <= randFact;
                }
                else
                    hasToDraw = false;
            }

            return hasToDraw;
        }

        private void UpdateScore(CardHand hand, string playerId)
        {
            int sub_points = 0;
            int ace_count = 0;

            Card[] theCards = new Card[hand.GetNumCardsInHand()];
            BlackJackData[] theAces = new BlackJackData[4];
            BlackJackData[] theHand = new BlackJackData[hand.GetNumCardsInHand()];
            for (int i = 0; i < hand.GetNumCardsInHand(); i++)
            {
                theCards[i] = hand.GetCard(i);
                bjData = new BlackJackData();
                bjData.AddCardData(theCards[i], 10);
                theHand[i] = bjData;
            }
            var sorted = theHand.OrderBy(i => i.m_value).ToArray<BlackJackData>();

            if (theCards.Length >= 2)
            {
                foreach (BlackJackData c in sorted)
                {
                    if (c.m_id != 1)
                        sub_points += c.m_value;
                    else
                    {
                        theAces[ace_count] = c;
                        ++ace_count;
                    }
                }
                if (ace_count > 0)
                {
                    while (ace_count > 0)
                    {
                        if (sub_points + 11 > 21)
                        {
                            if (theAces[ace_count - 1].m_id == 1)
                                sub_points += 1;
                        }
                        else
                        {
                            sub_points += 11;
                        }
                        --ace_count;
                    }
                }
            }
            if (playerId == "p1")
            {
                scoreP1 = sub_points;
            }
            else    //"p2"
            {
                scoreP2 = sub_points;
            }
        }

        private void TurnOpponentCards()
        {
            for (int i = 0; i < p2Hand.GetNumCardsInHand(); i++)
            {
                p2Hand.GetCard(i).setTurned(true);
            }
            // show opponent's score
            showP2Score = true;
        }

        /// <summary>
        /// starts a new game
        /// </summary>
        private void PrepareNewGame()
        {
            StartGame();
            gameStarted = true;
            spaceDown = true;
            gameOver = false;
        }

        public static int NextZ()
        {
            return m_rootZ++;
        }

        /// <summary>
        /// Gets random card from the card list. Removes card from the list
        /// </summary>
        /// <returns></returns>
        private Card GetRandomCard()
        {
            int max = m_allCards.Count;  // - 1;
            int random = RandomNumber(0, max);

            Card randCard = m_allCards[random];
            m_allCards.RemoveAt(random);
            return randCard;
        }

        private int RandomNumber(int min, int max)
        {
            return m_random.Next(min, max);
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(music);
        }

        private void MuteUnmuteSound()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
            else if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();         
        }
        
    }
}
