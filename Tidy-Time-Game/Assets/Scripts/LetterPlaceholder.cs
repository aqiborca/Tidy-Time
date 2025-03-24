using UnityEngine;
using UnityEngine.EventSystems;

public class LetterPlaceholder : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedLetter = eventData.pointerDrag;

        if (droppedLetter != null)
        {
            DraggableLetter draggableLetter = droppedLetter.GetComponent<DraggableLetter>();

            if (draggableLetter != null)
            {
                // Set the letter's position to the placeholder's position
                droppedLetter.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                // Check if the letter is in the correct place
                if (IsCorrectPlaceholder(droppedLetter))
                {
                    draggableLetter.isCorrectlyPlaced = true;
                    draggableLetter.OnCorrectPlacement();
                }
                else
                {
                    draggableLetter.isCorrectlyPlaced = false;
                    draggableLetter.OnIncorrectPlacement();
                }
            }
        }
    }

    // Placeholder logic to check if this placeholder is the correct one for the letter
    private bool IsCorrectPlaceholder(GameObject droppedLetter)
    {
        // Implement the logic to check if the letter matches the correct placeholder
        // For example, you can check the name or tag of the letter and compare it with the placeholder
        return droppedLetter.name == gameObject.name;  // Example check
    }
}
