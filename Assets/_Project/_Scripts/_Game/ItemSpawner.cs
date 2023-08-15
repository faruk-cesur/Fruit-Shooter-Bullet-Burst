using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemSpawnerData _itemSpawnerData;
    [SerializeField] private Timer _itemSpawnTimer;
    [SerializeField] private ObjectPool _itemObjectPool;

    private void ResetSpawnTime()
    {
        _itemSpawnTimer.SetTimer(_itemSpawnerData.SpawnTime);
    }

    private IEnumerator SpawnItemFromObjectPool()
    {
        while (_itemSpawnTimer.CurrentTime > 0)
        {
            yield break;
        }

        ResetSpawnTime();

        var randomIndex = Random.Range(0, _itemObjectPool.Pools.Length);
        var spawnedRandomItem = _itemObjectPool.GetPooledObject(randomIndex);
        //spawnedRandomBullet.transform.SetParent(null); // todo it gives null ref?!

        yield return new WaitForSeconds(2f);

        _itemObjectPool.SetPooledObject(spawnedRandomItem, randomIndex);
        //spawnedRandomBullet.transform.SetParent(objectPool.transform); 
        spawnedRandomItem.transform.ResetLocalPos();
        spawnedRandomItem.transform.ResetLocalRot();
    }
}