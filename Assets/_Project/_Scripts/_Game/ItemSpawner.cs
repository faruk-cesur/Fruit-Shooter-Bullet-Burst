using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("SETTINGS")] public float _minSpawnDelay = 0.25f;
    [SerializeField, BoxGroup("SETTINGS")] public float _maxSpawnDelay = 2f;
    [SerializeField, BoxGroup("SETTINGS")] private float _minRotateAngle;
    [SerializeField, BoxGroup("SETTINGS")] private float _maxRotateAngle;

    [SerializeField, BoxGroup("SETUP")] private ObjectPool _itemObjectPool;
    [SerializeField, BoxGroup("SETUP")] private Collider _spawnArea;
    private int _randomIndex;

    private void OnEnable()
    {
        StartCoroutine(SpawnItem());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitUntil(() => GameManager.Instance.CurrentGameState == GameState.Gameplay);
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            Vector3 position = new Vector3();
            position.x = Random.Range(_spawnArea.bounds.min.x, _spawnArea.bounds.max.x);
            position.y = Random.Range(_spawnArea.bounds.min.y, _spawnArea.bounds.max.y);
            position.z = Random.Range(_spawnArea.bounds.min.z, _spawnArea.bounds.max.z);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(_minRotateAngle, _maxRotateAngle));

            _randomIndex = Random.Range(0, _itemObjectPool.Pools.Length);
            GameObject prefab = _itemObjectPool.GetPooledObject(_randomIndex);
            prefab.transform.position = position;
            prefab.transform.rotation = rotation;
            yield return new WaitForSeconds(Random.Range(_minSpawnDelay, _maxSpawnDelay));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<Fruit>(out Fruit fruit))
        {
            if (fruit.IsImmune)
                return;

            for (var i = 0; i < _itemObjectPool.Pools.Length; i++)
            {
                var pool = _itemObjectPool.Pools[i];
                if (pool.ObjectPrefab.name + "(Clone)" == other.transform.parent.name)
                {
                    _itemObjectPool.SetPooledObject(other.transform.parent.gameObject, i);
                }
            }
        }
    }
}