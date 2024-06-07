using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Rifle, // ������
    Pistol, // ����
}

public class Gun : MonoBehaviour
{
    public GunType GType;

    // - ��ǥ �̹���
    public Sprite ProfileImage;

    // - ���ݷ�
    public int Damage = 40;

    // - �߻� ��Ÿ��
    public float FireCooltime = 0.2f;

    // - �Ѿ� ����
    public int BulletRemainCount = 0;
    public int BulletMaxCount = 30;

    // - ������ �ð�
    public float ReloadTime = 1.5f;


}
