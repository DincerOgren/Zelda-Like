using UnityEngine;

public class FootwithAnims : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform rightKnee;
    [SerializeField] Animator anim;
    [SerializeField] Transform player;
    [SerializeField][Range(0f, 2f)] float DistanceToGround = 1f;
    [SerializeField][Range(-2f, 2f)] float footZOffset = .4f;
    public float raycastDistance = 5f;
    public float ikRightFootWeight = 1f;
    public float ikLeftFootWeight = 1f;

    private Transform right, left;

    // Update is called once per frame
    void Update()
    {
        ikLeftFootWeight = anim.GetFloat("ikLeftFootWeight");
        ikRightFootWeight = anim.GetFloat("ikRightFootWeight");

    }
    //TODO: Prevent Foot's from sliding
    private void OnAnimatorIK(int layerIndex)
    {
        right = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        right.position += transform.forward * footZOffset;
        
        //Debug.DrawRay(right.position, Vector3.down, Color.red);
        Debug.DrawLine(right.position, right.position + Vector3.down * raycastDistance, Color.red);
        Ray ray = new(right.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer.value))
        {
            Vector3 forward = Vector3.ProjectOnPlane(player.forward, hit.normal);
            var rotation = Quaternion.LookRotation(forward, hit.normal);
            Vector3 pos = hit.point;
            pos.y += DistanceToGround;
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikRightFootWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, ikRightFootWeight);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, pos);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rotation);
        }
        left = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        left.position += transform.forward * footZOffset;


        //Debug.DrawRay(left.position, Vector3.down, Color.red);
        Debug.DrawLine(left.position, left.position + Vector3.down * raycastDistance, Color.white);

        Ray ray2 = new(left.position, Vector3.down);

        if (Physics.Raycast(ray2, out RaycastHit hit1, raycastDistance, groundLayer.value))
        {
            Vector3 forward = Vector3.ProjectOnPlane(player.forward, hit1.normal);
            var rotation = Quaternion.LookRotation(forward, hit1.normal);
            Vector3 pos = hit1.point;
            pos.y += DistanceToGround;
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikLeftFootWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ikLeftFootWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, pos);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, rotation);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(right.position, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(left.position, 0.2f);
    }
}
