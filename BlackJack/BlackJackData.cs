using System;
using System.Collections.Generic;

namespace BlackJack
{
    /// <summary>
    /// List of BlackJackData classes for tombstoning
    /// </summary>
    public class BlackJackDataList
    {
        public List<BlackJackData> m_list = new List<BlackJackData>();

        public BlackJackDataList()
        {
        }

        /// <summary>
        /// add Deck into BlackJackDataList
        /// </summary>
        /// <param name="deck"></param>
        public void AddDeckData(Deck deck)
        {
            for (int j = 0; j < deck.CardCount(); j++)
            {
                Card card = deck.GetCard(j);
                BlackJackData data = new BlackJackData();
                data.AddCardData(card, deck.InternalDeckId());
                m_list.Add(data);
            }
        }
    }

    /// <summary>
    /// Black Jack data class for card data
    /// </summary>
    public class BlackJackData
    {
        public int m_suit;
        public int m_id;
        public int m_value = 0;  // the value: ace=11 or 1; 2=2,...;10,11,12,13=10
        public bool m_isTurned = false;
        public int m_internalDeckId;
        public int m_z;

        public BlackJackData()
        {
            // Serialized class constructor cannot have parameters
        }

        public BlackJackData(Card c)
        {
            if (c != null)
            {
                m_suit = (int)c.Suit();
                m_id = c.CardId();
                m_internalDeckId = 10;
                m_isTurned = false;
                m_z = c.m_z;
                m_value = 0;
                if (c.CardId() > 1 && c.CardId() < 11)
                    m_value = c.CardId();
                else
                {
                    if (c.CardId() > 10)
                        m_value = 10;
                    if (c.CardId() == 1)
                        m_value = 11;
                }
            }
        }

        /// <summary>
        /// create Black Jack data from Card
        /// </summary>
        /// <param name="card"></param>
        /// <param name="internalDeckId"></param>
        public void AddCardData(Card card, int internalDeckId)
        {
            m_suit = (int)card.Suit();
            m_id = card.CardId();
            m_isTurned = card.IsTurned();
            m_internalDeckId = internalDeckId;
            m_z = card.m_z;
            m_value = ComputeCardValue(card);
        }
        
        /// <summary>
        /// determines the value of given card for the game 
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private int ComputeCardValue(Card card)
        {
            switch (card.CardId())
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return card.CardId();
                case 11:
                case 12:
                case 13:
                    return 10;                
            }
            return 0;
        }
    }
}
