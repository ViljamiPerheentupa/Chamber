using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropHealth : BaseHealth {
    protected override void Die() {
        Destroy(gameObject);
    }
}
