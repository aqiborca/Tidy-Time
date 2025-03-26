using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Item"; //for debugging

    void OnMouseDown()
    {
        Debug.Log(itemName + " collected");
        gameObject.SetActive(false); 
    }
}
