using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class CardImporter : EditorWindow
{
    [MenuItem("Narzêdzia/Importuj Karty Z CSV")]
    public static void ImportCards()
    {
        string path = EditorUtility.OpenFilePanel("Wybierz plik CSV z kartami", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Nazwa")) continue;

            string[] cols = line.Split(";");
            if (cols.Length < 5) continue;

            CardData card = ScriptableObject.CreateInstance<CardData>();
            card.cardName = cols[0];
            card.faction = Enum.TryParse<Faction>(cols[1], out var f) ? f : Faction.Neutralne;
            card.power = int.TryParse(cols[2], out var p) ? p : 0;
            card.range = Enum.TryParse<RangeType>(cols[3], out var r) ? r : RangeType.Dowolny;
            card.effect = EffectFactory.CreateEffectFromName(cols[5]);
            card.effectDescription = cols[4];

            string spritePath = $"Assets/Art/Cards/{card.cardName}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
            {
                card.artwork = sprite;
            }
            else
            {
                Debug.Log($"Brak grafiki dla karty: {card.cardName}");
            }

            string assetPath = $"Assets/Resources/CardData/{card.cardName}.asset";
            AssetDatabase.CreateAsset(card, assetPath);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Zaimportowano wszystkie karty");
    }
}
