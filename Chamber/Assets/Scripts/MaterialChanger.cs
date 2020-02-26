using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Material changeMaterial;
    public Material originalMaterial;
    MeshRenderer mat;
    void Start()
    {
        mat = GetComponent<MeshRenderer>();
        originalMaterial = mat.material;
    }

    public void ChangeMaterial() {
        mat.material = changeMaterial;
    }

    public void ReturnMaterial() {
        mat.material = originalMaterial;
    }
}
