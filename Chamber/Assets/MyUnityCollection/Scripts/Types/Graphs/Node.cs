

namespace Muc.Geometry {

  using System.Linq;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  [System.Serializable]
  public class Node : INode<Node> {

    [field: SerializeField]
    public Vector3 position { get; set; }

    [field: SerializeField]
    public ICollection<Node> links { get; } = new HashSet<Node>();

    public void AddLinks(IEnumerable<Node> fromNodes) { foreach (var node in links) AddLinks(node); }
    public void AddLinks(Node fromNode) {
      if (fromNode == this || fromNode.links.Contains(this)) return;
      links.Add(fromNode);
      fromNode.links.Add(this);
    }

    public void RemoveLinks(IEnumerable<Node> fromNodes) { foreach (var node in links) RemoveLinks(node); }
    public void RemoveLinks(Node fromNode) {
      if (fromNode == this || !fromNode.links.Contains(this)) return;
      links.Remove(fromNode);
      fromNode.links.Remove(this);
    }


    public void ClearLinks() {
      // Convert to array so we dont modify the Enumerable during enumeration
      foreach (var node in links.ToArray()) {
        links.Remove(node);
        node.links.Remove(this);
      }
    }

  }

}