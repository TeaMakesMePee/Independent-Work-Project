using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

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
            divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = (1f - f_abilityActive / 1.5f);
            //Debug.LogError((1f - f_abilityActive / 1.5f));
        }

        #region old code 1
        //if (/*!b_abilityActive && */absorbed > 0f) //if anyone shot while ability was active
        //{
        //    //if (divisionUI.transform.Find("Ability").gameObject.activeSelf) //Set green absorbed icon to true and off the purple one
        //    //{
        //    //    divisionUI.transform.Find("Ability").gameObject.SetActive(false);
        //    //    divisionUI.transform.Find("AfterAbility").gameObject.SetActive(true);
        //    //}

        //    if (currentTime <= timeToBurst) //2 seconds to reduce absorbed to 0
        //    {
        //        currentTime += Time.deltaTime;
        //        absorbed = Mathf.Lerp(0f, absorbed, currentTime / timeToBurst);
        //        Debug.LogError("Resetting absorbed: " + absorbed);
        //    }
        //    else //after that hard reset absorbed and currentTime
        //    {
        //        Debug.LogError("Hard reset: " + absorbed);
        //        absorbed = 0f;
        //        currentTime = 0f;
        //    }
        //    //divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = currentTime / timeToBurst;
        //}
        #endregion
        absorbed = Mathf.Lerp(0f, absorbed, Time.deltaTime * 3f);
        if (absorbed * 0.2f > 1f)
        {
            if (!divisionUI.transform.Find("AfterAbility").gameObject.activeSelf)
            {
                divisionUI.transform.Find("AfterAbility").gameObject.SetActive(true);
                divisionUI.transform.Find("Ability").gameObject.SetActive(false);
            }
        }
        else
        {
            if (!divisionUI.transform.Find("Ability").gameObject.activeSelf)
            {
                divisionUI.transform.Find("Ability").gameObject.SetActive(true);
                divisionUI.transform.Find("AfterAbility").gameObject.SetActive(false);
            }
        }

        if (Input.GetMouseButton(0) && theLoadout.readyFire())
        {
            Shoot();
        }

        #region old code 2
        //if (abilityCooldown > 0f)
        //{
        //    if (absorbed == 0f && f_abilityActive == 0f) //if anyone didnt shoot while ability was active
        //    {
        //        if (divisionUI.transform.Find("AfterAbility").gameObject.activeSelf) //Set green absorbed icon to true and off the purple one
        //        {
        //            divisionUI.transform.Find("AfterAbility").gameObject.SetActive(false);
        //            divisionUI.transform.Find("Ability").gameObject.SetActive(true);
        //        }
        //        float cdTime = b_absorbed ? (i_abilityCooldown - timeToBurst - 1.5f) : (i_abilityCooldown - 1.5f);
        //        if (abilityCooldown < 0f)
        //        {
        //            b_absorbed = false;
        //            abilityCooldown = 0f;
        //        }
        //        divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = abilityCooldown / cdTime;
        //    }
        //    abilityCooldown -= Time.deltaTime;
        //}
        #endregion
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
