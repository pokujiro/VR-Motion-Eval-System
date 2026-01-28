using UnityEngine;
using UnityEngine.InputSystem;

public class TrackerDebugPose : MonoBehaviour
{
    [Header("Bind these to devicePose/position and devicePose/rotation")]
    public InputActionProperty positionAction;
    public InputActionProperty rotationAction;

    void OnEnable()
    {
        positionAction.action?.Enable();
        rotationAction.action?.Enable();
    }

    void OnDisable()
    {
        positionAction.action?.Disable();
        rotationAction.action?.Disable();
    }

    void Update()
    {
        if (positionAction.action == null || rotationAction.action == null) return;
        if (positionAction.action.activeControl == null || rotationAction.action.activeControl == null) return;

        Vector3 pos = positionAction.action.ReadValue<Vector3>();
        Quaternion rot = rotationAction.action.ReadValue<Quaternion>();

        transform.SetPositionAndRotation(pos, rot);
    }
}
