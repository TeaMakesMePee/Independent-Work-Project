using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

/*
 * This is the Tank division child class
 * Certain parent (division.cs) functions here are overriden to fit the design of this division
 * TakeDamage is overriden because the tank division has the ability to absorb damage and convert it into more damage
*/

public class Tank : Division
{
    private bool b_abilityActive;
    private float f_abilityActive;
    private float absorbed;
    private float currentTime;
    private float timeToBurst = 2f;
    private bool b_absorbed;

    public Tank() { }

    public override void Init(float _jumpForce, float _abilityCooldown, float _moveSpeed)
    {
        b_abilityActive = false;
        f_abilityActive = 0f;
        absorbed = 0f;
        currentTime = 0f;
        divisionUI = GameObject.Find("TankUI");
        b_absorbed = false;
        base.Init(_jumpForce, _abilityCooldown, _moveSpeed);
        ResetUI();
    }

    public override void UpdateDivisionStats()
    {
        if (f_abilityActive > 0f)
        {
            f_abilityActive -= Time.deltaTime;
            if (f_abilityActive <= 0f)
            {
                b_abilityActive = false;
                f_abilityActive = 0f;
                abilityCooldown = i_abilityCooldown;
            }
            divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = (1f - f_abilityActive / 3f);
        }

        absorbed = Mathf.Lerp(absorbed, 0f, Time.deltaTime * 0.5f);
        if (absorbed * 0.2f > 1f)
        {
            if (!divisionUI.transform.GetChild(1).gameObject.activeSelf) //checking after ability
            {
                divisionUI.transform.GetChild(1).gameObject.SetActive(true);
                divisionUI.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            if (!divisionUI.transform.GetChild(0).gameObject.activeSelf) //checking curr ability
            {
                divisionUI.transform.GetChild(0).gameObject.SetActive(true);
                divisionUI.transform.GetChild(1).gameObject.SetActive(false);
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
            b_abilityActive = true;
            f_abilityActive = 3f;
            currentTime = 0f;
        }
    }

    public override void ResetUI()
    {
        divisionUI.transform.GetChild(0).gameObject.SetActive(true);
        divisionUI.transform.GetChild(1).gameObject.SetActive(false);
        divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = 0f;
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
            b_absorbed = true;
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
            photonView.RPC("Shoot", RpcTarget.All, theLoadout.GetWeapon().damage + absorbed * 0.2f, theLoadout.GetWeapon().firerate);
        }
        else
        {
            theLoadout.CheckReload();
        }
    }
}
