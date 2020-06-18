using UnityEngine;
using System.Collections;

public class Flank : Division
{
    private bool isDoubleJump;
    private float jumpForce;

    public Flank() { }

    public override void Init(float _jumpForce)
    {
        jumpForce = _jumpForce;
        isDoubleJump = false;
    }

    public override void UseAbility()
    {
        Debug.LogError("Flank");
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
            isDoubleJump = false;
        }
        else
        {
            if (playerRig.velocity.y <= 0f && !isDoubleJump)
            {
                Vector3 vel = playerRig.velocity;
                vel.y = 0;
                playerRig.velocity = vel + Vector3.up * jumpForce;
                isDoubleJump = true;
            }
        }
    }
}
