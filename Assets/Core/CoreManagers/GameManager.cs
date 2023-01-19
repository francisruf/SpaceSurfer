using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Getters
    private PlayerController _playerController;
    public PlayerController PlayerController { get { return _playerController; } }

    private void OnEnable()
    {
        PlayerController.OnPlayerControllerInstantiate += HandleNewPlayerController;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerControllerInstantiate -= HandleNewPlayerController;
    }

    // TODO : Create instance wrapper class
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void HandleNewPlayerController(PlayerController controller)
    {
        _playerController = controller;
    }
}
