using ProjectArtillery.Core;
using UnityEngine;

namespace ProjectArtillery.Player
{
    /// <summary>
    /// Phase 1 player locomotion: A/D (or arrows) horizontal move, Space jump when grounded.
    /// Requires Rigidbody2D and a ground-check transform. Uses legacy Input Manager.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private GameConfig config;

        [Header("Ground Check")]
        [SerializeField]
        private Transform groundCheck;

        [SerializeField]
        private LayerMask groundLayers = ~0;

        [Header("Optional")]
        [Tooltip("If true, horizontal input is ignored (e.g. after death in later phases).")]
        [SerializeField]
        private bool inputEnabled = true;

        private Rigidbody2D _body;
        private float _moveInput;
        private bool _jumpRequested;
        private bool _isGrounded;

        /// <summary>True when the ground check overlaps configured ground layers.</summary>
        public bool IsGrounded => _isGrounded;

        /// <summary>Enable or disable player movement input.</summary>
        public bool InputEnabled
        {
            get => inputEnabled;
            set => inputEnabled = value;
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            ConfigureRigidbody();
        }

        private void ConfigureRigidbody()
        {
            if (_body == null)
            {
                return;
            }

            _body.freezeRotation = true;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            _body.gravityScale = 1f;
        }

        private void Update()
        {
            if (!inputEnabled)
            {
                _moveInput = 0f;
                _jumpRequested = false;
                return;
            }

            // Legacy Input Manager: Project Settings → Player → Active Input Handling = Both or Input Manager.
            _moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
                _jumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            RefreshGrounded();

            float speed = config != null ? config.moveSpeed : 5f;
            Vector2 velocity = _body.linearVelocity;
            velocity.x = _moveInput * speed;

            if (_jumpRequested)
            {
                if (_isGrounded)
                {
                    float jump = config != null ? config.jumpSpeed : 8f;
                    velocity.y = jump;
                }

                _jumpRequested = false;
            }

            _body.linearVelocity = velocity;
        }

        private void RefreshGrounded()
        {
            if (groundCheck == null)
            {
                _isGrounded = false;
                return;
            }

            float radius = config != null ? config.groundCheckRadius : 0.12f;
            Collider2D hit = Physics2D.OverlapCircle(
                groundCheck.position,
                radius,
                groundLayers);

            _isGrounded = hit != null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
            {
                return;
            }

            float radius = config != null ? config.groundCheckRadius : 0.12f;
            Gizmos.color = _isGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, radius);
        }
#endif
    }
}
