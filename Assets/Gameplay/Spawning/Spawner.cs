using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private EPlayerCharacterType _playerCharacterType;
    public EPlayerCharacterType PlayerCharacterType { get { return _playerCharacterType; } }
}
