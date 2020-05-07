using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
    public int decalLimit = 100;
    public int decalAmount;
    List<GameObject> particles = new List<GameObject>();
    void Start()
    {
        decalLimit -= 1;
        decalAmount = transform.childCount;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    decalAmount = transform.childCount;
    //}

    public void ClearAll() {
        while (particles.Count > 0) {
            GameObject decal = particles[0];
            particles.Remove(decal);
            Destroy(decal);
        }

        decalAmount = 0;
    }

    public void NewDecal(GameObject decal) {
        if (decalLimit == 0) {
            return;
        } else if (decalAmount > decalLimit) {
            var destroyDecal = particles[0];
            particles.Remove(destroyDecal);
            Destroy(destroyDecal);
            decalAmount--;
        }
        particles.Add(decal);
        decalAmount++;
    }
}
