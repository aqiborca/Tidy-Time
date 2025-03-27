using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LetterRandomizer : MonoBehaviour
{
    public void RandomizeLetters(bool allowCorrectPlacements)
    {
        var placeholders = FindObjectsOfType<LetterPlaceholder>().ToList();
        var letters = FindObjectsOfType<DraggableLetter>().ToList();

        // Clear all placements
        foreach (var placeholder in placeholders)
        {
            if (placeholder.transform.childCount > 0)
            {
                var child = placeholder.transform.GetChild(0);
                child.SetParent(null);
            }
        }

        // Create list of allowed placements
        List<Transform> possiblePlacements = new List<Transform>();
        foreach (var placeholder in placeholders)
        {
            bool isCorrectForAnyLetter = letters.Any(l => 
                l.name.Replace("Letter ", "") == placeholder.name.Replace("Placeholder ", ""));
            
            if (allowCorrectPlacements || !isCorrectForAnyLetter)
            {
                possiblePlacements.Add(placeholder.transform);
            }
        }

        // Assign letters randomly
        letters = letters.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < letters.Count; i++)
        {
            Transform targetParent = i < possiblePlacements.Count ? 
                possiblePlacements[i] : 
                placeholders[i % placeholders.Count].transform;

            letters[i].transform.SetParent(targetParent);
            letters[i].rectTransform.anchoredPosition = Vector2.zero;
            letters[i].startParent = targetParent;
            letters[i].startPosition = Vector2.zero;
            letters[i].isCorrectlyPlaced = false;
        }
    }
}