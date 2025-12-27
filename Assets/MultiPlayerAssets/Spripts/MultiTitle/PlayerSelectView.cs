using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelectView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private Transform _scrollView;
    [SerializeField] private Image _baseImage;
    [SerializeField] private Image _nameBaseImage;
    [SerializeField] private Image _selectedImage;
    [SerializeField] private Image _applyImage;

    [Header("Config")]
    [SerializeField] private float _scrollSpeed = 0.3f;
    private const float ScrollWidth = 300;
    private const int SelectCharacterMax = 5; //８に変更する必要ある
    private const float InputThreshold = 0.5f;

    private PlayerInput _playerInput;
    private int _playerIndex = 0;
    private int _selectIndex = 0;
    private bool _isAction = false;
    private bool _isSelected = false;

    private System.Action<int, CharacterType> _selectedCallback;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.onActionTriggered += OnScrollView;
    }

    private void OnDestroy()
    {
        _playerInput.onActionTriggered -= OnScrollView;
    }

    public void Setup(int playerIndex, System.Action<int, CharacterType> selectedCallback)
    {
        _selectedCallback = selectedCallback;
        _playerIndex = playerIndex;
        _playerNameText.text = $"Player {_playerIndex + 1}";
        _nameBaseImage.color = CommonParam.IndexColorList[playerIndex];
        _baseImage.color = CommonParam.IndexColorList[playerIndex];
    }

    private void OnScrollView(InputAction.CallbackContext context)
    {
        if(context.action.name.Equals("Navigate"))
        {
            if (_isAction || _isSelected) return;
            var direction = context.ReadValue<Vector2>();
            //if (direction == Vector2.zero) return;
            if (direction.x > InputThreshold && _selectIndex < SelectCharacterMax - 1) ScrollView(-1);
            else if (direction.x < -InputThreshold && _selectIndex > 0) ScrollView(1);
        }
        else if (context.action.name.Equals("Apply"))
        {
            IsSelected();
        }
    }

    private void ScrollView(int dir)
    {
        _isAction = true;
        var startPos = _scrollView.localPosition;
        var endPos = startPos + Vector3.right * ScrollWidth * dir;
        LMotion.Create(startPos, endPos, _scrollSpeed)
        .WithEase(Ease.Linear)
        .WithOnComplete(() => { _selectIndex += -1 * dir; _isAction = false; }) // Withはどの順番でも大丈夫
        .BindToLocalPosition(_scrollView)
        .AddTo(gameObject);
    }

    private void IsSelected()
    { 
        var targetChar = (CharacterType)_selectIndex;
        if (GameMainManager.Instance.IsCharacterTake(targetChar))
        {
            Debug.Log($"Character{targetChar} is already take");
            return;
        }

        _isSelected = true;
        _selectedImage.gameObject.SetActive(false);
        _applyImage.gameObject.SetActive(true);
        _selectedCallback?.Invoke(_playerIndex, targetChar);
    }
}
