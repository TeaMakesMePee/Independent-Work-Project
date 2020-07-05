using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

public class Division : MonoBehaviourPunCallbacks
{
    protected float moveSpeed;
    protected float jumpForce;
    protected float i_abilityCooldown, abilityCooldown;
    protected Rigidbody playerRig;
    protected PlayerLoadout theLoadout;
    protected GameObject divisionUI;
    public Division() { }

    public virtual void Init(float _jumpForce, float _abilityCooldown, float _moveSpeed) 
    {
        jumpForce = _jumpForce;
        i_abilityCooldown = _abilityCooldown;
        abilityCooldown = 0f;
        playerRig = GetComponent<Rigidbody>();
        theLoadout = GetComponent<PlayerLoadout>();
        moveSpeed = _moveSpeed;
    }

    public virtual void UseAbility() { }

    public virtual void ResetUI() { }

    public virtual void UpdateDivisionStats() 
    {
        if (abilityCooldown > 0f)
        {
            abilityCooldown -= Time.deltaTime;
            if (abilityCooldown < 0f)
                abilityCooldown = 0f;
            divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = abilityCooldown / i_abilityCooldown;
        }
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

    public virtual void Shoot()
    {
        if (theLoadout.GetWeapon().FireBullet())
        {
            //Debug.LogError("got ammo");
            photonView.RPC("Shoot", RpcTarget.All, theLoadout.GetWeapon().damage, theLoadout.GetWeapon().firerate);
        }
        else
        {
            theLoadout.CheckReload();
        }
    }

    public virtual float GetMoveSpeed()
    {
        return moveSpeed;
    }
}