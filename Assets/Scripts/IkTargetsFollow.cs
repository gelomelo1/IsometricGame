using UnityEngine;

public class IkTargetsFollow : MonoBehaviour
{
    public Transform LeftHandTarget;
    public Transform LeftHandHint;
    public Transform RightHandTarget;
    public Transform RightHandHint;

    public Transform LeftHandTargetPoint;
    public Transform LeftHandHintPoint;
    public Transform RightHandTargetPoint;
    public Transform RightHandHintPoint;

    void Update()
    {
        Follow(LeftHandTarget, LeftHandTargetPoint);
        Follow(LeftHandHint, LeftHandHintPoint);

        Follow(RightHandTarget, RightHandTargetPoint);
        Follow(RightHandHint, RightHandHintPoint);
    }

    private void Follow(Transform target, Transform point)
    {
        if (target == null || point == null) return;

        target.position = point.position;
        target.rotation = point.rotation;
    }
}
