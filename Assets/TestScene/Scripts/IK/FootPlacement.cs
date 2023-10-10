using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class FootPlacement : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] FootPlacement otherLeg;
    [SerializeField] Transform knee;
    [SerializeField] float speed;
    [SerializeField] float stepDistance;
    [SerializeField] float stepLength;
    [SerializeField] float stepHeight;
    [SerializeField] float maxStepHeight;
    [SerializeField] Vector3 footOfset;
    public float sinDeneme;

    float footSpacing;
    Vector3 oldPos, curPos, newPos;
    Vector3 oldNormal, curNormal, newNormal;

    float lerp;
    void Start()
    {
        lerp = 1;
        footSpacing = transform.localPosition.x;
        curPos = oldPos = newPos = transform.position;
        curNormal = oldNormal = newNormal = transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = curPos;
        transform.up= curNormal;
        //if (newPos.y>curPos.y)
        //{
        //    curPos.y= newPos.y;
        //}
        //print("Gameobject= " + this.name + " Posses = " + curPos.y + " new " + newPos.y);


        Ray ray = new(knee.position + (knee.right * footSpacing), Vector3.down);

        Debug.DrawRay(knee.position + (knee.right * footSpacing), Vector3.down, Color.black);

        if (Physics.Raycast(ray,out RaycastHit hit,10,groundLayer.value,QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Distance(newPos, hit.point) > stepDistance && !otherLeg.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = knee.InverseTransformPoint(hit.point).z > knee.InverseTransformPoint(newPos).z ?
                    1 : -1;

                newPos = hit.point + (direction * stepLength * knee.forward) + footOfset;
                newNormal = hit.normal;
            }
        }

        if (lerp < 1 )
        {
            Vector3 tempPos = Vector3.Lerp(oldPos,newPos,lerp);
            tempPos.y = Mathf.Sin(Mathf.PI * lerp) + stepHeight;
            //tempPos.y= Mathf.Min(Mathf.Sin(Mathf.PI * lerp) * stepHeight,maxStepHeight);
            
            curPos = tempPos;
            curNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPos = newPos;
            oldNormal = newNormal;
        }
        
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPos, .1f);

    }
}
