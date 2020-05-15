using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, IRestorable
{
    public bool destroyed;

    #region RestoreData
    Vector3 position;
    bool locked;
    bool destroyedInitial;
    #endregion

    private void Awake() {
        position = transform.position;
        destroyedInitial = destroyed;
    }
    public void Restore() {
        if (locked) return;
        destroyed = destroyedInitial;
        transform.position = position;
    }
    public void Lock() {
        locked = true;
    }
}
