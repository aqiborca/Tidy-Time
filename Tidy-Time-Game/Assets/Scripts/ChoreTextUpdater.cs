using UnityEngine;
using TMPro;

public class ChoreTextUpdater : MonoBehaviour
{
    public string choreName; // Set this in the Inspector to match the chore's key in ChoreManager
    private TextMeshProUGUI textComponent;
    private string originalText;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            originalText = textComponent.text; // Store the original text
        }
        UpdateTextStyle();
    }

    private void Update()
    {
        UpdateTextStyle();
    }

    private void UpdateTextStyle()
    {
        if (ChoreManager.Instance != null && textComponent != null)
        {
            if (ChoreManager.Instance.IsChoreCompleted(choreName))
            {
                textComponent.text = "<s>" + originalText + "</s>"; // Apply strikethrough
            }
            else
            {
                textComponent.text = originalText; // Reset to original text if chore is not complete
            }
        }
    }
}
