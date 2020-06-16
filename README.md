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
    public float velocity = 0.03f;

    // enemy 와 충돌했을 때, 튕겨져 나가는 힘이 발생하도록 설정.
    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Enemy")
        {
            other.rigidbody.AddForce(Vector3.forward * 1000);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnAnimatorMove()
    {
        // 키 입력시 변하는 위치값과 회전값 지정.
        playerRigidbody.MovePosition(playerRigidbody.position + movement * velocity * Time.deltaTime);
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
    > 돌아다니다가 부딪힌 경우에만 Player을 쫓는 Enemy도 동일한 코드를 활용할 수 있도록 수정. -> isCrashed가 true가 되었을 경우에 Player을 추적할 수 있도록.
    ```
  public class EnemyAround : MonoBehaviour
{
    // 이동에 사용할 에이전트
    public NavMeshAgent navMeshAgent;

    // 웨이포인트 (Enemy가 돌아다녀야할 좌표들)
    public Transform[] waypoints;

    int currentWaypointIndex;

    float checkTimer = 0f;
    bool isCrashed = false;

    // player와 충돌하면 잠시 naviation을 멈추고, 튕겨져나가도록 설정. 그 후 다시 navigation 재개
    PlayerMovement playermovement;
    private void OnCollisionEnter(Collision other)
    {

        if (other.transform.tag == "Player")
        {
            navMeshAgent.Stop();
            other.rigidbody.AddForce(Vector3.forward * 1000);

            while(checkTimer < 50000f)
            {
                checkTimer += Time.deltaTime;
            }
            checkTimer = 0f;
            isCrashed = true;
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
        if(isPlayerDiscovered || isCrashed) // enemy 시야에 player가 한번이라도 들어왔거나 부딪혔다면 player를 쫓아 navigation 하도록 함.
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


}
    ```

### Stage에 nevigation mesh 설정
    > enemy가 해당 구역안에서 돌아다니도록 설정 // 돌아다니다가 시야에 Player가 들어오면 추적하는 적 13명 구현 +} 돌아다니다가 Player와 부딪히면 추적하는 적 구현


### ViewRange 코드 작성
    > player가 enemy 시야 안에 들어오면 기존에 진행하던 navigation  대신에 player를 쫓아다니도록 함.
    ```
    public class ViewRange : MonoBehaviour
{
    bool isPlayerInRange = false;
    public Transform player;
    public EnemyAround enemyAround;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == player)
        {
            isPlayerInRange = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange == true)
        {
            enemyAround.DiscoverPlayer(); 
        }
    }
}
```

### GameEnding 코드 작성
    > Player가 특정시간(특정 음악 Drop 시점)에 Goal에 도달했는지를 확인하여 Stage를 종료시키는 코드
    > 제한시간안에 Goal에 도달하지 못하면  Fail 화면 출력 후 현재 Scene을 Relaod 
    > Stage 1 BGM 추가 : Big Room (Mix Set 일부 발췌)
    > Clear 할 경우 Drop 듣고 다음 스테이지로 이동할 수 있도록 코드 작성.
    ```

public class GameEnding : MonoBehaviour
{
    bool isPlayerAtGoal = false;
    public Transform player;
    public Text timeText;
    private float time;
    bool isCleared = false;
    float endTimer = 0f;
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public int stageIndex;

    public CanvasGroup FailBackgroundImageCanvasGroup;
    public CanvasGroup ClearBackgroundImageCanvasGroup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            isPlayerAtGoal = true; // player가 Goal에 도착해있다는 것을 인지
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            isPlayerAtGoal = false; // player가 Goal에서 벗어났다는 것을 인지
        }
    }

    void EndLevel(CanvasGroup imageCanvasGroup,bool goNext)
    {
        endTimer = endTimer + Time.deltaTime;
        imageCanvasGroup.alpha = endTimer / fadeDuration;

        // 타이머가 1초(fadeDuration) + displayImageDuration이 지나면 Scene 변경
        if(endTimer > fadeDuration + displayImageDuration)
        {
            if(goNext)
            {
                SceneManager.LoadScene(stageIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
}

    // Update is called once per frame
    void Update()
    {
        // 흘러가는 시간을 계산
        time += Time.deltaTime;
        timeText.text = "시간 : " + Mathf.Round(time);

        if(true) // 시간요소 추가 예정
        {
            if (time > 113f && time < 116f) // Player가 30~32초에 Goal에 도달했으면 Clear
            {
                if(isPlayerAtGoal)
                {
                    print("clear"); // 다음 씬으로 넘어가도록 코딩 예정
                    isCleared = true;
                }
            }
            else if(time > 117f && isCleared == false)
            {
                EndLevel(FailBackgroundImageCanvasGroup, isCleared); // 제한 시간안에 Goal에 도달하지 못하면 현재 Stage 재시작.
            }


            if(isCleared)
            {
                if(time > 130f)
                {
                    EndLevel(ClearBackgroundImageCanvasGroup, isCleared);
                }
            }
        }
    }
}





    ```