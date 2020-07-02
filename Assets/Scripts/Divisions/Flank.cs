using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Flank : Division
{
    private bool isDoubleJump;
    private bool isDashing;
    private float c_dashTime, t_dashTime;
    public Flank() { }

    public override void Init(float _jumpForce, float _abilityCooldown, float _moveSpeed)
    {
        isDoubleJump = isDashing = false;
        t_dashTime = 0.1f;
        divisionUI = GameObject.Find("FlankUI");
        base.Init(_jumpForce, _abilityCooldown, _moveSpeed);
        ResetUI();
    }

    public override void UpdateDivisionStats()
    {
        if (Input.GetMouseButton(0) && theLoadout.readyFire())
        {
            base.Shoot();
        }
        if (isDashing)
        {
            c_dashTime += Time.deltaTime;
            if (c_dashTime < t_dashTime)
            {
                Vector3 dir = transform.forward;
                dir.y = 0f;
                playerRig.AddForce(dir * 50f, ForceMode.VelocityChange);
            }
            else
            {
                isDashing = false;
            }
        }
        base.UpdateDivisionStats();
    }

    public override void UseAbility()
    {
        if (abilityCooldown <= 0f)
        {
            abilityCooldown = i_abilityCooldown;
            isDashing = true;
            c_dashTime = 0f;
        }
    }

    public override void ResetUI()
    {
        divisionUI.transform.Find("AbilityDisabled").GetComponent<Image>().fillAmount = 0f;
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
