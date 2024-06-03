using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterRotateAbility : CharacterAbility
{
    // ���콺 �̵��� ���� ī�޶�� �÷��̾ ȸ��
    public Transform CameraRoot;
   
    private float _mx; // ����
    private float _my;
    //public float RotationSpeed = 200f;
    private void Start()
    {
        SetMouseLock(true);


        if (Owner.PhotonView.IsMine)
        {
            GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>().Follow = CameraRoot;
        }
    }


    private void OnLevelWasLoaded()
    {
        SetMouseLock(true);


        if (Owner.PhotonView.IsMine)
        {
            GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>().Follow = CameraRoot;
        }
    }

    private void Update()

    {
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }

        // ����:
        // 1. ���콺 �Է� ���� �޴´�.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. ȸ�� ���� ���콺 �Է¿� ���� �̸� �����Ѵ�.
        _mx += mouseX * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my += mouseY * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my = Mathf.Clamp(_my, -50f, 20f);

        // 3. ī�޶�(3��Ī)�� ĳ���͸� ȸ�� �������� ȸ����Ų��. 
        transform.eulerAngles = new Vector3(0, _mx, 0f); // X�� Z ���� 0���� ����, ȸ���� Y���� �߽����θ� 
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f); // "����"�� �θ� ������Ʈ�� ȸ���� �������� �Ѵ�
                                                                // -_my: ���콺�� ���� ������ �� ī�޶� �Ʒ��� ���ϰ� 

        // 4. �ó׸ӽ�- virtual ī�޶�
    }

    public void SetMouseLock(bool isLocked)
    { 
        if (isLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void SetRandomRotation()
    {
        _mx = Random.Range(0, 360);
        _my = 0;
    }
}