using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum CharacterType
{
    Character1, Character2, Character3, Character4, None　//8まで入れる
}

public class CommonParam
{
    public static readonly List<Color> IndexColorList = new List<Color>() 
        {
            new Color(0, 0, 1, 0.4f),
            new Color(1, 0, 0, 0.4f),
            new Color(0, 1, 0, 0.4f),
            new Color(1, 1, 0, 0.4f),                
        };
}

public class PlayerInfo
{
    public InputDevice PairWithDevice { get; private set; } = default;
    public CharacterType SelectedCharacter { get; private set; } = default;
    public int PlayerIndex { get; private set; } = default(int);

    public void SetCharacterType(CharacterType characterType)
    {
        this.SelectedCharacter = characterType;
    }

    public PlayerInfo(InputDevice pairWithDevice, CharacterType selectedCharacter, int index)
    {
        PairWithDevice = pairWithDevice;
        SelectedCharacter = selectedCharacter;
        PlayerIndex = index;
    }
}


public class GameMainManager : Singleton<GameMainManager>
{
    [Header("Input Settings")]
    [SerializeField] private InputAction _action;

    [Header("UI Settings")]
    [SerializeField] private FadeManager _fadeManager;  
    [SerializeField] private Transform _selectParent;
    [SerializeField] private PlayerSelectView _playerSelectPrefab;

    [Header("Game Settings")]
    [SerializeField, Scene] private int _nextSceneIndex;
    [SerializeField] private int _selectPlayerMax = 4;

    private List<PlayerInfo> _players = new List<PlayerInfo>();
    public List<PlayerInfo> Players => _players;
    private int _selectedCount = 0;

    public override void Awake()
    {
        base.Awake();
        if(Instance != this)return;

        if (_action != null)
        {
            _action.Enable();
            _action.performed += OnJoin;
        }
    }

    private void OnDestory()
    {
        if (_action != null)
        {
            _action.performed -= OnJoin;
            _action.Disable();
        }
    }

    /// <summary>
    /// キャラクターが重複してないか確認する
    /// </summary>
    public bool IsCharacterTake(CharacterType characterType)
    {
        if (characterType == CharacterType.None)return false;

        return _players.Any(p => p.SelectedCharacter == characterType);
    }

    private void OnJoin(InputAction.CallbackContext context)
    {
        var device = context.control.device;
        //上限重複参加確認
        if (_players.Count >= _selectPlayerMax) return;
        if (_players.Any(player => player.PairWithDevice.Equals(device))) return;
        int index = _players.Count;
        //InputでUIつくる
        var player = PlayerInput.Instantiate(prefab: _playerSelectPrefab.gameObject,
            playerIndex: index,
            pairWithDevice: device);
        player.transform.SetParent(_selectParent, false);
        if (player.TryGetComponent<PlayerSelectView>(out var view))
            {
              view.Setup(index, SelectedCallback);
            }
        _players.Add(new PlayerInfo(device, CharacterType.None, index));
        Debug.Log($"Player{index} Joined");
    }

    private void SelectedCallback(int index, CharacterType characterType)
    {
        _players[index].SetCharacterType(characterType);
        _selectedCount++;
        if(_players.Count >= 2 && _selectedCount >= _players.Count)
        {
            _action?.Disable();
            NextScene().Forget();
            Debug.Log("A");
        }
    }

    private async UniTask NextScene()
    {
        await UniTask.WaitForSeconds(1);
        bool isFading = true;
        if (_fadeManager != null)
        {
            _fadeManager.FadeOut(() => isFading = false);
            await UniTask.WaitUntil(() => isFading is false);
        }
        await SceneManager.LoadSceneAsync(_nextSceneIndex);
        if (_fadeManager != null) _fadeManager.FadeIn();
    }

}
