using UnityEngine;

[CreateAssetMenu]
public class ItemSpawnerData : ScriptableObject
{
    [field: SerializeField] public float SpawnTime { get; set; }
}