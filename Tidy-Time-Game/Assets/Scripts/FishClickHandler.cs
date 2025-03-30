using UnityEngine;

public class FishClickHandler : MonoBehaviour
{
    private FishManager manager;

    private void Start()
    {
        manager = FindObjectOfType<FishManager>();
    }

    private void OnMouseDown()
    {
        if (manager != null)
        {
            manager.OnBigFishClicked();
        }
    }
}
