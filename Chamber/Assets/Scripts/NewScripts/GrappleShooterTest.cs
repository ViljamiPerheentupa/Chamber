using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleShooterTest : MonoBehaviour
{
    public LayerMask layerMask;
    RaycastHit hit;
    Image crosshair;

    public float magnetSpeed = 0.5f;

    //public float maxGrabMagnitude = 10f;
    //public float grabDuration = 1f;
    //float grabTimer = 0;

    Collider grabbedObject;
    Vector3 magnetPoint;
    Rigidbody grabbedRig;
    //Vector3 grabDirection;
    //Vector3 pullDirection;
    //Vector3 pushDirection;

    Vector3 grapplePoint;
    public float grappleSpeed = 2f;

    public Color highlightColor;
    public Color grabColor;
    public Color grappleColor;
    Color normalColor;

    bool grabbed = false;
    bool magnetized = false;
    bool grappling = false;

    void Start()
    {
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        normalColor = crosshair.color;
        //grabDirection = Vector3.zero;
        //pullDirection = Vector3.zero;
        //pushDirection = Vector3.zero;
        grapplePoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbed) {
            Highlight();
        }
        if (!grappling) {
            if (Input.GetButtonDown("Fire1") && !grabbed) {
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask)) {
                    if (hit.collider.tag == "GrappleSpot") {
                        grapplePoint = hit.collider.transform.position;
                        grappling = true;
                    } else {
                        grabbedObject = hit.collider;
                        if (grabbedObject.GetComponent<IGrapple>() != null) {
                            //pushDirection = (grabbedObject.transform.position - transform.position) / 2;
                            //pushDirection = pushDirection.normalized * maxGrabMagnitude / 4;
                            grabbedRig = grabbedObject.gameObject.GetComponent<Rigidbody>();
                            grabbed = true;
                        } else grabbedObject = null;
                    }
                }
                return;
            }
            if (Input.GetButtonDown("Fire1") && grabbed) {
                Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask);
                magnetPoint = hit.point;
                magnetized = true;
                grabbed = false;
                //pushDirection *= 2;
                //LetGo();
                return;
            }
            if (Input.GetButtonDown("Fire1") && magnetized) {
                print("Manually let go");
                StopMagnet();
            }
            //if (Input.GetButtonDown("Fire2") && grabbed) {
            //    print("Pulled towards the player");
            //    pushDirection = Vector3.zero;
            //    pullDirection = transform.position - grabbedObject.transform.position;
            //    LetGo();
            //}
        }
    }

    private void LateUpdate() {
        if (magnetized) {
            Magnetize();
        }
        if (grappling) {
            Grapple();
            transform.parent.GetComponent<SimpleGravity>().grappling = true;
        } else transform.parent.GetComponent<SimpleGravity>().grappling = false;
    }

    void Grapple() {
        transform.parent.position = Vector3.LerpUnclamped(transform.parent.position, grapplePoint, grappleSpeed);
        if (Vector3.Distance(transform.parent.position, grapplePoint) < 0.2f) {
            transform.parent.position = grapplePoint + ((transform.parent.position - grapplePoint) * 0.1f);
            grappling = false;
            grapplePoint = Vector3.zero;
        }
    }

    void Magnetize() {
        grabbedRig.useGravity = false;
        grabbedRig.position = Vector3.LerpUnclamped(grabbedRig.position, magnetPoint, magnetSpeed);
        if (Vector3.Distance(grabbedRig.position, magnetPoint) < grabbedObject.bounds.size.y) {
            StopMagnet();
        }
    }

    void StopMagnet() {
        grabbedRig.useGravity = true;
        grabbedRig = null;
        grabbedObject = null;
        magnetized = false;
        grabbed = false;
    }

    //void Grab() {
    //    GrabDuration();
    //    crosshair.color = grabColor;
    //    grabDirection += new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0);
    //    if (grabDirection.magnitude >= maxGrabMagnitude) {
    //        grabDirection = Vector3.ClampMagnitude(grabDirection, maxGrabMagnitude);
    //        print("Grab exceeded maximum magnitude, letting go");
    //        LetGo();
    //    }
    //}

    //void GrabDuration() {
    //    grabTimer += Time.deltaTime;
    //    if (grabTimer >= grabDuration) {
    //        LetGo();
    //        return;
    //    }
    //}

    //void LetGo() {
    //    grabDirection = transform.TransformDirection(grabDirection);
    //    pullDirection = Vector3.ClampMagnitude(pullDirection, maxGrabMagnitude / 2);
    //    grabDirection += pullDirection;
    //    pushDirection = Vector3.ClampMagnitude(pushDirection, maxGrabMagnitude / 2);
    //    grabDirection += pushDirection;
    //    grabTimer = 0;
    //    grabbed = false;
    //    grabbedObject.GetComponent<IGrapple>().Grab(grabDirection);
    //    grabbedObject = null;
    //    grabDirection = Vector3.zero;
    //    pullDirection = Vector3.zero;
    //    pushDirection = Vector3.zero;
    //}

    void Highlight() {
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask)) {
            if (hit.collider.GetComponent<IGrapple>() != null) {
                crosshair.color = highlightColor;
            } else if (hit.collider.tag == "GrappleSpot") crosshair.color = grappleColor;
            else crosshair.color = normalColor;
        } else crosshair.color = normalColor;
    }
}
