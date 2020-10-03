

namespace Game.Paint {

  using System.Collections.Generic;
  using UnityEngine;

  public class ConductiveData {

    bool _electric = false;
    protected HashSet<Mesh> sources;

    public bool electric => conductive && _electric;
    public bool conductive => sources != null;


    public bool HasSource(Mesh mesh) {
      if (sources == null) return false;
      return sources.Contains(mesh);
    }

    public void RemoveSource(Mesh mesh) {
      if (sources != null) {
        sources.Remove(mesh);
        if (sources.Count < 0) sources = null;
      }
    }

    public void AddSource(Mesh mesh) {
      if (mesh == null) throw new System.ArgumentNullException(nameof(mesh));
      if (sources == null) sources = new HashSet<Mesh>();
      sources.Add(mesh);
    }

  }

}
