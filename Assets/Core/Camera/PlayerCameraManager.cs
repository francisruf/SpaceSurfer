using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    private PlayerController _player;
    private bool _shouldFollow;
    Vector3 targetPos = new Vector3();

    private void OnEnable()
    {
        PlayerController.OnPlayerControllerInstantiate += HandleNewPlayerController;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerControllerInstantiate -= HandleNewPlayerController;
    }

    private void Awake()
    {
        targetPos.z = transform.position.z;
    }

    private void HandleNewPlayerController(PlayerController controller)
    {
        _player = controller;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log(GameManager.instance.PlayerController.gameObject.name);
    }
}
