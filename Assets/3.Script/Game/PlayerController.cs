using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    #region Private ����
    private Rigidbody rb;
    private Animator anim;
    #endregion

    #region Public ����
    public Transform pointer; // ĳ���Ͱ� �ٶ� ����
    public Bomb bombPrefab; // ��ź ����ü ������
    public Transform shotPoint; // ����ü ������ġ
    public float moveSpeed = 5f; // �̵��ӵ�
    public float shotPower = 15f; // ����ü ������ ��
    public float hp = 100f; // ü�� 
    public int shotCount = 0; // �߻� Ƚ��
    public Text hpText;
    public Text shotText;

    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        pointer.gameObject.SetActive(photonView.IsMine); // ���� �����ϴ� ĳ������ pointer�� Ȱ��ȭ��
        hpText.text = hp.ToString();
    }
    private void FixedUpdate()
    {
        if (false == photonView.IsMine) return;
        Rotate();
    }
    private void Update()
    {
        if (false == photonView.IsMine) return;
        Move();

        if (Input.GetButtonDown("Fire1"))
        {
            shotCount++;
            shotText.text = shotCount.ToString();
            // ���ÿ����� ȣ��
            //Fire();

            // PhotonNetwork�� RPC�� ȣ��
            photonView.RPC("Fire", RpcTarget.All, shotPoint.position, shotPoint.forward);
        }


    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;
    }

    private void Rotate()
    {
        var position = rb.position; // �� rb�� ��ġ
        position.y = 0; //�������� ���� �� �����Ƿ� y�� ��ǥ�� 0����.

        var forward = pointer.position - position;

        rb.rotation = Quaternion.LookRotation(forward, Vector3.up);
        // �� ��ġ���� pointer���� �ٶ󺸰� ��
    }

    // Bomb�� ����䰡 ���� ��� ���ʿ��� ��Ŷ�� ��ȯ�Ǵ� ��ȿ���� �߻� �ϹǷ�, 
    // Ư�� Ŭ���̾�Ʈ�� Fire�� ȣ���� ��� �ٸ� Ŭ���̾�Ʈ���� RPC�� ����
    // �Ȱ��� Fire�� ȣ���ϰ� �ϰ����
    [PunRPC] // ��Ʈ����Ʈ �پ�Ѵ�.
    private void Fire(Vector3 shotPoint, Vector3 shotDirection, PhotonMessageInfo info)
    {
        //������ �����ؼ�, ���� �ð��� �� Ŭ���̾�Ʈ�� �ð� ���̸�ŭ ���� ����.
        print($"Fire Procedure Called by {info.Sender.NickName}");
        print($"local time : {PhotonNetwork.Time}");
        print($"server time : {info.SentServerTime}");

        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        var bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);
        bomb.rb.AddForce(shotDirection * shotPower, ForceMode.Impulse);
        bomb.owner = photonView.Owner;

        // ��ź�� ��ġ���� ��ź�� ��� ��ŭ �����ð����� ������ ��ġ�� ����
        bomb.rb.position += bomb.rb.velocity * lag;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        //if (hp =< 0) // ����
        hpText.text = hp.ToString();

    }

    public void Heal(float amount)
    {
        hp += amount;
        hpText.text = hp.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //stream�� ���ؼ� hp�� shotCount�� ����ȭ
        //stream�� queue�� ����
        if (stream.IsWriting) // ����
        {
            stream.SendNext(hp);
            stream.SendNext(shotCount);
        }
        else // ������
        {
            // Ư�� �� �� ���⶧���� ���� ������� �޾ƾ��Ѵ�.
            /*hp = float.Parse(stream.ReceiveNext().ToString());
            shotCount = int.Parse(stream.ReceiveNext().ToString());*/
            hp = (float)stream.ReceiveNext();
            shotCount = (int)stream.ReceiveNext();
            hpText.text = hp.ToString();
            shotText.text = shotCount.ToString();
        }
    }
}
