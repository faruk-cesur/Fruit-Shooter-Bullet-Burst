using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public float GunDamage { get; set; }
    [field: SerializeField] public float GunReloadTime { get; set; }
    [field: SerializeField] public float GunBulletAmount { get; set; }
}
