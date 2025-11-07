using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Player
{
    public string playerName;
    public Faction faction;
    public List<CardInstance> cardsOnBoard = new List<CardInstance>();
    public int totalPoints;

    public bool lostLastRound = false;

    public Player(string name, Faction chosenFaction)
    {
        playerName = name;
        faction = chosenFaction;
    }

    public void AddCardToBoard(CardInstance card)
    {
        cardsOnBoard.Add(card);
        RecalculatePoints();
    }

    public void RecalculatePoints()
    {
        totalPoints = 0;
        foreach (var c in cardsOnBoard)
        {
            totalPoints += c.currentPower;
        }
    }

    public void ClearBoard()
    {
        cardsOnBoard.Clear();
        totalPoints = 0;
    }
}
