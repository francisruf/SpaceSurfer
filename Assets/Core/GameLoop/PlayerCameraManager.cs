using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    private Character _player;
    private bool _shouldFollow;
    Vector3 targetPos = new Vector3();

    private void OnEnable()
    {
        Character.OnPlayerCharacterEnabled += HandleNewPlayerCharacter;
    }

    private void OnDisable()
    {
        Character.OnPlayerCharacterEnabled += HandleNewPlayerCharacter;
    }

    private void Awake()
    {
        targetPos.z = transform.position.z;
    }
    private void HandleNewPlayerCharacter(Character character)
    {
        _player = character;
        _shouldFollow = _player != null ? true : false;
    }

    private void LateUpdate()
    {
        if (!_shouldFollow)
            return;

        targetPos.x = _player.transform.position.x;
        targetPos.y = _player.transform.position.y;
        transform.position = targetPos;
    }
}
