using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{    
    [SerializeField] private ItemSpawnerData _itemSpawnerData;
    [SerializeField] private Timer _itemSpawnTimer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(_itemSpawnTimer.StartTimer());
        }
    }
    
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
        var objectPool = ObjectPool.Instance;
        var randomIndex = Random.Range(0, objectPool.Pools.Length);
        var spawnedRandomItem = objectPool.GetPooledObject(randomIndex);
        //spawnedRandomBullet.transform.SetParent(null); // todo it gives null ref?!

        yield return new WaitForSeconds(2f);

        objectPool.SetPooledObject(spawnedRandomItem, randomIndex);
        //spawnedRandomBullet.transform.SetParent(objectPool.transform); 
        spawnedRandomItem.transform.ResetLocalPos();
        spawnedRandomItem.transform.ResetLocalRot();
    }
}