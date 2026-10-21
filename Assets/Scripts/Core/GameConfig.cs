using UnityEngine;

namespace ProjectArtillery.Core
{
    /// <summary>
    /// Central tunable parameters for Project Artillery.
    /// Create via Assets → Create → Project Artillery → Game Config.
    /// Assign the same asset to PlayerController / PlayerAim (and later combat scripts).
    /// </summary>
    [CreateAssetMenu(
        fileName = "GameConfig",
        menuName = "Project Artillery/Game Config",
        order = 0)]
    public class GameConfig : ScriptableObject
    {
        [Header("Player Movement")]
        [Tooltip("Horizontal move speed in world units per second.")]
        [Min(0f)]
        public float moveSpeed = 5f;

        [Tooltip("Upward velocity applied on jump (world units per second).")]
        [Min(0f)]
        public float jumpSpeed = 8f;

        [Tooltip("Radius used for ground overlap checks.")]
        [Min(0.01f)]
        public float groundCheckRadius = 0.12f;

        [Header("Aiming")]
        [Tooltip("Minimum aim angle in degrees. 0° = right, 90° = up.")]
        [Range(0f, 90f)]
        public float minAimAngleDegrees = 5f;

        [Tooltip("Maximum aim angle in degrees. 0° = right, 90° = up.")]
        [Range(90f, 180f)]
        public float maxAimAngleDegrees = 175f;

        [Tooltip("Length of the aim line drawn in the scene.")]
        [Min(0.1f)]
        public float aimLineLength = 2.5f;

        [Header("Future Combat (unused in Phase 1)")]
        [Min(1)]
        public int playerMaxHealth = 100;

        [Min(0f)]
        public float projectileSpeed = 14f;

        [Min(0f)]
        public float projectileDamage = 20f;

        [Min(0.01f)]
        public float explosionRadius = 2f;

        [Min(0f)]
        public float fireCooldownSeconds = 0.8f;

        [Min(1)]
        public int maxAliveProjectilesPerPlayer = 3;

        /// <summary>
        /// Clamps an aim angle (degrees) into the configured [min, max] range.
        /// </summary>
        public float ClampAimAngle(float angleDegrees)
        {
            float min = Mathf.Min(minAimAngleDegrees, maxAimAngleDegrees);
            float max = Mathf.Max(minAimAngleDegrees, maxAimAngleDegrees);
            return Mathf.Clamp(angleDegrees, min, max);
        }
    }
}
