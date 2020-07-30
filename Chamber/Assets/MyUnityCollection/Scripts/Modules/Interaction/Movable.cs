﻿

namespace Muc.Interaction {

  using System.Linq;
  using System.Collections.Generic;

  using UnityEngine;

  using Muc.Geometry;
  using Muc.Types.Extensions;
  using Muc.Components;


  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(Interactable))]
  public class Movable : MonoBehaviour {

    [Tooltip("Keep distance from the " + nameof(Interactor) + " above this fraction of maximum distance (0 = zero distance)")]
    public float distanceMin = 0;
    [Tooltip("Keep distance from the " + nameof(Interactor) + " below this fraction of maximum distance (1 = maximum interaction distance)")]
    public float distanceMax = 1;

    [Tooltip("When letting go, transfer movement to velocity")]
    public bool transferMovement = true;

    [Tooltip("!!! Move " + nameof(Interactable) + " to the center of the " + nameof(Interactor) + "'s ray")]
    public bool restrictCenter = true;

    [Tooltip("Enable collision when no longer colliding with the " + nameof(Interactor) + " instead of immediately")]
    public bool waitCollisionEnd = true;


    public float baseReturnSpeed = 2f;
    public float returnTimeScale = 5f;


    /// <summary> Whether this interactable is being interacted with </summary>
    public bool interacting { get => interactable.interacting; }

    public Interactable interactable { get; private set; }
    public Interaction interaction { get => interactable.interaction; }
    public new Rigidbody rigidbody { get => rb; }

    private Rigidbody rb;
    private CollisionTracker cc;
    private TransformHistory transHistory;

    private List<Collider> associates = new List<Collider>();

    private float targetDistance;
    private float returnTime;
    private bool returned;
    private bool usedGravity;

    void Start() {
      rb = GetComponent<Rigidbody>();
      interactable = GetComponent<Interactable>();
      interactable.AddActivationEventListeners(OnActivate, OnActive, OnDeactive);

      cc = GetComponent<CollisionTracker>() ?? gameObject.AddComponent<CollisionTracker>();
      transHistory = GetComponent<TransformHistory>() ?? gameObject.AddComponent<TransformHistory>();
      transHistory.SetMinSize(2);
    }

    void FixedUpdate() {
      if (associates.Count > 0) {
        Collider[] cols = rb.GetComponentsInChildren<Collider>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Collider nextCollider in cols) bounds.Encapsulate(nextCollider.bounds);

        var overlaps = Physics.OverlapBox(bounds.center, bounds.extents);

        var unignored = new List<Collider>();
        foreach (var associate in associates) {
          if (!overlaps.Contains(associate)) {
            Physics.IgnoreCollision(rb.GetComponent<Collider>(), associate, false);
            unignored.Add(associate);
          }
        }

        associates.RemoveAll(e => unignored.Contains(e));
      }
    }


    public void OnActivate(Interaction _) {
      usedGravity = rb.useGravity;
      var associate = interaction.source.associatedCollider;
      if (associate) {
        Physics.IgnoreCollision(rb.GetComponent<Collider>(), associate);
        associates.RemoveAll(e => e == associate);
      }
      rb.useGravity = false;
      var maxDif = interaction.dif.SetLen(interaction.source.maxDistance);
      Line line = new Line(
        Vector3.Lerp(interaction.sourcePos, interaction.sourcePos + maxDif, distanceMin),
        Vector3.Lerp(interaction.sourcePos, interaction.sourcePos + maxDif, distanceMax)
      );
      var closestPoint = line.ClampPoint(interaction.targetPos);
      targetDistance = Vector3.Distance(interaction.sourcePos, closestPoint);
    }


    public void OnActive(Interaction _) {
      var targetPos = interaction.sourcePos + (interaction.dif.SetLenSafe(targetDistance).SetDirSafe(interaction.source.transform.forward));
      var dif = targetPos - rb.position;
      var dir = dif.normalized;

      if (cc.colliding) {
        rb.AddForce(dir * interaction.source.prefs.maxForce, ForceMode.Force);
        returned = false;
        returnTime = Time.time;
        // Project velocity towards target if there is no collision
        if (!rb.SweepTest(dir, out var _, dif.magnitude)) {
          rb.velocity = Vector3.Project(rb.velocity, dif);
        }
      } else if (rb.SweepTest(dir, out var _, dif.magnitude)) {
        returned = false;
        returnTime = Time.time;
        rb.AddForce(dir * interaction.source.prefs.maxForce, ForceMode.Force);
      } else {
        // No collision detected
        if (returned) {
          rb.velocity = Vector3.zero;
          interaction.targetPos = targetPos;
        } else {
          float elapsed = (Time.time - returnTime) * returnTimeScale;
          float maxMove = baseReturnSpeed * Time.deltaTime + Mathf.Pow(elapsed, 2);
          interaction.targetPos = Vector3.MoveTowards(interaction.targetPos, targetPos, maxMove);
          if (transform.position == targetPos) returned = true;
        }
      }
    }


    public void OnDeactive(Interaction _) {
      rb.useGravity = usedGravity;

      if (waitCollisionEnd && interaction.source.associatedCollider)
        associates.Add(interaction.source.associatedCollider);

      if (transferMovement) {
        // Transfer relative movement
        var prevDif = transHistory[1].position - interaction.source.transHistory[1].position;
        var vel = (interaction.dif - prevDif) / Time.deltaTime;

        var force = vel * rb.mass;
        force = force.SetLenSafe(Mathf.Min(force.magnitude, interaction.source.prefs.maxForce));
        rb.AddForce(force, ForceMode.Impulse);

        // Transfer interactor movement
        rb.velocity = (interaction.sourcePos - interaction.source.transHistory[1]) / Time.deltaTime;
      } else {
        rb.velocity = Vector3.zero;
      }
    }
  }
}
