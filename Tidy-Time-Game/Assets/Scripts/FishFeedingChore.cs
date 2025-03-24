using UnityEngine;

//manages fish feeding mechanics and letter collection
public class FishFeedingChore : MonoBehaviour
{
    public enum ObjectType { Fish, Letter }

    [Tooltip("Specify the type of this object")]
    [SerializeField] private ObjectType objectType = ObjectType.Fish;

    [Header("Fish Settings")]
    [Tooltip("Next fish stage game object")]
    [SerializeField] private GameObject nextFishStage;

    [Tooltip("Initial fish to reactivate when resetting")]
    [SerializeField] private GameObject initialFish;

    [Tooltip("Letters to spawn when fish is fully grown")]
    [SerializeField] private GameObject[] lettersToSpawn;

    private static bool fishFeedingCompleted = false;

    //handles click behavior for fish objects 
    private void HandleFishClick()
    {
        if (fishFeedingCompleted) return;

        if (nextFishStage != null)
        {
            //progress to next growth stage
            nextFishStage.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            //fish is fully grown - spit letters and reset
            SpitLetters();
            ResetFish();

            fishFeedingCompleted = true;
        }
    }


    //activates all assigned letter objects
    private void SpitLetters()
    {
        if (lettersToSpawn == null) return;

        foreach (GameObject letter in lettersToSpawn)
        {
            if (letter != null)
            {
                letter.SetActive(true);
            }
        }
    }

    //resets fish to initial state
    private void ResetFish()
    {
        if (initialFish != null)
        {
            initialFish.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    //handles click behavior for letter objects
    private void HandleLetterClick()
    {
        gameObject.SetActive(false);
    }


    private void OnMouseDown()
    {

        switch (objectType)
        {
            case ObjectType.Fish:
                HandleFishClick();
                break;
            case ObjectType.Letter:
                HandleLetterClick();
                break;
        }
    }

}