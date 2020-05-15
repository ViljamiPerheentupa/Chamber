using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RestoreGroup : MonoBehaviour {

    public List<IRestorable> restorables;
    public List<Transform> transforms;
    public Vector3 size;
    private void Awake() {
        restorables = new List<IRestorable>();
        var allRestorables = FindObjectsOfType<MonoBehaviour>().OfType<IRestorable>();
        var localBounds = new Bounds(Vector3.zero, size);
        foreach(var r in allRestorables) {
            var mb = (MonoBehaviour)r;
            var pos = mb.transform.position;
            var localPos = transform.InverseTransformPoint(pos);
            if(localBounds.Contains(localPos)) {
                print(r);
                restorables.Add(r);
                transforms.Add(mb.transform);
            }
        }
        print(restorables.Count);
    }

    public void RestoreObjects() {
        foreach(var restorable in restorables) {
            restorable.Restore();
        }
        print("Restoring complete");
    }

    void OnDrawGizmosSelected() {
        //Gizmos.matrix = Matrix4x4.identity;
        //Gizmos.DrawWireCube(Vector3.zero, size);
        //foreach(var item in transforms) {
        //    Gizmos.DrawWireSphere(transform.InverseTransformPoint(item.position), 0.1f);
        //}
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
