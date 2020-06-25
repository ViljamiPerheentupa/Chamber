using UnityEngine;

[CreateAssetMenu(fileName = "DecalData", menuName = "Chamber/DecalData", order = 0)]
public class DecalData : ScriptableObject {
    public Vector3 decalSize;
    public Material decalMaterial;
}