using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMoveable
{
    CharacterController controller { get; set; }

    bool IsFacingPlayer { get; set; }

    void MoveEnemy(Vector3 normalizedDirection);
    void CheckForFacingPlayer(Vector3 playerPosition);

    void CheckForFacingDirection(Vector3 velocity);
}
