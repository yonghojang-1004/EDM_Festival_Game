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

### 배경음악
    낮 - big room, 노을 - progresive house, 야간 - hardstyle 예정

### 클리어 조건
    곡 별 drop이 터지는 시간에 목표(펜스)에 도달해 있는지 여부 확인 -> 성공/실패 판단

## 개발
### player Animation 추가 - walking, punching
    > Player Animator 관련 코드 작성
    ```
    public class PlayerMovement : MonoBehaviour
    {
        Vector3 movement;
        Animator animator;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            // 매 프레임마다 상하/좌우/펀치 입력 여부 확인
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            float punching = Input.GetAxis("Jump");

            movement.Set(horizontal, 0f, vertical);
            movement.Normalize();

            // 걷는 Animation 재생 위해 상하좌우 입력 여부 확인
            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            bool isWalking = hasHorizontalInput || hasVerticalInput;
            animator.SetBool("IsWalking", isWalking);

            // 펀칭 Animation 재생 위해  spacebar 입력 여부 확인
            bool isPunching = !Mathf.Approximately(punching, 0f);
            animator.SetBool("IsPunching", isPunching);
        }
    }
    ```
