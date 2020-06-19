using UnityEngine;
using System.Collections;

public class Division : MonoBehaviour
{
    protected float jumpForce;
    protected float i_abilityCooldown, abilityCooldown;
    protected Rigidbody playerRig;
    public Division() { }

    public virtual void Init(float _jumpForce, float _abilityCooldown) 
    {
        jumpForce = _jumpForce;
        i_abilityCooldown = _abilityCooldown;
        abilityCooldown = 0f;
        playerRig = GetComponent<Rigidbody>();
    }

    public virtual void UseAbility() { }

    public virtual void UpdateDivisionStats() 
    {
        if (abilityCooldown > 0f)
            abilityCooldown -= Time.deltaTime;
    }

    public virtual void Jump(bool inAir) 
    {
        if (!inAir)
        {
            Vector3 vel = playerRig.velocity;
            vel.y = 0f;
            playerRig.velocity = vel + Vector3.up * jumpForce;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        Player thePlayer = GetComponent<Player>();
        thePlayer.SetHealth(thePlayer.GetHealth() - damage);
    }
}