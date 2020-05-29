using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 movement;
    Quaternion rotation = Quaternion.identity;
    Animator animator;
    Rigidbody playerRigidbody;

    // 캐릭터가 1초간 회전하는 각도를 라디안으로 저장.
    public float turnSpeed = 20f;

    // 캐릭터의 이동 속도 설정.
    public float velocity = 0.01f;

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

        // movement와 현재 GameObject의 transform이 향하고 있는 방향을 가지고 turnSpeed 만큼 회전된 값을 계산
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movement, turnSpeed * Time.deltaTime, 0f);
        rotation = Quaternion.LookRotation(desiredForward);
    }
}
