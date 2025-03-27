using UnityEngine;
using System.Linq;

public class LetterRandomizer : MonoBehaviour
{
    private void Start()
    {
        RandomizeAllLetters();
    }

    public void RandomizeAllLetters()
    {
        var placeholders = FindObjectsOfType<LetterPlaceholder>()
            .OrderBy(x => Random.value)
            .ToArray();

        var letters = FindObjectsOfType<DraggableLetter>()
            .Where(l => !l.isCorrectlyPlaced)
            .ToArray();

        // First, remove all letters from placeholders
        foreach (var placeholder in FindObjectsOfType<LetterPlaceholder>())
        {
            if (placeholder.transform.childCount > 0)
            {
                var letter = placeholder.transform.GetChild(0).GetComponent<DraggableLetter>();
                if (letter != null)
                {
                    letter.transform.SetParent(null);
                }
            }
        }

        // Then assign letters to random placeholders
        for (int i = 0; i < Mathf.Min(letters.Length, placeholders.Length); i++)
        {
            letters[i].transform.SetParent(placeholders[i].transform);
            letters[i].rectTransform.anchoredPosition = Vector2.zero;
            letters[i].startParent = placeholders[i].transform;
            letters[i].startPosition = Vector2.zero;
        }
    }
}