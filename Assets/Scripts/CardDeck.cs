using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A class that manages the territory cards and their discard
public class CardDeck : MonoBehaviour
{
    private Stack<TerritoryCard> _deck;
    private Stack<TerritoryCard> _discardDeck;
    public void SetupDeck()
    {
        int infantryRem = 14, cavalryRem = 14, artilleryRem = 14;
        GameObject[] territoryObjects = GameObject.FindGameObjectsWithTag("Territory");
        _deck = new Stack<TerritoryCard>(territoryObjects.Length + 2);
        _discardDeck = new Stack<TerritoryCard>(territoryObjects.Length + 2);
        for(int i = 0; i < territoryObjects.Length; i++)
        {
            Territory t = territoryObjects[i].GetComponent<Territory>();
            int rand = Random.Range(1, infantryRem + cavalryRem + artilleryRem + 1);
            TroopType type;
            if(rand - artilleryRem <= 0)
            {
                type = TroopType.Artillery;
                artilleryRem--;
            }
            else if(rand - artilleryRem - cavalryRem < 0)
            {
                type = TroopType.Cavalry;
                cavalryRem--;
            }
            else
            {
                type = TroopType.Infantry;
                infantryRem--;
            }
            _deck.Push(new TerritoryCard(type, t));
        }
        _deck.Push(new TerritoryCard(TroopType.WildCard, null));
        _deck.Push(new TerritoryCard(TroopType.WildCard, null));
        Shuffle();
    }
    // Helper method that shuffles the deck
    public void Shuffle()
    {
        List<TerritoryCard> list = new List<TerritoryCard>(_deck);
        for ( int i = 0; i < list.Count; i++ )
        {
            int num = Random.Range(0, list.Count);
            TerritoryCard temp = list[i];
            list[i] = list[num];
            list[num] = temp;
        }
        _deck = new Stack<TerritoryCard>(list);
    }
    public TerritoryCard DrawCard()
    {
        if(_deck.Count == 0)
        {
            return null;
        }
        return _deck.Pop();
    }
    public void DiscardCard(TerritoryCard card)
    {
        _discardDeck.Push(card);
    }
}
