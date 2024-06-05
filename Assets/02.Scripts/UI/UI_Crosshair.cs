using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Crosshair : MonoBehaviour
{
    // 크로스헤어를 구성하는 네 개의 이미지 (상, 하, 좌, 우)
    public RawImage up;
    public RawImage down;
    public RawImage left;
    public RawImage right;

    // 크로스헤어가 기본적으로 위치해야 할 중심 위치들을 저장
    Vector3 upDefaultPos;
    Vector3 downDefaultPos;
    Vector3 leftDefaultPos;
    Vector3 rightDefaultPos;

    // 크로스헤어가 중앙으로 돌아가는 속도
    float returnToCenterSpeed;

    private void Start()
    {
        // 각 방향별 크로스헤어의 기본 위치를 저장
        upDefaultPos = up.transform.position;
        downDefaultPos = down.transform.position;
        leftDefaultPos = left.transform.position;
        rightDefaultPos = right.transform.position;
    }

    void LateUpdate()
    {
        // 매 프레임 크로스헤어를 기본 위치로 수축시키는 함수 호출
        ShrinkCrosshairToNormal();
    }

    // 크로스헤어를 기본 위치로 서서히 수축시키는 함수
    void ShrinkCrosshairToNormal() 
    {
        // 크로스헤어의 각 부분이 기본 위치보다 바깥에 있다면 중앙으로 이동
        if (up.transform.position.y > upDefaultPos.y)
            up.transform.position = new Vector3(up.transform.position.x, up.transform.position.y - returnToCenterSpeed, up.transform.position.z);

        if (down.transform.position.y < downDefaultPos.y)
            down.transform.position = new Vector3(down.transform.position.x, down.transform.position.y + returnToCenterSpeed, down.transform.position.z);

        if (left.transform.position.x < leftDefaultPos.x)
            left.transform.position = new Vector3(left.transform.position.x + returnToCenterSpeed, left.transform.position.y, left.transform.position.z);

        if (right.transform.position.x > rightDefaultPos.x)
            right.transform.position = new Vector3(right.transform.position.x - returnToCenterSpeed, right.transform.position.y, right.transform.position.z);
    }

    // 크로스헤어를 외부에서 지정한 양만큼 확장시키는 함수
    public void Expand(float expandAmount) 
    {
        up.transform.position += new Vector3(0, expandAmount, 0);
        down.transform.position += new Vector3(0, -expandAmount, 0);
        left.transform.position += new Vector3(-expandAmount, 0, 0);
        right.transform.position += new Vector3(expandAmount, 0, 0);
    }

    // 크로스헤어가 중앙으로 수축되는 속도를 설정하는 함수
    public void SetShrinkSpeed(float shrinkSpeed) 
    {
        returnToCenterSpeed = shrinkSpeed;
    }



}
