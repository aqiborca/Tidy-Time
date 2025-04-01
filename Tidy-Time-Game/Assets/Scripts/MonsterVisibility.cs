using UnityEngine;

public class MonsterVisibility : MonoBehaviour
{
    public GameObject monsterObject;

    void Start()
    {
        // Initial check when scene loads
        UpdateMonsterVisibility();
    }

    void Update()
    {
        // Keep checking every frame for when the monster is scared away
        UpdateMonsterVisibility();
    }

    void UpdateMonsterVisibility()
    {
        if (MonsterSpawnManager.Instance != null)
        {
            bool shouldBeVisible = MonsterSpawnManager.Instance.IsMonsterActive();

            // Update visibility according to events
            if (monsterObject.activeSelf != shouldBeVisible)
            {
                monsterObject.SetActive(shouldBeVisible);
                Debug.Log("Monster visibility: " + shouldBeVisible);
            }
        }
    }
}
