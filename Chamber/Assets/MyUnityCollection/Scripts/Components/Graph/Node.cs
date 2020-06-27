

namespace Muc.Components {

  using System.Collections.Generic;
  using System.Linq;

  using UnityEngine;

  using Muc.Types.Extensions;
  using Muc.Geometry;

  public class Node : MonoBehaviour, INode<Node> {

    public Vector3 position { get => transform.position; set => transform.position = value; }

    [field: SerializeField]
    public List<Node> links { get; private set; } = new List<Node>();
    ICollection<Node> INode<Node>.links => links;

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

    void OnDestroy() => ClearLinks();

  }

}