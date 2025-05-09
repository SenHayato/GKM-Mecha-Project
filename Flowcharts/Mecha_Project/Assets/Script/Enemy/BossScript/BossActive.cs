using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActive : EnemyActive
{
    [SerializeField] float rangeRadius;
    [SerializeField] float meleeRadius;

    [SerializeField] bool playerInMelee;
    [SerializeField] bool playerInRange;

    public override void Attacking()
    {

    }
    
    public override void PlayAnimation()
    {
        
    }
}
