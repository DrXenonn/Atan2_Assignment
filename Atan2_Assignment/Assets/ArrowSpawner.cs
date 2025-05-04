using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ArrowPrefab;
    [SerializeField] private float ArrowSpeed;
    [SerializeField] private float SpawnInterval;

    private ObjectPool<GameObject> _arrowPool;
    private float _timer;

    private void Awake()
    {
        _arrowPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Instantiate(ArrowPrefab);
                obj.SetActive(false);
                return obj;
            },
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
            },
            actionOnDestroy: (obj) =>
            {
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: 10
        );
    }

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 0, SpawnInterval);
    }

    [Obsolete("Obsolete")]
    private void Spawn()
    {
        var arrow = _arrowPool.Get();
        arrow.transform.position = transform.position;

        var direction = Random.insideUnitCircle.normalized;

        var rb = arrow.GetComponent<Rigidbody2D>();
        rb.velocity = direction * ArrowSpeed;

        var angleDeg = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        StartCoroutine(ReturnToPoolAfterDelay(arrow, 3f));
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        _arrowPool.Release(arrow);
    }
}