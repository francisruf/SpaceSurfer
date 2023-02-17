using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalWindController : MonoBehaviour
{
    // Components
    private NoiseRenderer _windRenderer;

    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private Vector2 _debugDirection;

    private Vector3Int _playerPositionOffset = Vector3Int.zero;
    private Vector2[] _currentOctaveOffsets;
    private float _time = 0.01f;

    private void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandlePlayerPositionUpdate;
    }

    private void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandlePlayerPositionUpdate;
    }

    private void Awake()
    {
        _windRenderer = GetComponent<NoiseRenderer>();
    }

    private void Start()
    {
        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);
    }

    private void Update()
    {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(_noiseSettings, _currentOctaveOffsets, _playerPositionOffset, _time, _debugDirection, false);

        _windRenderer.RenderNoiseMapByColor(noiseMap, _noiseSettings, _playerPositionOffset);
        _time += Time.deltaTime;
    }

    private void HandlePlayerPositionUpdate(Character playerCharacter)
    {
        Vector3 playerPos = playerCharacter.transform.position;
        Vector3Int playerPosInt = new Vector3Int();
        playerPosInt.x = Mathf.RoundToInt(playerPos.x);
        playerPosInt.y = Mathf.RoundToInt(playerPos.y);
        playerPosInt.z = 0;

        _playerPositionOffset = playerPosInt;
    }

}
