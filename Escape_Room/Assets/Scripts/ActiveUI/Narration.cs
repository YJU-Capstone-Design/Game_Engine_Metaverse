using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Narration : MonoBehaviour
{
    public string bed;           // ħ��
    public string deadBody;      // ��ü 
    public string chair;         // ����
    public string laptop;        // ��Ʈ��
    public string document;      // ����
    public string playerBag;     // �÷��̾� ����
    public string deadBodyBag;   // ������ ����
    public string IDcard;        // �ź���
    public string wallClock;     // ������ �ð�
    public string kitchenKnife;  // ��Į
    public string WallTV;        // ������ TV
    public string keyLock;       // ���� �ڹ���
    public string directionLock; // ���� �ڹ���
    public string dialLock;      // ��ȣ �ڹ���
    public string buttonLock;    // ��ư �ڹ���
    public string key;           // ����
    public string storageCloset; // ������


    private void Awake()
    {
        bed = "�����ڰ� ������ �ִ� ħ���. ���� �ܼ��� ���� �� ����.";
        deadBody = "�׾� �ִ� ��ü�� �ܼ��� ���� ���� �� ����.";
        chair = "���� ���� ���ڴ�. ���� Ư���� ���� ���� �� ����.";
        laptop = "��Ʈ�Ͽ� ��й�ȣ�� �ɷ� �ִ�. ���� ����� ���ؼ��� �н����尡 �ʿ��ϴ�.\n��Ʈ���� Ű�е� ����Ű�� ���� ������ �Ʒ��� ������ ��ũ�� ����ϴ�.";
        document = "3.25�̶�� ���ڿ� ���������� ���������� ���׶�̰� ���� �ִ�. �߿��� �� ����.";
        playerBag = "����, ��, ���찳, ��, ���� �� ����� �п�ǰ�� ����ִ�.(��Ʈ ��� �� ���� ����)";
        deadBodyBag = "�ź����� ��� �ִ� ������ ����ִ�.";
        IDcard = "�ź����� ���� �����ڴ� 25���� �� ����. ������ 1999.03.25�̴�.";
        wallClock = "���� �ð��� 19:32�̴�.";
        kitchenKnife = "�ǰ� ���� ��Į�̴�. �߿��� ����ǰ�� �� �� ����.";
        WallTV = "���������� �Ͼ�� ������� ����� ���� �ϰ� �ִ�. ���ݱ��� �����ڴ� 27���� �� ����.";
        keyLock = "���谡 �ʿ��� �ڹ����.";
        directionLock = "������ �ڹ����.";
        dialLock = "3���� ���ڸ� �Է��ϴ� ��ȣ �ڹ����.";
        buttonLock = "4���� ���ڸ� �Է��ϴ� ��ư �ڹ����.";
        key = "�̰ɷ� ������ �ڹ��踦 �� �� ���� �� ����.";
        storageCloset = "����� �������� �� ����";
    }
}
