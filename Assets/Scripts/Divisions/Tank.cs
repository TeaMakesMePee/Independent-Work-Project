using UnityEngine;
using System.Collections;
using Photon.Pun;

public class Tank : Division
{
    private bool b_abilityActive;
    private float f_abilityActive;
    private float absorbed;
    private float currentTime;
    private float timeToBurst = 2f;

    public Tank() { }

    public override void Init(float _jumpForce, float _abilityCooldown)
    {
        b_abilityActive = false;
        f_abilityActive = 0f;
        absorbed = 0f;
        currentTime = 0f;
        base.Init(_jumpForce, _abilityCooldown);
    }

    public override void UpdateDivisionStats()
    {
        if (f_abilityActive > 0f)
        {
            f_abilityActive -= Time.deltaTime;
            if (f_abilityActive <= 0f)
            {
                Debug.LogError("Absorbed damage: " + absorbed);
                Debug.LogError("Inactive");
                b_abilityActive = false;
            }
        }
        if (!b_abilityActive && absorbed > 0f)
        {
            if (currentTime <= timeToBurst)
            {
                currentTime += Time.deltaTime;
                absorbed = Mathf.Lerp(0f, absorbed, currentTime / timeToBurst);
                Debug.LogError("Resetting absorbed: " + absorbed);
            }
            else
            {
                Debug.LogError("Hard reset: " + absorbed);
                absorbed = 0f;
                currentTime = 0f;
            }
        }

        if (Input.GetMouseButton(0) && theLoadout.readyFire())
        {
            Shoot();
        }
        base.UpdateDivisionStats();
    }

    public override void UseAbility()
    {
        if (abilityCooldown <= 0f)
        {
            //Do ability
            b_abilityActive = true;
            f_abilityActive = 1.5f;
            currentTime = 0f;
            abilityCooldown = i_abilityCooldown;
            Debug.LogError("Active");
        }
    }

    public override void Jump(bool inAir)
    {
        base.Jump(inAir);
    }

    public override void TakeDamage(float damage)
    {
        if (b_abilityActive)
        {
            absorbed += damage * 0.9f;
            base.TakeDamage(damage * 0.1f); //Takes 10 percent damage when ability active
        }
        else
        {
            base.TakeDamage(damage);
        }
    }
    public override void Shoot()
    {
        if (theLoadout.GetWeapon().FireBullet())
        {
            photonView.RPC("Shoot", RpcTarget.All, theLoadout.GetWeapon().damage + absorbed * 0.2f); 
        }
        else
        {
            theLoadout.CheckReload();
        }
    }
}
