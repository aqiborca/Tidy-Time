using UnityEngine;

public class FishManager : MonoBehaviour
{
    [Header("Fish References")]
    public GameObject fishSmall;
    public GameObject fishBig;

    [Header("Fish Food")]
    public GameObject fishFood;

    [Header("Letters")]
    public GameObject[] letters;

    [Header("Burp Settings")]
    public int clicksToBurp = 4;
    private int clickCount = 0;

    private bool isFed = false;
    private bool hasBurped = false;

    void Start()
    {
        // Only show fish food if closet chore is done
        if (ChoreManager.Instance != null && ChoreManager.Instance.IsChoreCompleted("organizecloset"))
        {
            fishFood.SetActive(true);
        }
        else
        {
            fishFood.SetActive(false);
        }

        // Set initial states
        fishSmall.SetActive(true);
        fishBig.SetActive(false);
        foreach (GameObject letter in letters)
            letter.SetActive(false);
    }

    public void FeedFish()
    {
        isFed = true;
        fishFood.SetActive(false);
        fishSmall.SetActive(false);
        fishBig.SetActive(true);
    }

    public void OnBigFishClicked()
    {
        if (!isFed || hasBurped) return;

        clickCount++;

        if (clickCount < clicksToBurp)
        {
            // Grow fish slightly (optional visual)
            fishBig.transform.localScale *= 1.1f;
        }
        else
        {
            BurpFish();
        }
    }

    private void BurpFish()
    {
        hasBurped = true;

        // Spawn letters
        foreach (GameObject letter in letters)
            letter.SetActive(true);

        // Reset fish
        fishBig.SetActive(false);
        fishSmall.SetActive(true);
        fishBig.transform.localScale = Vector3.one; // Reset size
    }

    public void CollectLetter(GameObject letter)
    {
        letter.SetActive(false);
    }
}
