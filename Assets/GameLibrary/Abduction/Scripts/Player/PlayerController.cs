﻿using Abduction.Data;
using Abduction.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Abduction.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float speed = 1.0f;

        [SerializeField]
        private float drag = 0.01f;

        [SerializeField]
        private float roll = 45f;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private Transform playerSprite;

        [SerializeField]
        private PlayerBeam playerBeam;

        [SerializeField]
        private PlayerLaser playerLaser;

        [SerializeField]
        private CharacterConfig[] characterOptions;

        #endregion

        #region Components

        private SpriteRenderer playerRenderer;
        private BoxCollider2D playerCollider;
        private Rigidbody2D playerBody;

        #endregion

        #region Member Variables

        private Vector2 movement;
        private Vector2 mousePosition;
        private Vector2 aimDirection;

        private bool fireLaser;
        private bool mouseAim;

        #endregion

        #region Properties

        public Vector2 Velocity { get; private set; }

        #endregion

        #region Life Cycle

        private void Awake()
        {
            playerRenderer = playerSprite.GetComponent<SpriteRenderer>();
            playerCollider = GetComponent<BoxCollider2D>();
            playerBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            CharacterConfig config = characterOptions[0];

            playerRenderer.sprite = config.CharacterSprite;
            playerBeam.BeamSprite = config.CharacterBeam;
            playerLaser.LaserSprite = config.CharacterLaser;
            playerLaser.BurstSprite = config.CharacterBurst;

            playerLaser.SenderTag = gameObject.tag;
        }

        private void OnEnable()
        {
            InputEvents.Game.Subscribe(GameEvents.Move, OnMove);
            InputEvents.Game.Subscribe(GameEvents.MouseAim, OnMouseAim);
            InputEvents.Game.Subscribe(GameEvents.JoystickAim, OnJoystickAim);
            InputEvents.Game.Subscribe(GameEvents.FireBeam, OnFireBeam);
            InputEvents.Game.Subscribe(GameEvents.FireLaser, OnFireLaser);
        }

        private void OnDisable()
        {
            InputEvents.Game.Unsubscribe(GameEvents.Move, OnMove);
            InputEvents.Game.Unsubscribe(GameEvents.MouseAim, OnMouseAim);
            InputEvents.Game.Unsubscribe(GameEvents.JoystickAim, OnJoystickAim);
            InputEvents.Game.Unsubscribe(GameEvents.FireBeam, OnFireBeam);
            InputEvents.Game.Unsubscribe(GameEvents.FireLaser, OnFireLaser);
        }

        #endregion

        #region Updates

        private void Update()
        {
            if (mouseAim)
            {
                Vector2 screenPosition = gameCamera.WorldToScreenPoint(transform.position);

                float x = mousePosition.x - screenPosition.x;
                float y = mousePosition.y - screenPosition.y;

                aimDirection.Set(x, y);
            }

            playerLaser.Aim = aimDirection;
            playerBeam.Aim = aimDirection;

            if (!aimDirection.Equals(Vector2.zero))
                playerLaser.IsFiring = !playerBeam.IsActive && fireLaser;
        }

        private void FixedUpdate()
        {
            Velocity = Vector2.MoveTowards(Velocity, movement * speed, drag);

            float rotation = playerSprite.localEulerAngles.z;
            if (Velocity.x != 0f)
            {
                float angle = (-Velocity.x / speed) * roll;
                rotation = Mathf.MoveTowardsAngle(rotation, angle, 3f);
            }
            else
            {
                rotation = Mathf.MoveTowardsAngle(rotation, 0f, 3f);
            }
            playerSprite.localEulerAngles = new Vector3(0, 0, rotation);

            playerBody.velocity = Velocity;
        }

        #endregion

        #region Input Event Handlers

        private void OnMove(InputValue inputValue)
        {
            movement = inputValue.Get<Vector2>();
        }

        private void OnMouseAim(InputValue inputValue)
        {
            mousePosition = inputValue.Get<Vector2>();
            mouseAim = true;
        }

        private void OnJoystickAim(InputValue inputValue)
        {
            mouseAim = false;
            aimDirection = inputValue.Get<Vector2>();
        }

        private void OnFireBeam(InputValue inputValue)
        {
            if (playerLaser.IsFiring && !aimDirection.Equals(Vector2.zero))
                return;

            playerBeam.IsActive = inputValue.isPressed;
        }

        private void OnFireLaser(InputValue inputValue)
        {
            fireLaser = inputValue.isPressed;
        }

        #endregion
    }
}
