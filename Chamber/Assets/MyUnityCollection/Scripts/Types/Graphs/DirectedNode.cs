

namespace Muc.Geometry {

  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  [System.Serializable]
  public class DirectedNode : IDirectedNode<DirectedNode> {

    [field: SerializeField]
    public Vector3 position { get; set; }

    [field: SerializeField]
    public List<DirectedNode> inLinks { get; private set; } = new List<DirectedNode>();

    [field: SerializeField]
    public List<DirectedNode> outLinks { get; private set; } = new List<DirectedNode>();

    ICollection<DirectedNode> IDirectedNode<DirectedNode>.inLinks => inLinks;
    ICollection<DirectedNode> IDirectedNode<DirectedNode>.outLinks => outLinks;

    public void AddInbound(IEnumerable<DirectedNode> fromNodes) { foreach (var node in fromNodes) AddInbound(node); }
    public void AddInbound(DirectedNode fromNode) {
      if (fromNode == this || fromNode.outLinks.Contains(this)) return;
      this.inLinks.Add(fromNode);
      fromNode.outLinks.Add(this);
    }
    public void RemoveInbound(IEnumerable<DirectedNode> fromNodes) { foreach (var node in fromNodes) RemoveInbound(node); }
    public void RemoveInbound(DirectedNode fromNode) {
      if (fromNode == this || !fromNode.outLinks.Contains(this)) return;
      this.inLinks.Remove(fromNode);
      fromNode.outLinks.Remove(this);
    }

    public void AddOutbound(IEnumerable<DirectedNode> toNodes) { foreach (var node in toNodes) AddOutbound(node); }
    public void AddOutbound(DirectedNode toNode) {
      if (toNode == this || this.outLinks.Contains(toNode)) return;
      this.outLinks.Add(toNode);
      toNode.inLinks.Add(this);
    }
    public void RemoveOutbound(IEnumerable<DirectedNode> toNodes) { foreach (var node in toNodes) RemoveOutbound(node); }
    public void RemoveOutbound(DirectedNode toNode) {
      if (toNode == this || !this.outLinks.Contains(toNode)) return;
      this.outLinks.Remove(toNode);
      toNode.inLinks.Remove(this);
    }


    public void ClearLinks() {
      // Convert to array so we dont modify the Enumerable during enumeration
      foreach (var inLink in inLinks.ToArray())
        inLink.RemoveOutbound(this);
      foreach (var outLink in outLinks.ToArray())
        outLink.RemoveInbound(this);
    }

  }

}