using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject buffPrefab;
    public GameObject damagePrefab;

    [Header("Sounds")]
    public AudioClip damageSound;
    public AudioClip buffSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
