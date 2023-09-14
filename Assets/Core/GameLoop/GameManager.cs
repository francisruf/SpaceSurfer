using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    [SerializeField] private EPlayerCharacterType _defaultCharacterType;

    [Header("Player Prefabs")]
    [SerializeField] private GameObject _debugCharacter;
    [SerializeField] private GameObject _debugCharacter_YellowDude;
    [SerializeField] private GameObject _sailCharacter;

    [Header("Spawning")]
    [SerializeField] private Spawner _defaultSpawn;

    // Getters
    private PlayerController _playerController;
    public PlayerController PlayerController { get { return _playerController; } }
    private Character _playerCharacter;
    public Character PlayerCharacter { get { return _playerCharacter; } }

    private void OnEnable()
    {
        SailCharacter.onSailCharacterExplode += HandleSailCharacterExplore;
    }

    private void OnDisable()
    {
        SailCharacter.onSailCharacterExplode -= HandleSailCharacterExplore;
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
        SpawnPlayerCharacter(_defaultSpawn);
    }

    private void SpawnPlayerCharacter(Spawner spawner)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        EPlayerCharacterType characterType = _defaultCharacterType;

        if (spawner != null)
        {
            spawnPos = spawner.transform.position;
            spawnRotation = spawner.transform.rotation;
            characterType = spawner.PlayerCharacterType;
        }

        SpawnPlayerCharacter(spawnPos, spawnRotation, characterType);
    }

    private void SpawnPlayerCharacter(Vector3 spawnPos, Quaternion spawnRotation, EPlayerCharacterType characterType)
    {
        GameObject characterPrefab = null;

        switch (characterType)
        {
            case EPlayerCharacterType.DebugCharacter:
                characterPrefab = _debugCharacter;
                break;
            case EPlayerCharacterType.DebugCharacter_YellowDude:
                characterPrefab = _debugCharacter_YellowDude;
                break;
            case EPlayerCharacterType.SailCharacter:
                characterPrefab = _sailCharacter;
                break;
        }

        _playerCharacter = Instantiate(characterPrefab, spawnPos, spawnRotation).GetComponent<Character>();
        PlayerController controller = _playerCharacter.GetComponent<PlayerController>();

        if (controller != null)
        {
            _playerController = controller;
            controller.Possess(_playerCharacter);
            controller.SetControllerState(EPlayerControllerState.MoveCharacter);
        }

        _playerCharacter.EnableCharacter(true);
    }

    // TODO : Move to separate gamemode managers
    private void HandleSailCharacterExplore(SailCharacter character)
    {
        StartCoroutine(RespawnSailCharacter());
    }

    private IEnumerator RespawnSailCharacter()
    {
        yield return new WaitForSeconds(2f);

        Vector3 respawnPos = Vector3.zero;
        Quaternion respawnRotation = Quaternion.identity;

        if (_defaultSpawn != null)
        {
            respawnPos = _defaultSpawn.transform.position;
            respawnRotation = _defaultSpawn.transform.rotation;
        }

        _playerCharacter.Teleport(respawnPos, respawnRotation);
        _playerCharacter.EnableCharacter(true);
    }
}
