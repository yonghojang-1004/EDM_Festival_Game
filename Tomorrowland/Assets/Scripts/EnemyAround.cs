using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAround : MonoBehaviour
{
    // 이동에 사용할 에이전트
    public NavMeshAgent navMeshAgent;

    // 웨이포인트 (Enemy가 돌아다녀야할 좌표들)
    public Transform[] waypoints;

    int currentWaypointIndex;

    float checkTimer = 0f;

    // player와 충돌하면 잠시 naviation을 멈추고, 튕겨져나가도록 설정. 그 후 다시 navigation 재개
    PlayerMovement playermovement;
    private void OnCollisionEnter(Collision other)
    {

        if (other.transform.tag == "Player")
        {
            navMeshAgent.Stop();
            other.rigidbody.AddForce(Vector3.forward * 1000);

            while(checkTimer < 10f)
            {
                checkTimer += Time.deltaTime;
            }
            checkTimer = 0f;
            navMeshAgent.Resume();
        }
    }

    // enemy의 시야에 player가 들어왔을 경우 player를 쫓도록 설정.
    bool isPlayerDiscovered;
    public void DiscoverPlayer()
    {
        isPlayerDiscovered = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.SetDestination(waypoints[1].position);
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerDiscovered) // enemy 시야에 player가 한번이라도 들어왔다면 player를 쫓아 navigation 하도록 함.
        {
            navMeshAgent.SetDestination(waypoints[0].position);
        }
        // 매 프레임마다 에이전트가 목적지에 도착했는지 확인
        else if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
           // 목적지에 도착! 
           // 다음 목적지에 사용할 index 계산
           if((currentWaypointIndex+1)%waypoints.Length == 0)
            {
                currentWaypointIndex++;
            }

           currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
           //목적지를 다음 웨이포인트로 이동
           navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);    
        }
    }
}
