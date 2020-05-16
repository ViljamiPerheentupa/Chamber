using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropHealth : BaseHealth {
    protected override void Die() {
        gameObject.SetActive(false);
    }

    public override void StartReset() {
        base.StartReset();
        
        gameObject.SetActive(true);
    }
}
