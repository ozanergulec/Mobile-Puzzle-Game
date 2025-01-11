using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float destroyAfterSeconds = 2f;

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
}
