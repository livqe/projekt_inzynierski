using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SavedDeck
{
    public string deckName;
    public Faction faction;
    public List<string> cardIds;

    public SavedDeck(string name, Faction factionType)
    {
        deckName = name;
        faction = factionType;
        cardIds = new List<string>();
    }
}

[System.Serializable]
public class DeckListWrapper
{
    public List<SavedDeck> decks = new List<SavedDeck>();
}

public static class DeckStorage
{
    private static string savePath => Application.persistentDataPath + "/decks.json";

    public static void SaveDecks(List<SavedDeck> decks)
    {
        DeckListWrapper wrapper = new DeckListWrapper {  decks = decks };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Zapisano talie w {savePath}.");
    }

    public static List<SavedDeck> LoadDecks()
    {
        if (!File.Exists(savePath)) return new List<SavedDeck>();

        string json = File.ReadAllText(savePath);
        DeckListWrapper wrapper = JsonUtility.FromJson<DeckListWrapper>(json);
        return wrapper != null ? wrapper.decks : new List<SavedDeck>();
    }
}