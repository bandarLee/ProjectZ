using Cinemachine;
using UnityEngine;

public class CharacterRotateAbility : CharacterAbility
{
    // ���콺 �̵��� ���� ī�޶�� �÷��̾ ȸ��
    public Transform CameraRoot;
   
    private float _mx; // ����
    private float _my;
    //public float RotationSpeed = 200f;

    public float sideViewRotationOffset = 90f;  // ī�޶��� �߰� ȸ�� ������
    public float smoothTime = 1f;  // ī�޶� ȸ���� �ε巯�� ��ȯ �ð�
    private float _targetRotation; // ��ǥ ȸ�� ����
    private float _rotationVelocity; // ȸ�� �ӵ� (�ε巯�� ��ȯ�� ���� ���� ����)
    private bool isSideViewActive = false;

    private void Start()
    {
        SetMouseLock(true);


        if (Owner.PhotonView.IsMine)
        {
            GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>().Follow = CameraRoot;
        }
    }

    private void Update()
    {
        if (!Owner.PhotonView.IsMine)
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
        _my = Mathf.Clamp(_my, -90f, 90f);

        // 3. ī�޶�(3��Ī)�� ĳ���͸� ȸ�� �������� ȸ����Ų��. 
        transform.eulerAngles = new Vector3(0, _mx, 0f); // X�� Z ���� 0���� ����, ȸ���� Y���� �߽����θ� 
        //CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f); // "����"�� �θ� ������Ʈ�� ȸ���� �������� �Ѵ�
                                                                // -_my: ���콺�� ���� ������ �� ī�޶� �Ʒ��� ���ϰ� 

        // 4. �ó׸ӽ�- virtual ī�޶�

        HandleSideView();
    }

    // �÷��̾ �¿�� ������ �� ī�޶� ȸ�� ó��
    private void HandleSideView()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            isSideViewActive = true;
            //CameraRoot.localEulerAngles += new Vector3(0, -sideViewRotationOffset * Mathf.Sign(horizontalInput), 0);
            _targetRotation = _mx + -sideViewRotationOffset * Mathf.Sign(horizontalInput);
        }
        else 
        {
            isSideViewActive = false;
            // CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f);
            _targetRotation = _mx;
        }


        // �ε巴�� ī�޶��� Y�� ȸ�� ������ ����
        float smoothedRotation = Mathf.SmoothDampAngle(CameraRoot.eulerAngles.y, _targetRotation, ref _rotationVelocity, smoothTime);
        CameraRoot.eulerAngles = new Vector3(-_my, smoothedRotation, 0);
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