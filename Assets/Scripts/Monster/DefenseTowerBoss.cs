using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTowerBoss : MonoBehaviour
{
     public GameObject particlePrefab; // 파티클 오브젝트 프리팹
    [SerializeField] private string towerName; //타워 이름
    [SerializeField] private float range; //공격 거리
    [SerializeField] private int damage; //데미지
    [SerializeField] private float rateOfAccurasy; //공격 정확도
    [SerializeField] private float rateOfFire; //발사속도
    private float currentRateOfFire; //현재 발사속도
    [SerializeField] private float viewAngle; //�þ߰�
    [SerializeField] private float spinSpeed; //���� ȸ�� �ӵ�.
    [SerializeField] private LayerMask layerMask; //�����̴� ��� Ÿ������ ����(�÷��̾�)
    [SerializeField] private Transform tf_TopGun; //���Ÿ���� ��ž.
    [SerializeField] private ParticleSystem particle_MuzzleFlash; 
    [SerializeField] private GameObject go_HitEffect_Prefab; //���� ����Ʈ.

    private RaycastHit hitInfo; //레이캐스트 맞은 곳 정보 가져오기
    private Animator anim; 
    private AudioSource theAudio;

    private bool isFindTarget = false; //�� Ÿ�� �߽߰� true
    private bool isAttack = false; //�ѱ� ����� �� ������ ��ġ�� �� true


    private Transform tf_Target; //���� ������ Ÿ��

    [SerializeField] private AudioClip sound_Fire;
    // Start is called before the first frame update
    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        theAudio.clip = sound_Fire;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Spin();
        SearchEnemy();
        LookTarget();
        Attack();
    }

    private void Spin()
    {
        if(!isFindTarget && !isAttack)
        {
            Quaternion _spin = Quaternion.Euler(0f, tf_TopGun.eulerAngles.y + (1f * spinSpeed * Time.deltaTime) , 0f);
            tf_TopGun.rotation = _spin;
        }
    }

    private void SearchEnemy()
    {
        Collider[] _targets = Physics.OverlapSphere(tf_TopGun.position, range, layerMask);

        for (int i = 0; i < _targets.Length; i++)
        {
            Transform _targetTf = _targets[i].transform;

            if(_targetTf.tag == "Player")
            {
                Vector3 _direction = (_targetTf.position - tf_TopGun.position).normalized;
                float _angle = Vector3.Angle(_direction, tf_TopGun.forward);

                if(_angle < viewAngle * 0.5f)
                {
                    tf_Target = _targetTf;
                    isFindTarget = true;

                    if(_angle < 5f)
                        isAttack = true;
                    else
                        isAttack= false;
                    return;

                }
            }
        }
        tf_Target= null;
        isAttack= false;
        isFindTarget=false;

    }

    private void LookTarget()
    {
        if(isFindTarget == true)
        {
            Vector3 _direction = (tf_Target.position - tf_TopGun.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            Quaternion _rotation = Quaternion.Lerp(tf_TopGun.rotation, _lookRotation, 0.2f);
            tf_TopGun.rotation= _rotation;
        }
    }

    private void Attack()
    {
        if(isAttack == true)
        {
            currentRateOfFire += Time.deltaTime;
            if(currentRateOfFire >= rateOfFire)
            {
                currentRateOfFire= 0;
                anim.SetTrigger("Fire");
                GenerateParticle();

                if(Physics.Raycast(tf_TopGun.position,
                                   tf_TopGun.forward + new Vector3(Random.Range(-1, 1f) * rateOfAccurasy, Random.Range(-1, 1f) * rateOfAccurasy ,  0f),
                                   out hitInfo, range, layerMask))
                {
                    GameObject _temp = Instantiate(go_HitEffect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Destroy(_temp, 1f);

                    if (hitInfo.transform.tag == "Player")
                    {
                        hitInfo.transform.GetComponent<StatusController>().DecreaseHP(damage);
                    }
                }

                
         
            }
        }
    }
    void GenerateParticle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (particlePrefab != null)
        {
            // 플레이어 방향으로 파티클 오브젝트 생성
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(playerDirection);

            // 파티클 오브젝트를 생성하고 위치 및 방향 설정
            waitfire(); //피하기 위한 딜레이\
            GameObject particleObject = Instantiate(particlePrefab, transform.position, rotation);
            particle_MuzzleFlash.Play();
            theAudio.Play();

            // 일정 시간이 지난 후 파티클 오브젝트 파괴
            Destroy(particleObject, 2.0f);  
        }
    }

    IEnumerable waitfire()
    {
        yield return new WaitForSeconds(2f);
    }
    






}
