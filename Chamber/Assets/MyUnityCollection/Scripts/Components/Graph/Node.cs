﻿

namespace Muc.Components {

  using System.Collections.Generic;
  using System.Linq;

  using UnityEngine;

  using Muc.Types.Extensions;
  using Muc.Geometry;
  using System;

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


    #region Search

    public enum SearchType {
      DepthFirst,
      BreadthFirst,
    }


    public Node DepthFirstSearch(Predicate<Node> predicate, HashSet<Node> visited)
      => _DepthFirstSearch(predicate, visited, this);

    public Node DepthFirstSearch(Predicate<Node> predicate)
      => _DepthFirstSearch(predicate, new HashSet<Node>(), this);

    private static Node _DepthFirstSearch(in Predicate<Node> predicate, in HashSet<Node> visited, in Node node) {

      visited.Add(node);
      if (predicate(node))
        return node;

      foreach (var link in node.links) {
        if (visited.Add(link))
          return _DepthFirstSearch(predicate, visited, link);
      }

      return null;
    }


    public Node BreadthFirstSearch(Predicate<Node> predicate, HashSet<Node> visited)
      => _BreadthFirstSearch(this, predicate, visited);

    public Node BreadthFirstSearch(Predicate<Node> predicate)
      => _BreadthFirstSearch(this, predicate, new HashSet<Node>());

    private static Node _BreadthFirstSearch(Node node, Predicate<Node> predicate, HashSet<Node> visited) {

      var queue = new Queue<Node>();

      queue.Enqueue(node);
      visited.Add(node);

      while (queue.Count > 0) {
        var current = queue.Dequeue();

        if (predicate(current))
          return current;

        foreach (var future in current.links) {
          if (visited.Add(future))
            queue.Enqueue(future);
        }
      }

      return null;
    }

    #endregion
  }

}