using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class CardImporter : EditorWindow
{
    [MenuItem("Narzêdzia/Importuj Karty Z CSV")]
    public static void ImportCards()
    {
        string path = EditorUtility.OpenFilePanel("Wybierz plik CSV z kartami", "Assets/Resources/CardData", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Nazwa")) continue;

            string[] cols = line.Split(";");
            if (cols.Length < 7)
            {
                Debug.LogWarning($"Pominiêto liniê (za ma³o kolumn): {linie}");
                continue;
            }

            CardData card = ScriptableObject.CreateInstance<CardData>();
            card.cardName = cols[0];
            card.faction = Enum.TryParse<Faction>(cols[1], out var f) ? f : Faction.Neutralne;
            card.power = int.TryParse(cols[2], out var p) ? p : 0;
            card.range = Enum.TryParse<RangeType>(cols[3], out var r) ? r : RangeType.Dowolny;
            card.effectDescription = cols[4];
            
            string effectName = cols[5];
            string effectParams = cols[6];

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

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Zaimportowano wszystkie karty");
    }
}
