using UnityEngine;
using UnityEngine.Events;

public class LineOfSightToPlayer : MonoBehaviour
{
    // TODO: Implement
    private Vector3 PlayerPos => FakePlayer.Instance.transform.position;
    [SerializeField]
    private UnityEvent<bool> onPlayerSightChanged;
    private UnityEvent<bool> OnPlayerSightChanged => onPlayerSightChanged;
    [SerializeField]
    private LayerMask layerMask;
    private LayerMask LayerMask => layerMask;
    [SerializeField]
    private float maxDetectionDist;
    private float MaxDetectionDist => maxDetectionDist;

    private bool IsPlayerVisible { get; set; }

    void Update()
    {
        if (Vector3.Distance(transform.position, PlayerPos) > MaxDetectionDist)
        {
            return;
        }

        Ray ray = new Ray(transform.position, PlayerPos - transform.position);
        Debug.DrawRay(ray.origin, ray.direction);
        bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, MaxDetectionDist, LayerMask);

        // TODO: Do this properly
        bool isPlayerVisible = isHit && hitInfo.collider.gameObject.layer == 7;
        if (isPlayerVisible != IsPlayerVisible)
        {
            IsPlayerVisible = isPlayerVisible;
            OnPlayerSightChanged?.Invoke(IsPlayerVisible);
        }
    }
}
