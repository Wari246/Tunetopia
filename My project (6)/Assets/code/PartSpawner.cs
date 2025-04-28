using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public Transform partsAreaParent;
    public GameObject[] partPrefabs; // 登場順に並べる
    private int currentIndex = 0;

    public void SpawnNextPart()
    {
        if (currentIndex >= partPrefabs.Length) return;

        GameObject part = Instantiate(partPrefabs[currentIndex], partsAreaParent);
        part.transform.localPosition = GetSpawnPosition(currentIndex);
        currentIndex++;
    }

    private Vector3 GetSpawnPosition(int index)
    {
        // indexに応じてパーツエリア内で並べる
        return new Vector3(index * 2.0f, 0, 0); // 横に並べる例
    }
}
