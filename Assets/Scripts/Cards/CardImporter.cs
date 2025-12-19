using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class CardImporter : EditorWindow
{
    [MenuItem("Narzędzia/Importuj Karty Z CSV")]
    public static void ImportCards()
    {
        string path = EditorUtility.OpenFilePanel("Wybierz plik CSV z kartami", "Assets/Resources/CardData", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        int lineNumber = 0;

        foreach (string line in lines)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Nazwa")) continue;

            try
            {
                string[] cols = line.Split(";");
                if (cols.Length < 7)
                {
                    Debug.LogWarning($"Pominięto linię (za mało kolumn): {line}");
                    continue;
                }

                CardData card = ScriptableObject.CreateInstance<CardData>();
                card.cardName = cols[0];
                card.faction = Enum.TryParse<Faction>(cols[1], out var f) ? f : Faction.Neutralne;
                
                string powerString = cols[2].Trim();
                card.power = int.TryParse(powerString, out var p) ? p : 0;

                if (powerString == "-")
                {
                    card.powerDisplayOverride = "-";
                }
                else if (powerString == "∞")
                {
                    card.powerDisplayOverride = "∞";
                }
                else
                {
                    card.powerDisplayOverride = "";
                }

                card.range = Enum.TryParse<RangeType>(cols[3].Trim(), out var r) ? r : RangeType.Dowolny;
                card.effectDescription = cols[4].Replace("\"", "");
                string effectName = cols[5].Trim();
                string effectParams = cols[6].Trim();

                if (cols.Length > 7 && !string.IsNullOrEmpty(cols[7]))
                {
                    card.maxCopies = int.TryParse(cols[7].Trim(), out var c) ? c : 1;
                }
                else
                {
                    card.maxCopies = 1;
                }

                string assetPath = $"Assets/Resources/CardData/{card.cardName}.asset";
                AssetDatabase.CreateAsset(card, assetPath);

                CardEffect newEffect = EffectFactory.CreateEffectFromName(effectName, effectParams);

                if (newEffect != null)
                {
                    newEffect.effectName = effectName;
                    newEffect.effectDescription = card.effectDescription;

                    AssetDatabase.AddObjectToAsset(newEffect, card);

                    card.effect = newEffect;
                }

                string spritePath = $"Assets/Art/Cards/{card.cardName}.png";
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite != null)
                {
                    card.artwork = sprite;
                }
                else
                {
                    Debug.LogWarning($"Brak grafiki dla karty: {card.cardName}. Szukano w: {spritePath}.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Błąd w linii {lineNumber} CSV. Treść linii: {line}\nSzczegóły błędu: {ex.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Zaimportowano wszystkie karty");
    }
}
