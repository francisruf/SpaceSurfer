using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    [SerializeField] private EPlayerCharacterType _playerCharacterType;

    [Header("Player Prefabs")]
    [SerializeField] private GameObject _debugCharacter;
    [SerializeField] private GameObject _sailCharacter;

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

    private void Start()
    {
        SpawnPlayerCharacter();
    }

    private void SpawnPlayerCharacter()
    {
        GameObject characterPrefab = null;

        switch (_playerCharacterType)
        {
            case EPlayerCharacterType.DebugCharacter:
                characterPrefab = _debugCharacter;
                break;
            case EPlayerCharacterType.SailCharacter:
                characterPrefab = _sailCharacter;
                break;
        }

        Character character = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity).GetComponent<Character>();
        character.InitializeCharacter(true);
    }

    private void HandleNewPlayerController(PlayerController controller)
    {
        _playerController = controller;
    }
}
