using UnityEngine;

public class AssemblyRootReceiver : MonoBehaviour
{
    public static AssemblyRootReceiver Instance { get; private set; }

    private void Awake()
    {
        // シングルトン（1つだけ存在することを保証）
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // スケールは常に (1,1,1)
        transform.localScale = Vector3.one;
    }

    // 呼び出し元から登録される
    public void RegisterAsChild(Transform obj)
    {
        obj.SetParent(transform, worldPositionStays: true);
    }
}
