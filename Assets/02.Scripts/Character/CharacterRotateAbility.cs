using Cinemachine;
using UnityEngine;

public class CharacterRotateAbility : CharacterAbility
{
    public Transform CameraRoot;
    public Transform GunTransform;
    public Transform UpperBodyTransform;

    private float _mx;
    private float _my;
    public bool CharacterRotateLocked = false;

    private float lastRotationY;
    private float lastCameraRotationX;

    private CharacterGunFireAbility _gunFireAbility;

    private void Start()
    {
        SetMouseLock(true);

        InitializeCamera();
        _gunFireAbility = GetComponent<CharacterGunFireAbility>();
    }
    public void InitializeCamera()
    {
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

        if (_gunFireAbility != null && Character.LocalPlayerInstance._quickSlotManager.currentEquippedItem != null &&
           Character.LocalPlayerInstance._quickSlotManager.currentEquippedItem.itemType == ItemType.Gun)
        {
            GunTransform.localEulerAngles = new Vector3(-_my, -116.89f, 1.424f);
            UpperBodyTransform.localEulerAngles = new Vector3(-_my, UpperBodyTransform.localEulerAngles.y, UpperBodyTransform.localEulerAngles.z);
        }

        lastRotationY = _mx;
        lastCameraRotationX = _my;
    }
}
