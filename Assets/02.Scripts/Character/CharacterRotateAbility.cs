using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterRotateAbility : CharacterAbility
{
    // 마우스 이동에 따라 카메라와 플레이어를 회전
    public Transform CameraRoot;
   
    private float _mx; // 각도
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

        // 순서:
        // 1. 마우스 입력 값을 받는다.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. 회전 값을 마우스 입력에 따라 미리 누적한다.
        _mx += mouseX * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my += mouseY * Owner.Stat.RotationSpeed * Time.deltaTime;
        _my = Mathf.Clamp(_my, -50f, 20f);

        // 3. 카메라(3인칭)와 캐릭터를 회전 방향으로 회전시킨다. 
        transform.eulerAngles = new Vector3(0, _mx, 0f); // X와 Z 축은 0으로 고정, 회전은 Y축을 중심으로만 
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0f); // "로컬"은 부모 오브젝트의 회전을 기준으로 한다
                                                                // -_my: 마우스를 위로 움직일 때 카메라가 아래를 향하게 

        // 4. 시네머신- virtual 카메라
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