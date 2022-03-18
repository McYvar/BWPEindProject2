using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    int healt { get; set; }

    void takeDamage(int amount);

    void setHealth(int amount);
}
