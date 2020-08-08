﻿using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

/*
 * This is the damage division child class
 * Certain parent (division.cs) functions here are overriden to fit the design of this division
 * Shoot is overriden because the damage division increases fire rate when ability is active
*/

public class Damage : Division
{
    private float f_abilityActive;
    public Damage() { }

    public override void Init(float _jumpForce, float _abilityCooldown, float _moveSpeed)
    {
        divisionUI = GameObject.Find("DamageUI");
        base.Init(_jumpForce, _abilityCooldown, _moveSpeed);
        ResetUI();
    }

    public override void UpdateDivisionStats()
    {
        if (Input.GetMouseButton(0) && theLoadout.readyFire())
        {
            Shoot();
        }
        if (f_abilityActive > 0f)
        {
            f_abilityActive -= Time.deltaTime;
            if (f_abilityActive <= 0f)
            {
                f_abilityActive = 0f;
                abilityCooldown = i_abilityCooldown;
            }
            divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = (1f - f_abilityActive / 1.5f);
        }
        base.UpdateDivisionStats();
    }

    public override void UseAbility()
    {
        if (abilityCooldown <= 0f)
        {
            f_abilityActive = 1.5f;
        }
    }

    public override void ResetUI()
    {
        divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = 0f;
    }

    public override void Jump(bool inAir)
    {
        base.Jump(inAir);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void Shoot()
    {
        if (theLoadout.GetWeapon().FireBullet())
        {
            photonView.RPC("Shoot", RpcTarget.All, theLoadout.GetWeapon().damage, ((f_abilityActive > 0f) ? theLoadout.GetWeapon().firerate * 0.75f : theLoadout.GetWeapon().firerate));
        }
        else
        {
            theLoadout.CheckReload();
        }
    }
}
