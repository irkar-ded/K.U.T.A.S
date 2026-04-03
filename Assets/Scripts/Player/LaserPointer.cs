using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [SerializeField] LayerMask layerCanLaser;
    [SerializeField] Transform laserStartPoint;
    [SerializeField]Transform currentTarget;
    [HideInInspector]public Vector3 finalPos;
    Gun gun;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        gun = GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameIsStarted == false || GameManager.instance.endGameState ||  Pause.isPaused == true || gun == null)
        {
            if(lineRenderer.enabled == true)
                lineRenderer.enabled = false;
            return;
        }
        else if(lineRenderer.enabled == false)
        {
            SetLaserPointerPosition();
            lineRenderer.enabled = true;
        }
        SetLaserPointerPosition();
    }
    void SetLaserPointerPosition()
    {
        lineRenderer.SetPosition(0,laserStartPoint.position);
        Vector3 targetPos = gun.getTargetLook();
        Vector3 dirRay = (gun == null ? Vector3.zero : targetPos) - transform.position;
        dirRay.y = 0;
        if(Physics.SphereCast(transform.position,1,dirRay.normalized,out RaycastHit hit, dirRay.magnitude, layerCanLaser) && hit.transform.tag != "TeleportBullet")
        {
            finalPos = hit.point + dirRay.normalized;
            if(currentTarget != hit.transform)
            {
                if(currentTarget != null)
                    LeaveInteract();
                currentTarget = hit.transform;
                currentTarget.SendMessage("TurnSelectedOutlineColor",SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            targetPos.y = transform.position.y;
            finalPos = targetPos + dirRay.normalized;
            LeaveInteract();
        }
        lineRenderer.SetPosition(1, finalPos);
    }
    public void LeaveInteract()
    {
        if(currentTarget != null)
        {
            currentTarget.gameObject.SendMessage("TurnDefualtOutlineColor",SendMessageOptions.DontRequireReceiver);
            currentTarget = null;
        }
    }
}

