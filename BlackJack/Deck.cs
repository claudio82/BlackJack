using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace BlackJack
{
    /// <summary>
    /// Base deck class
    /// </summary>
    ///
    public class Deck
    {
        // Cards of this deck
        protected List<Card> m_cards = new List<Card>();

        // Position of this deck on the screen
        protected Point m_pos;

        // Deck bacground property
        Texture2D m_background;

        // Deck types
        public enum DeckType
        {
            EPot = 1
        };

        protected int m_internalDeckId;

        protected DeckType m_deckType;

        public Deck()
        {
            m_deckType = DeckType.EPot;
        }

        public Deck(int internalDeckId)
        {
            m_internalDeckId = internalDeckId;
            m_deckType = DeckType.EPot;
        }

        ~Deck()
        {
        }

        public int InternalDeckId()
        {
            return m_internalDeckId;
        }

        /// <summary>
        /// Deck type
        /// </summary>
        /// <returns></returns>
        public DeckType Type()
        {
            return m_deckType;
        }

        /// <summary>
        /// Deck poition
        /// </summary>
        public virtual Point Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }

        /// <summary>
        /// Count of cards
        /// </summary>
        /// <returns></returns>
        public virtual int CardCount()
        {
            return m_cards.Count;
        }

        /// <summary>
        /// Top most card of this deck
        /// </summary>
        /// <returns></returns>
        public virtual Card GetLast()
        {
            return m_cards[m_cards.Count - 1];
        }

        /// <summary>
        /// Removes last card / top most card from this deck
        /// </summary>
        /// <returns></returns>
        public virtual Card RemoveLast()
        {
            Card card = m_cards[m_cards.Count - 1];
            m_cards.Remove(card);
            return card;
        }

        /// <summary>
        /// Returns selected card
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual Card GetCard(int index)
        {
            return m_cards[index];
        }

        /// <summary>
        /// Adds new card to this deck
        /// </summary>
        /// <param name="newCard"></param>
        public virtual void AddCard(Card newCard)
        {
            // Cards are positioned with 20 pixels cap
            Rectangle r = newCard.CardRectangle;
            Point p = m_pos;
            p.Y = p.Y + (Card.CARD_CAP * m_cards.Count);
            r.Location = p;
            newCard.CardRectangle = r;

            newCard.m_z = Game1.NextZ();

            // Add card into this deck
            m_cards.Add(newCard);
        }

        /// <summary>
        /// Removes card from the deck
        /// </summary>
        /// <param name="c"></param>
        public virtual void RemoveCard(Card c)
        {
            // Remove card from the deck
            m_cards.Remove(c);            
        }

        public virtual void RemoveAllCards()
        {
            if (m_cards.Count > 0)
                m_cards.RemoveRange(0, m_cards.Count);
        }

        /// <summary>
        /// Draw deck background and all cards of this deck
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public virtual void Draw(SpriteBatch theSpriteBatch)
        {
            // Draw empty deck background
            Rectangle r = new Rectangle(m_pos.X, m_pos.Y, Card.CARD_WIDTH, Card.CARD_HEIGHT);
            if (m_background != null)
                theSpriteBatch.Draw(m_background, r, Color.White);

            // Draw cards
            for (int i = 0; i < m_cards.Count; i++)
            {
                Card c = m_cards[i];
                c.Draw(theSpriteBatch);
            }
        }

        public virtual void LoadBackground(ContentManager theContentManager)
        {
            m_background = theContentManager.Load<Texture2D>("deck_background");
        }

    }
}
