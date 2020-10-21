using System.Collections;
using UnityEngine;

public class AutoDestruction : MonoBehaviour
{
    public float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestruction());
    }

    private IEnumerator SelfDestruction()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
