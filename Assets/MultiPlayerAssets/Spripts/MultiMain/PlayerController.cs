using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tag] private string _hitTag;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _jumpPower = 1f;
    [SerializeField] private List<Sprite> _sprites = new List<Sprite>();
    [SerializeField] private SpriteRenderer _playerImage;

    private PlayerInput _playerInput;
    private bool _isGround = false;
    private bool _isJump = false;
    private Rigidbody2D _rb;
    private int _playerIndex;
    private CharacterType _characterType;
    private Vector2 _direction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _playerInput.onActionTriggered += OnMove;
    }

    private void OnDisable()
    {
        _playerInput.onActionTriggered -= OnMove;
    }

    public void Setup(int playerIndex, CharacterType characterType)
    {
        _playerIndex = playerIndex;
        _characterType = characterType;
        _playerImage.sprite = _sprites[(int)characterType];
    }

    private void FixedUpdate()
    {
        if(_isJump)
        {
            _isJump = false;
            _rb.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }
        var velocity = new Vector2(_direction.x * _speed * Time.deltaTime, 0);
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if(context.action.name.Equals("Move"))
        {
            _direction = context.ReadValue<Vector2>();
        }
        if(_isGround && context.action.name.Equals("Jump"))
        {
            _isJump = true;
            _isGround = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_isGround is false && collision.gameObject.CompareTag(_hitTag))
        {
            _isGround = true;
        }
    }
}
