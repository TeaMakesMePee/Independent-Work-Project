using UnityEngine;
using System.Collections;

public class Damage : Division
{
    private float jumpForce;
    private float i_abilityCooldown, abilityCooldown;
    private Rigidbody playerRig;

    public Damage() { }

    public override void Init(float _jumpForce, float _abilityCooldown)
    {
        jumpForce = _jumpForce;
        i_abilityCooldown = _abilityCooldown;
        abilityCooldown = 0f;
        playerRig = GetComponent<Rigidbody>();
    }

    public override void UpdateDivisionStats()
    {
        if (abilityCooldown >= 0f)
            abilityCooldown -= Time.deltaTime;
    }

    public override void UseAbility()
    {
        if (abilityCooldown < 0f)
        {
            //Do ability
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
        }
    }
}
