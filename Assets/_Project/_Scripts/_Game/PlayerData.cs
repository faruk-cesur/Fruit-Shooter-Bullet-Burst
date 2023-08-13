using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public float GunDamage { get; set; }
}
