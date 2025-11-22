using UnityEngine;

public class InsectAttackState : InsectBaseState
{
    private GameObject poisonThronPrefab; // Poison Thron 프리팹
    private Transform firePoint;          // 발사 위치
    private float thronSpeed = 10f;       // 발사 속도

    private const float ATTACK_DURATION = 1.5f;
    private const float ATTACK_COOLDOWN = 2f; // 공격 쿨다운 시간
    private float lastAttackTime = 0f;       // 마지막 공격 시간
    private float ATTACK_RANGE => insect.attackRange;  // 공격 사거리
    private float CHASE_RANGE => insect.chaseRange;    // 추적 사거리
    private const float DAMAGE_AMOUNT = 20f;          // 공격 데미지
    private bool hasAttacked = false;

    // 상태 키 설정
    protected override void SetStateKey()
    {
        stateKey = EState.Attack;
    }

    // 공격 상태에 쓰일 필드 초기화
    public void InitializeAttack(GameObject prefab, Transform point, float speed)
    {
        poisonThronPrefab = prefab;
        firePoint = point;
        thronSpeed = speed;
    }

    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Attack");
        hasAttacked = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // 타겟을 향해 회전
        if (insect.player != null)
        {
            Vector3 direction = (insect.player.transform.position - insect.transform.position).normalized;
            direction.y = 0; // Y축 회전은 무시
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                insect.transform.rotation = Quaternion.Slerp(insect.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // 공격 도중에는 상태를 유지
        if (stateTimer < ATTACK_DURATION)
        {
            // 공격이 중간 상태일 때 발사 수행
            if (!hasAttacked && stateTimer >= ATTACK_DURATION * 0.5f)
            {
                FirePoisonThron(); // Poison Thron 발사
                hasAttacked = true;
                lastAttackTime = Time.time; // 마지막 공격 시간 기록
            }
            return;
        }

        // 공격 종료 후 상태 전환 처리
        HandlePostAttack();
    }

    // Poison Thron 발사 로직
    private void FirePoisonThron()
    {
        if (poisonThronPrefab == null || firePoint == null) return;

        Vector3 pos = firePoint.position;

        // firePoint가 아니라 "본체"의 전방을 사용 (XZ 평면으로 납작)
        Vector3 flatForward = Vector3.ProjectOnPlane(insect.transform.forward, Vector3.up).normalized;
        // 몬스터가 완전히 위쪽을 보고 있거나, 거의 위쪽을 보고있을때는 백터값이 아예 없거나 작은 값이 나오기때문에
        // 이럴경우에는 그냥 정면 (0, 0, 1)을 적용
        if (flatForward.sqrMagnitude < 1e-6f) flatForward = Vector3.forward; 

        Quaternion rot = Quaternion.LookRotation(flatForward, Vector3.up);

        // 풀에서 꺼낼 때부터 원하는 회전 전달 + 한 번 더 강제 세팅
        GameObject thron = ProjectilePoolManager.Instance
            .SpawnFromPool(poisonThronPrefab, pos, rot);
        thron.transform.SetPositionAndRotation(pos, rot);

        Rigidbody rb = thron.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity  = false;
            rb.drag = rb.angularDrag = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // 발사전, 이동속도, 회전속도 초기화
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            rb.velocity = flatForward * thronSpeed;
        }

        var thorn = thron.GetComponent<PoisonThorn>();
        if (thorn != null) thorn.Initialize(DAMAGE_AMOUNT);
    }
    
    // 플레이어의 위치 가져오기
    private Vector3 GetPlayerPosition()
    {
        if (insect.player == null) return insect.transform.position;
        return insect.player.transform.position;
    }

    // 공격 상태가 종료되었는지 확인
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Attack;

        // 공격 모션이 끝나지 않았으면 상태 종료 금지
        if (stateTimer < ATTACK_DURATION)
        {
            return false;
        }

        HandlePostAttack(out nextState);
        return true;
    }

    // 공격 후 상태 전환 처리
    private void HandlePostAttack(out EState nextState)
    {
        nextState = EState.Idle;

        if (IsPlayerInAttackRange())
        {
            // 공격 쿨다운이 지났는지 확인
            if (Time.time - lastAttackTime >= ATTACK_COOLDOWN)
            {
                nextState = EState.Attack;
            }
            else
            {
                nextState = EState.Idle; // 쿨다운 중에는 Idle 상태로 전환
            }
        }
        else if (IsPlayerInChaseRange())
        {
            nextState = EState.Chase;
        }
    }

    private void HandlePostAttack()
    {
        EState nextState;
        HandlePostAttack(out nextState);
        if (nextState != stateKey)
        {
            stateTimer = float.MaxValue; // 상태 강제 종료
        }
    }
}