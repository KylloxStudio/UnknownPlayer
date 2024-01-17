using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    public UnitCode unitCode { get; }
    public int maxHealth { get; set; }
    public int maxStamina { get; set; }
    public float maxSpeed { get; set; }
    public float jumpPower { get; set; }
    public float climbSpeed { get; set; }

    public PlayerStatus()
    {
    }

    public PlayerStatus(UnitCode unitCode, int maxHealth, int maxStamina, float maxSpeed, float jumpPower, float climbSpeed)
    {
        this.unitCode = unitCode;
        this.maxHealth = maxHealth;
        this.maxStamina = maxStamina;
        this.maxSpeed = maxSpeed;
        this.jumpPower = jumpPower;
        this.climbSpeed = climbSpeed;
    }

    public PlayerStatus SetUnitStatus(UnitCode unitCode)
    {
        PlayerStatus status = null;

        switch (unitCode)
        {
            case UnitCode.player1:
                status = new PlayerStatus(unitCode, 1000, 5000, 7.5f, 17.5f, 7.25f);
                break;
            case UnitCode.player2:
                status = new PlayerStatus(unitCode, 750, 3500, 8.75f, 17.5f, 8.25f);
                break;
        }
        return status;
    }
}