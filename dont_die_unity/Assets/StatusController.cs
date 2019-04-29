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
    PlayerController pc;

    private void Awake()
    {
        if(GetComponent<PlayerController>()!=null)
            pc = GetComponent<PlayerController>();
    }

    public void OnStatusHeal(int ticks, float amount, bool isReset)
    {
        StatusEffect status = new StatusEffect(Effects.Type.Heal, ticks, amount, pc);
        statusEffects.Add(status);
    }
    public void OnStatusDamage(int ticks, float amount, bool isReset)
    {
        StatusEffect status = new StatusEffect(Effects.Type.Damage, ticks, amount, pc);
        statusEffects.Add(status);
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
            for (int i = statusEffects.Count; i > 0; i--)
            {
                if(statusEffects[i]!=null)
                {
                    if (statusEffects[i].myTicks > 0)
                    {
                        statusEffects[i].DoEffect();
                        statusEffects[i].myTicks -= 1;
                    }

                    else if (statusEffects[i].myTicks <= 0)
                    {
                        statusEffects.RemoveAt(i);
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

    public StatusEffect(Effects.Type _myType, int _ticks, float _power, PlayerController _pc)
    {
        pc = _pc;

        myType = _myType;
        myPower = _power;
        myTicks = _ticks;
    }

    public void DoEffect()
    {
        switch(myType)
        {
            case Effects.Type.Heal:
                pc.Hurt(-myPower);
                break;

            case Effects.Type.Damage:
                pc.Hurt(myPower);
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
