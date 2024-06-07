using Cinemachine;
using UnityEngine;

public class CharacterRotateAbility : CharacterAbility
{
    public Transform CameraRoot;

    private float _mx;
    private float _my;
    private bool CharacterRotateLocked = false;

    private float lastRotationY;
    private float lastCameraRotationX;

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
        if (Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }

        if (!CharacterRotateLocked)
        {
            CharacterRotate();
        }
        else
        {
            transform.eulerAngles = new Vector3(0, lastRotationY, 0f);
            CameraRoot.localEulerAngles = new Vector3(-lastCameraRotationX, 0, 0f);
        }
    }

    public void SetMouseLock(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            CharacterRotateLocked = false;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            CharacterRotateLocked = true;

            ResetMouseInput();
        }
    }

    private void ResetMouseInput()
    {
        _mx = transform.eulerAngles.y;
        _my = -CameraRoot.localEulerAngles.x;

        // 마지막 회전값 저장
        lastRotationY = _mx;
        lastCameraRotationX = _my;
    }

    public void SetRandomRotation()
    {
        _mx = Random.Range(0, 360);
        _my = 0;
    }

    public void CharacterRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _mx += mouseX * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my += mouseY * Owner.Stat.RotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -50f, 25f);

        transform.eulerAngles = new Vector3(0, _mx, 0f);
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f);

        lastRotationY = _mx;
        lastCameraRotationX = _my;
    }
}
