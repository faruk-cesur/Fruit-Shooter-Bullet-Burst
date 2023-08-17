using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class GameplayData : ScriptableObject
{
    [field: SerializeField, BoxGroup("Static For Now")] public float GunDamage { get; set; }
    [field: SerializeField, BoxGroup("Static For Now")] public float GunReloadTime { get; set; }
    [field: SerializeField, BoxGroup("Bullet Amount")] public int BulletAmount { get; set; }
    [field: SerializeField, BoxGroup("Aim Sensitivity")] public float AimSensitivity { get; set; }
    [field: SerializeField, BoxGroup("Aim Shake")] public float AimShakeDuration { get; set; }
    [field: SerializeField, BoxGroup("Aim Shake")] public float AimShakeStrength { get; set; }
    [field: SerializeField, BoxGroup("Aim Shake")] public int AimShakeVibrato { get; set; }
    [field: SerializeField, BoxGroup("Aim Shake")] public float AimShakeRandomness { get; set; }
    [field: SerializeField, BoxGroup("God Mode")] public float GodMode { get; set; }
}