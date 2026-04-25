using UnityEngine;
using UnityEngine.AI;

public class GloinkController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    private NavMeshAgent Agent => agent;

    // TODO: Implement
    private Vector3 PlayerPos => Vector3.zero;
    private bool IsPlayerVisible { get; set; }

    public void SetIsPlayerVisible(bool isPlayerVisible)
    {
        IsPlayerVisible = isPlayerVisible;
    }

    void Start()
    {

    }

    void Update()
    {
        if (!IsPlayerVisible)
        {
            return;
        }

        Agent.destination = PlayerPos;
    }
}
