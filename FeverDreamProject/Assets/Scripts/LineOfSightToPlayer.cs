using UnityEngine;
using UnityEngine.Events;

public class LineOfSightToPlayer : MonoBehaviour
{
    // TODO: Implement
    private Vector3 PlayerPos => Vector3.zero;
    [SerializeField]
    private UnityEvent<bool> onPlayerSightChanged;
    private UnityEvent<bool> OnPlayerSightChanged => onPlayerSightChanged;
    [SerializeField]
    private LayerMask layerMask;
    private LayerMask LayerMask => layerMask;

    private bool IsPlayerVisible { get; set; }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.position - PlayerPos);
        bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask);

        // TODO: Do this properly
        bool isPlayerVisible = isHit && hitInfo.collider.gameObject.name == "Player";
        if (isPlayerVisible != IsPlayerVisible)
        {
            IsPlayerVisible = isPlayerVisible;
            OnPlayerSightChanged?.Invoke(IsPlayerVisible);
        }
    }
}
