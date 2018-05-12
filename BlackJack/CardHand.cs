using System;
using System.Collections.Generic;

namespace BlackJack
{
    /// <summary>
    /// A hand of cards
    /// </summary>
    ///
    public class CardHand
    {
        //public static int CARD_MAX_NUM = 26;
        protected List<Card> m_hand;

        public CardHand()
        {
            m_hand = new List<Card>();
        }

        public void AddCard(Card card)
        {
            m_hand.Add(card);
        }

        public Card GetCard(int index)
        {
            if (index < 0 || index > m_hand.Count - 1)
                return null;
            return m_hand[index];
        }

        public int GetNumCardsInHand()
        {
            return m_hand.Count;
        }
    }
}
