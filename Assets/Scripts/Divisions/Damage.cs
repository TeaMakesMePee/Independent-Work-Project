using UnityEngine;
using System.Collections;

public class Damage : Division
{
    private float jumpForce;

    public Damage() { }

    public override void Init(float _jumpForce)
    {
        jumpForce = _jumpForce;
    }

    public override void UseAbility()
    {
        Debug.LogError("Dmg");
    }

    public override void UpdateDivisionStats()
    {
        
    }

    public override void Jump(bool inAir, Rigidbody playerRig)
    {
        if (!inAir)
        {
            Vector3 vel = playerRig.velocity;
            vel.y = 0;
            playerRig.velocity = vel + Vector3.up * jumpForce;
        }
    }
}
