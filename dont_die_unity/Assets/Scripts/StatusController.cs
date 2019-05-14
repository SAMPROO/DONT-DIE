using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    
    [Header("Time between ticks in seconds")]
    [Range(0.1f,2f)]
    public float tickDelay=0.5f;
    public float currentTickTime;
    private List<StatusEffect> statusEffects = new List<StatusEffect>();
    private float[] statusPower;
    public PlayerController pc;
    public RagdollRig rr;

    private void Awake()
    {
        if(GetComponent<PlayerController>()!=null)
            pc = GetComponent<PlayerController>();

        if (GetComponent<PlayerController>() != null)
            rr = GetComponent<RagdollRig>();
    }
    private void Update()
    {
        ApplyStatus();
    }

    public void ResetStatus()
    {
        // TODO implement
        foreach(StatusEffect effect in statusEffects)
        {
            effect.myTicks = 0;
        }
    }

    public void OnStatusHeal(int ticks, float amount, bool isReset)
    {
        StatusEffect status = new StatusEffect(Effects.Type.Heal, ticks, amount, pc, rr);
        statusEffects.Add(status);
        status.OnApply();
    }
    public void OnStatusDamage(int ticks, float amount, bool isReset)
    {
        StatusEffect status = new StatusEffect(Effects.Type.Damage, ticks, amount, pc, rr);
        statusEffects.Add(status);
        status.OnApply();
    }
    public void OnStatusSlow(int ticks, float amount, bool isReset)
    {

    }
    public void OnStatusStun(int ticks, bool isReset)
    {

    }

    public void ApplyStatus()
    {
        if (currentTickTime < tickDelay)
            currentTickTime += 1 * Time.deltaTime;

        else
        {
            for (int i = statusEffects.Count - 1; i >= 0; i--)
            {
                if(statusEffects[i]!=null)
                {
                    if (statusEffects[i].myTicks <= 0)
                    {
                        statusEffects[i].Release();
                        statusEffects.RemoveAt(i);
                    }

                    else
                    {
                        statusEffects[i].DoEffect();
                        statusEffects[i].myTicks -= 1;
                    }

                    
                }
                
            }
            currentTickTime = 0;
        }
    }
}
public class StatusEffect
{
    private Effects.Type myType;
    public int myTicks;
    public float myPower;
    PlayerController pc;
    RagdollRig rr;

    public StatusEffect(Effects.Type _myType, int _ticks, float _power, PlayerController _pc, RagdollRig _rr)
    {
        pc = _pc;
        rr = _rr;
        myType = _myType;
        myPower = _power;
        myTicks = _ticks;
    }

    public void DoEffect()
    {
        switch(myType)
        {
            case Effects.Type.Heal:
                rr.healVFX.gameObject.SetActive(true);
                rr.healVFX.Play(true);
                pc.Hurt(-myPower);
                break;

            case Effects.Type.Damage:
                rr.damageVFX.gameObject.SetActive(true);
                rr.damageVFX.Play(true);
                pc.Hurt(myPower);
                break;

            case Effects.Type.Slow:

                break;

            case Effects.Type.Stun:

                break;
        }
    }
    public void Release()
    {
        myTicks = 0;
        switch (myType)
        {
            case Effects.Type.Heal:
                rr.healVFX.gameObject.SetActive(false);
                pc.Hurt(-myPower);
                break;

            case Effects.Type.Damage:
                rr.damageVFX.gameObject.SetActive(false);
                pc.Hurt(myPower);
                break;

            case Effects.Type.Slow:

                break;

            case Effects.Type.Stun:

                break;
        }
    }
    public void OnApply()
    {
        Debug.Log("HeyBaby im applying, my type is "+myType);
        switch (myType)
        {
            case Effects.Type.Heal:
                rr.healVFX.gameObject.SetActive(true);
                rr.healVFX.Play(true);
                break;

            case Effects.Type.Damage:
                rr.damageVFX.gameObject.SetActive(true);
                rr.damageVFX.Play(true);
                break;

            case Effects.Type.Slow:

                break;

            case Effects.Type.Stun:

                break;
        }
    }
}
public class Effects
{
    public enum Type { Heal, Damage, Slow, Stun };
}
