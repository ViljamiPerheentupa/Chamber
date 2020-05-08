using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalManager : MonoBehaviour {
    public int decalLimit = 100;
    public GameObject decalTemplate;
    List<GameObject> decals = new List<GameObject>();
    public int decalIterator = 0;

    void Start() {
        Resize(decalLimit);
    }

    public void Resize(int newLimit) {
        decalLimit = newLimit;
        if (newLimit > decals.Count) {
            while (newLimit - decals.Count > 0) {
                GameObject newDecal = Instantiate(decalTemplate, new Vector3(0,0,0), new Quaternion(), transform);
                decals.Add(newDecal);
            }
        }    
        else if (newLimit < decals.Count) {
            while (decals.Count - newLimit > 0) {
                GameObject decal = decals[decals.Count-1];
                Destroy(decal);
                decals.RemoveAt(decals.Count-1);
            }
        }
    }
    
    public void ClearAll() {
        for (int i = 0; i < decals.Count; ++i) {
            decals[i].SetActive(false);
        }

        decalIterator = 0;
    }

    public void NewDecal(Vector3 position, Quaternion rotation, Vector3 size, Material material, Transform parent) {
        if (decalLimit == 0) {
            return;
        }
        
        GameObject d = decals[decalIterator];
        d.transform.position = position;
        d.transform.rotation = rotation;
        d.transform.parent = parent;
        DecalProjector dp = d.GetComponent<DecalProjector>();
        dp.material = material;
        dp.size = size;

        decalIterator = (decalIterator + 1) % decalLimit;
    }
}
