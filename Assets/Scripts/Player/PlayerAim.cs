using ProjectArtillery.Core;
using UnityEngine;

namespace ProjectArtillery.Player
{
    /// <summary>
    /// Phase 1 mouse aiming with angle clamp (~5°–175°) and LineRenderer feedback.
    /// Angle convention: 0° = right, 90° = up (Unity world X right, Y up).
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerAim : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private GameConfig config;

        [Header("References")]
        [Tooltip("World origin of the barrel / aim pivot. Defaults to this transform.")]
        [SerializeField]
        private Transform aimOrigin;

        [Tooltip("Optional child rotated to match the aim direction (e.g. a barrel sprite).")]
        [SerializeField]
        private Transform aimVisual;

        [Tooltip("LineRenderer used to draw the aim direction. Created at runtime if missing.")]
        [SerializeField]
        private LineRenderer aimLine;

        [Header("Camera")]
        [Tooltip("Camera used to convert mouse screen position to world. Defaults to Camera.main.")]
        [SerializeField]
        private Camera aimCamera;

        [Header("Debug UI")]
        [SerializeField]
        private bool showAngleOnScreen = true;

        private float _currentAngleDegrees = 90f;

        /// <summary>Current clamped aim angle in degrees (0° right, 90° up).</summary>
        public float CurrentAngleDegrees => _currentAngleDegrees;

        /// <summary>Unit direction matching <see cref="CurrentAngleDegrees"/>.</summary>
        public Vector2 AimDirection => AngleToDirection(_currentAngleDegrees);

        private void Awake()
        {
            if (aimOrigin == null)
            {
                aimOrigin = transform;
            }

            if (aimCamera == null)
            {
                aimCamera = Camera.main;
            }

            EnsureAimLine();
        }

        private void Update()
        {
            if (aimCamera == null)
            {
                aimCamera = Camera.main;
                if (aimCamera == null)
                {
                    return;
                }
            }

            Vector3 mouseScreen = Input.mousePosition;
            // Keep Z relative to camera so ScreenToWorldPoint is valid for orthographic 2D.
            mouseScreen.z = Mathf.Abs(aimCamera.transform.position.z - aimOrigin.position.z);
            Vector3 mouseWorld = aimCamera.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = aimOrigin.position.z;

            Vector2 toMouse = (Vector2)(mouseWorld - aimOrigin.position);
            float rawAngle = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

            if (config != null)
            {
                _currentAngleDegrees = config.ClampAimAngle(rawAngle);
            }
            else
            {
                _currentAngleDegrees = Mathf.Clamp(rawAngle, 5f, 175f);
            }

            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            Vector2 direction = AimDirection;
            float length = config != null ? config.aimLineLength : 2.5f;

            if (aimVisual != null)
            {
                float zRotation = _currentAngleDegrees;
                aimVisual.rotation = Quaternion.Euler(0f, 0f, zRotation);
            }

            if (aimLine != null)
            {
                Vector3 start = aimOrigin.position;
                Vector3 end = start + (Vector3)(direction * length);
                aimLine.positionCount = 2;
                aimLine.SetPosition(0, start);
                aimLine.SetPosition(1, end);
            }
        }

        private void EnsureAimLine()
        {
            if (aimLine != null)
            {
                ConfigureLine(aimLine);
                return;
            }

            aimLine = GetComponent<LineRenderer>();
            if (aimLine == null)
            {
                aimLine = gameObject.AddComponent<LineRenderer>();
            }

            ConfigureLine(aimLine);
        }

        private static void ConfigureLine(LineRenderer line)
        {
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.startWidth = 0.05f;
            line.endWidth = 0.02f;
            line.numCapVertices = 2;
            line.sortingOrder = 10;

            if (line.sharedMaterial == null)
            {
                // Default-Line / Sprites-Default work without custom materials in a fresh project.
                Shader shader = Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    line.material = new Material(shader);
                }
            }

            line.startColor = new Color(1f, 0.85f, 0.2f, 1f);
            line.endColor = new Color(1f, 0.4f, 0.1f, 0.85f);
        }

        private static Vector2 AngleToDirection(float angleDegrees)
        {
            float radians = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        private void OnGUI()
        {
            if (!showAngleOnScreen)
            {
                return;
            }

            const float pad = 12f;
            GUI.Label(
                new Rect(pad, pad, 320f, 24f),
                $"Aim angle: {_currentAngleDegrees:0.0}°  (0° right, 90° up)");
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Transform origin = aimOrigin != null ? aimOrigin : transform;
            float length = config != null ? config.aimLineLength : 2.5f;
            Vector2 dir = AngleToDirection(_currentAngleDegrees);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin.position, origin.position + (Vector3)(dir * length));
        }
#endif
    }
}
