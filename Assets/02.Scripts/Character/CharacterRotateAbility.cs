using Cinemachine;
using UnityEngine;

public class CharacterRotateAbility : MonoBehaviour
{
    // ���콺 �̵��� ���� ī�޶�� �÷��̾ ȸ��
    public Transform CameraRoot;

    private float _mx; // ����
    private float _my;
    public float RotationSpeed = 200f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>().Follow = CameraRoot;
       
    }

    private void Update()
    {

        // ����:
        // 1. ���콺 �Է� ���� �޴´�.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. ȸ�� ���� ���콺 �Է¿� ���� �̸� �����Ѵ�.
        _mx += mouseX * RotationSpeed * Time.deltaTime;
        _my += mouseY * RotationSpeed * Time.deltaTime;
        _my = Mathf.Clamp(_my, -90f, 90f);

        // 3. ī�޶�(3��Ī)�� ĳ���͸� ȸ�� �������� ȸ����Ų��. 
        transform.eulerAngles = new Vector3(0, _mx, 0f); // X�� Z ���� 0���� ����, ȸ���� Y���� �߽����θ� 
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f); // "����"�� �θ� ������Ʈ�� ȸ���� �������� �Ѵ�
                                                                // -_my: ���콺�� ���� ������ �� ī�޶� �Ʒ��� ���ϰ� 

        // 4. �ó׸ӽ�- virtual ī�޶�

    }

    public void SetRandomRotation()
    {
        _mx = Random.Range(0, 360);
        _my = 0;
    }
}
