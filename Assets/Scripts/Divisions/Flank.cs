using UnityEngine;
using System.Collections;

public class Flank : Division
{
    private bool isDoubleJump;

    public Flank() { }

    public override void Init(float _jumpForce, float _abilityCooldown)
    {
        base.Init(_jumpForce, _abilityCooldown);
        isDoubleJump = false;
    }

    public override void UpdateDivisionStats()
    {
        base.UpdateDivisionStats();
    }

    public override void UseAbility()
    {
        if (abilityCooldown <= 0f)
        {
            Vector3 dir = transform.forward;
            dir.y = 0f;
            playerRig.AddForce(dir * 500f, ForceMode.Impulse);
            abilityCooldown = i_abilityCooldown;
        }
    }

    public override void Jump(bool inAir)
    {
        if (!inAir)
        {
            Vector3 vel = playerRig.velocity;
            vel.y = 0f;
            playerRig.velocity = vel + Vector3.up * jumpForce;
            isDoubleJump = false;
        }
        else
        {
            if (playerRig.velocity.y <= 0f && !isDoubleJump)
            {
                Vector3 vel = playerRig.velocity;
                vel.y = 0f;
                playerRig.velocity = vel + Vector3.up * jumpForce;
                isDoubleJump = true;
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
