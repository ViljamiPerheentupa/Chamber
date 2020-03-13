using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerDamage {
    void TakeDamage(int damage);
}
public interface IRestorable {
    void Restore();
    void Lock();
}