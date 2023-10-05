using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField]
    private float followSpeed = 5f;
    private Transform player;
    float mouseX;
    float mouseY;

    #region CurrentWorking
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = -Input.GetAxis("Mouse Y");


        //var ray=Vector3.(cam.transform.position, transform.position, Mathf.Infinity);
        //ray.
        //Vector3 sum = player.position + camOffset;
        //Vector3 destination = Vector3.Lerp(transform.position, sum, followSpeed * Time.deltaTime);
        //transform.position = destination;

        transform.eulerAngles += new Vector3(mouseY, mouseX, 0);
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed);
    }
    #endregion


}
