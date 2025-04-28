using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnObjectRemoved : MonoBehaviour
{
    public Transform targetLocation; // 監視する特定の場所
    public Transform spawnLocation; // 新しいパーツの生成場所
    public GameObject partPrefab; // 生成するパーツのプレハブ
    public string targetTag = "TargetObject"; // 監視対象オブジェクトのタグ
    public float moveSpeed = 3.0f; // 移動速度

    void Update()
    {
        // 監視対象のオブジェクトがまだ存在するか確認
        GameObject existingObject = GameObject.FindGameObjectWithTag(targetTag);
        if (existingObject == null)
        {
            StartCoroutine(MoveToTarget());
            SpawnNewPart();
        }
    }

    IEnumerator MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float journeyTime = Vector3.Distance(startPosition, targetLocation.position) / moveSpeed;

        while (elapsedTime < journeyTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetLocation.position, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetLocation.position;
        Debug.Log(gameObject.name + " moved to " + targetLocation.position);
    }

    void SpawnNewPart()
    {
        if (partPrefab != null && spawnLocation != null)
        {
            Instantiate(partPrefab, spawnLocation.position, Quaternion.identity);
            Debug.Log("New part spawned at " + spawnLocation.position);
        }
        else
        {
            Debug.LogWarning("partPrefab or spawnLocation is not assigned!");
        }
    }
}
