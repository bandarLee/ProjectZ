using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Rifle, // 따발총
    Pistol, // 권총
}

public class Gun : MonoBehaviour
{
    public GunType GType;

    // - 대표 이미지
    public Sprite ProfileImage;

    // - 공격력
    public int Damage = 40;

    // - 발사 쿨타임
    public float FireCooltime = 0.2f;

    // - 총알 개수
    public int BulletRemainCount = 0;
    public int BulletMaxCount = 30;

    // - 재장전 시간
    public float ReloadTime = 1.5f;


}
