using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerCheckable
{
    bool IsChased { get; set; }

    bool IsWithinAttackDistance {  get; set; }
    void SetChaseStatus(bool isChased);

    void SetAttackDistancebool(bool isAttackDistance);
}
