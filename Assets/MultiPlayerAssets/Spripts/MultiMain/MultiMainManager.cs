using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiMainManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        GameMainManager.Instance.Players.ForEach(player =>
        {
            var playerSprite = PlayerInput.Instantiate(prefab: _playerController.gameObject,
            playerIndex: player.PlayerIndex,
            pairWithDevice: player.PairWithDevice);
            playerSprite.GetComponent<PlayerController>().Setup(player.PlayerIndex, player.SelectedCharacter);
        });
    }
}
