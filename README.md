Tomorrowland
==================

EDM Festival Game Project
--------------------------


# 개요
## 구성
### 캐릭터
     > 사람 캐릭터 생성 - Taichi Character Pack Asset으로 결정
     ![A]t text]("Player Character Image.png")

     > 방해꾼 사람 캐릭터 생성

### 스테이지
    목표 설정 (펜스)
    맵 구성(입구, 통로, 중앙분리 펜스 등)
    낮, 노을, 야간 3 stage 구상중

### 장애요소
    방해꾼은 플레이어가 펜스에 쉽게 접근하지 못하도록 방해하는 역할. (미는 등의 물리적 상호작용 예정)
    3종류의 방해꾼 (A - 고정형, B - 배회형, C - 시비형)
        > 고정형 - 방해꾼마다 특정한 위치에 있다가 플레이어로부터 물리적 힘을 받으면 플레이어를 밀기 위해 따라다님.
        > 배회형 - 방해꾼마다 특정 지역을 돌아다니다가 플레이어로부터 물리적 힘을 받으면 플레이어를 밀기 위해 따라다님.
        > 시비형 - 방해꾼마다 특정 지역을 돌아다니다가 플레이어가 시야 안에 들어오면 플레이어를 밀기 위해 따라다님.
          ㄴ 항상 걸어다니는 Animation 재생, 특정 조건 충족시(player가 시야에 들어옴) 때리는 Animation 재생 / Animator Controller를 통해 구현.

### 배경음악
    낮 - big room, 노을 - progresive house, 야간 - hardstyle 예정

### 클리어 조건
    곡 별 drop이 터지는 시간에 목표(펜스)에 도달해 있는지 여부 확인 -> 성공/실패 판단

## 개발
### PlayerMovement 코드 작성
    > player Animation 추가 - walking, punching
    > player rotation 값 추가 및 Animation 적용
    ```
    public class PlayerMovement : MonoBehaviour
{
    Vector3 movement;
    Quaternion rotation = Quaternion.identity;
    Animator animator;
    Rigidbody playerRigidbody;

    // 캐릭터가 1초간 회전하는 각도를 라디안으로 저장.
    public float turnSpeed = 20f;

    // 캐릭터의 이동 속도 설정.
    public float velocity = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnAnimatorMove()
    {
        // 키 입력시 변하는 위치값과 회전값 지정.
        playerRigidbody.MovePosition(playerRigidbody.position + movement * velocity);
        playerRigidbody.MoveRotation(rotation);
    }

    // Update is called once per frame
    void Update()
    {
        // 매 프레임마다 상하/좌우/펀치 입력 여부 확인
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float punching = Input.GetAxis("Jump");

        movement.Set(horizontal * (-1.0f), 0f, vertical * (-1.0f));
        movement.Normalize();

        // 걷는 Animation 재생 위해 상하좌우 입력 여부 확인
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        animator.SetBool("IsWalking", isWalking);

        // 펀칭 Animation 재생 위해  spacebar 입력 여부 확인
        bool isPunching = !Mathf.Approximately(punching, 0f);
        animator.SetBool("IsPunching", isPunching);

        // movement와 현재 GameObject의 transform이 향하고 있는 방향을 가지고 turnSpeed 만큼 회전된 값을 계산
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movement, turnSpeed * Time.deltaTime, 0f);
        rotation = Quaternion.LookRotation(desiredForward);
    }
}
    ```

### 카메라 설정
    > virtual camera로 follow => player 설정 // 이 과정에서 player가 거꾸로 움직여서 코드 수정. vector의 horizontal, vertical 값에 각각 -1 곱함.

### collider 설정
    > player 와 enemy 에게 gravity 부여.
    > ground 및 fense에 collider 설정해서 캐릭터가 돌아다닐 수 있는 지표면과 장애물 설정.

### EnemyAround 코드 작성
    > 
    ```
    public class EnemyAround : MonoBehaviour
    {
    // 이동에 사용할 에이전트
    public NavMeshAgent navMeshAgent;

    // 웨이포인트 (Enemy가 돌아다녀야할 좌표들)
    public Transform[] waypoints;

    int currentWaypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    // Update is called once per frame
    void Update()
    {
        // 매 프레임마다 에이전트가 목적지에 도착했는지 확인
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            // 목적지에 도착! 
            // 다음 목적지에 사용할 index 계산
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            //목적지를 다음 웨이포인트로 이동
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
    }
    ```

### Stage에 nevigation mesh 설정
    > enemy가 해당 구역안에서 돌아다니도록 설정