using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input.Touch;

namespace BlackJack
{
    /// <summary>
    /// Black Jack card that has value, land and texture
    /// </summary>
    ///
    public class Card
    {
        public enum SuitEnum
        {
            EClubs = 1,
            EDiamond = 2,
            EHeart = 3,
            ESpade = 4
        };
        SuitEnum m_suit;
        int m_id = 0; // 1=ace, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11=jack, 12=queen, 13=king        

        public static int CARD_WIDTH = 80;
        public static int CARD_HEIGHT = 110;
        public static int CARD_CAP = 25;

        Texture2D m_card = null; // Card picture
        Texture2D m_cardBack = null; // Card back picture
        Rectangle m_rect; // Card rectangle
        Rectangle m_originRect; // for cancel card move back to original position

        public int m_z = 0; // Card z-order

        bool m_isTurned = false; // Is card face or background visible

        String m_cardName; // for loading texture content

        public Card(int suit, int id, int z)
        {
            m_z = z;
        }

        public Card(SuitEnum suit, int id, int z, ContentManager theContentManager, bool isTurned = false)
        {
            m_z = z;

            this.LoadCard(suit, id, theContentManager, isTurned);
        }

        ~Card()
        {
        }

        /// <summary>
        /// Get card suit
        /// </summary>
        /// <returns></returns>
        public SuitEnum Suit()
        {
            return m_suit;
        }

        /// <summary>
        /// Get card value / id
        /// </summary>
        /// <returns></returns>
        public int CardId()
        {
            return m_id;
        }
        
        /// <summary>
        /// Is this turned card?
        /// </summary>
        /// <returns></returns>
        public bool IsTurned()
        {
            return m_isTurned;
        }

        /// <summary>
        /// Returns owner deck of this
        /// </summary>
        //public Deck OwnerDeck
        //{
        //    get
        //    {
        //        return p_ownerDeck;
        //    }
        //    set
        //    {
        //        p_ownerDeck = value;
        //    }
        //}

        /// <summary>
        /// Returns card rectangle
        /// </summary>
        public Rectangle CardRectangle
        {
            get
            {
                return m_rect;
            }
            set
            {
                m_rect = value;
            }
        }

        /// <summary>
        /// Sets the card either turned or not.
        /// </summary>
        /// <param name="turned">If true, the value of the card will be shown.</param>
        public void setTurned(bool turned)
        {
            m_isTurned = turned;
        }

        /// <summary>
        /// load the texture for the requested card
        /// </summary>
        /// <param name="suit"></param>
        /// <param name="id"></param>
        /// <param name="theContentManager"></param>
        /// <param name="isTurned"></param>
        private void LoadCard(SuitEnum suit, int id, ContentManager theContentManager, bool isTurned)
        {
            m_id = id;
            m_suit = suit;
            m_isTurned = isTurned;

            // Set default pos(0) and size
            m_rect = new Rectangle(0, 0, CARD_WIDTH, CARD_HEIGHT);
            m_originRect = m_rect;

            // Card background
            m_cardBack = theContentManager.Load<Texture2D>("card_background");

            // Card foreground
            switch (m_suit)
            {
                case SuitEnum.EClubs:
                    {
                        m_cardName = "Club";
                        break;
                    }
                case SuitEnum.EDiamond:
                    {
                        m_cardName = "Diamond";
                        break;
                    }
                case SuitEnum.EHeart:
                    {
                        m_cardName = "Heart";
                        break;
                    }
                case SuitEnum.ESpade:
                    {
                        m_cardName = "Spade";
                        break;
                    }
            }

            switch (m_id)
            {
                case 1:
                    {
                        m_cardName = String.Concat(m_cardName, "_ace");
                        break;
                    }
                case 2:
                    {
                        m_cardName = String.Concat(m_cardName, "_2");
                        break;
                    }
                case 3:
                    {
                        m_cardName = String.Concat(m_cardName, "_3");
                        break;
                    }
                case 4:
                    {
                        m_cardName = String.Concat(m_cardName, "_4");
                        break;
                    }
                case 5:
                    {
                        m_cardName = String.Concat(m_cardName, "_5");
                        break;
                    }
                case 6:
                    {
                        m_cardName = String.Concat(m_cardName, "_6");
                        break;
                    }
                case 7:
                    {
                        m_cardName = String.Concat(m_cardName, "_7");
                        break;
                    }
                case 8:
                    {
                        m_cardName = String.Concat(m_cardName, "_8");
                        break;
                    }
                case 9:
                    {
                        m_cardName = String.Concat(m_cardName, "_9");
                        break;
                    }
                case 10:
                    {
                        m_cardName = String.Concat(m_cardName, "_10");
                        break;
                    }
                case 11:
                    {
                        m_cardName = String.Concat(m_cardName, "_jack");
                        break;
                    }
                case 12:
                    {
                        m_cardName = String.Concat(m_cardName, "_queen");
                        break;
                    }
                case 13:
                    {
                        m_cardName = String.Concat(m_cardName, "_king");
                        break;
                    }
            }

            // Load corresponding card
            m_card = theContentManager.Load<Texture2D>(m_cardName);
        }

        /// <summary>
        /// Draw this card
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (m_isTurned)
            {
                if (m_card != null)
                    theSpriteBatch.Draw(m_card, m_rect, Color.White);
            }
            else
            {
                if (m_cardBack != null)
                    theSpriteBatch.Draw(m_cardBack, m_rect, Color.White);
            }
        }
    }
}
