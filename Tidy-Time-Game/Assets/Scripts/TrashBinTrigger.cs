using UnityEngine;

public class TrashBinTrigger : MonoBehaviour
{
    private int trashInBinCount = 0;
    private int totalTrashPieces;

    void Start()
    {
        //count all GameObjects tagged "Trash" at the start
        totalTrashPieces = GameObject.FindGameObjectsWithTag("Trash").Length;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trash"))
        {
            Debug.Log("Trash got in the bin");

            trashInBinCount++;

            //prevent it from being counted again
            other.tag = "Untagged";

            //if all trash pieces are in, mark as complete
            if (trashInBinCount >= totalTrashPieces)
            {
                Debug.Log("All trash is in the bin");
                ChoreManager.Instance.CompleteChore("garbage");
            }
        }
    }
}
