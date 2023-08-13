using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public float GunDamage { get; set; }
    [field: SerializeField] public float BulletReloadDuration { get; set; }
}
