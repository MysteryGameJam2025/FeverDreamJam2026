using UnityEngine;
using UnityEngine.AI;

public class GloinkController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    private NavMeshAgent Agent => agent;
    [SerializeField]
    private GloinkShooting shooting;
    private GloinkShooting Shooting => shooting;
    [SerializeField]
    private float distanceToStayFromPlayer;
    private float DistanceToStayFromPlayer => distanceToStayFromPlayer;

    // TODO: Implement
    private Vector3 PlayerPos => FakePlayer.Instance.transform.position;
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

        UpdateDestination();
        Shooting.Shoot(PlayerPos - transform.position);
    }

    void UpdateDestination()
    {
        if (Vector3.Distance(PlayerPos, transform.position) <= DistanceToStayFromPlayer)
        {
            Agent.destination = transform.position;
            return;
        }
        Agent.destination = PlayerPos;
    }
}
