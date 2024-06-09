using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Narration : MonoBehaviour
{
    public PhotonManager photonManager;

    public string bed;           // ħ��
    public string deadBody;      // ��ü 
    public string chair;         // ����
    public string laptop;        // ��Ʈ��
    public string document;      // ����
    public string playerBag;     // �÷��̾� ����
    public string deadBodyBag;   // ������ ����
    public string wallet;        // ����
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
    public string hint; // ��Ʈ
    public string hintZero; // ��Ʈ�� �� ���
    public string remote; // ������
    public string livingroomTV; // �Ž� TV
    public string whiteBoard; // ȭ��Ʈ����
    public string doorLock; // �����
    public string refrigerator; // �����


    private void Awake()
    {
        bed = "�����ڰ� ������ �ִ� ħ���. ���� �ܼ��� ���� �� ����.";
        deadBody = "�׾� �ִ� ��ü�� �ܼ��� ���� ���� �� ����.";
        chair = "���� ���� ���ڴ�. ���� Ư���� ���� ���� �� ����.";
        laptop = "��Ʈ�Ͽ� ��й�ȣ�� �ɷ� �ִ�. ���� ����� ���ؼ��� �н����尡 �ʿ��ϴ�.\n��Ʈ���� Ű�е� ����Ű�� ���� ������ �Ʒ��� ������ ��ũ�� ����ϴ�.";
        document = "3.25�̶�� ���ڿ� ���������� ���������� ���׶�̰� ���� �ִ�. �߿��� �� ����.";
        playerBag = "����, ��, ���찳, ��, ���� �� ����� �п�ǰ�� ����ִ�.\n�п�ǰ���� �ڹ��� �ϳ��� �� �� ���� �� ����.";
        deadBodyBag = "�������� �����Դϴ�. ������ ���캸�ðڽ��ϱ�?\n������ ���캸�� �����ø� Enter�� �����ֽʽÿ�...";
        wallet = "�������� ���� �ӿ��� ������ �߰��߽��ϴ�.\n�ڼ��� Ȯ���ϰ� �����ø� Enter�� �����ֽʽÿ�...";
        IDcard = "�ź����� ���� �����ڴ� 25���� �� ����. ������ 1999.03.25�̴�.";
        wallClock = "�������� �� ���� �� ����. �ð��� 7:32�� ����Ű�� �ִ�.";
        kitchenKnife = "�ǰ� ���� ��Į�̴�. �߿��� ����ǰ�� �� �� ����.";
        WallTV = "���������� �Ͼ�� ������� ����� ���� �ϰ� �ִ�.\n���ݱ��� �����ڴ� 27���� �� ����.";
        keyLock = "���谡 �ʿ��� �ڹ����.\n���踦 ����Ͻñ� ���Ͻø� Enter�� �����ֽʽÿ�...";
        directionLock = "������ �ڹ����.\n�Է��� ���Ͻø� Enter�� �����ֽʽÿ�...";
        dialLock = "3���� ���ڸ� �Է��ϴ� ��ȣ �ڹ����.\n�Է��� ���Ͻø� Enter�� �����ֽʽÿ�...";
        buttonLock = "4���� ���ڸ� �Է��ϴ� ��ư �ڹ����.\n�Է��� ���Ͻø� Enter�� �����ֽʽÿ�...";
        key = "�̰ɷ� ������ �ڹ��踦 �� �� ���� �� ����.";
        storageCloset = "���� �ڹ����� ��Ʈ�� �����ִ� ������ �ִ�.\nAA ������ 3���� ������ �������̴�.";
        hintZero = "��� ������ ��Ʈ Ƚ���� �� ����ϼ̽��ϴ�.";
        remote = "���������� �ƹ��� ������ ����.";
        livingroomTV = "������ �������� ȭ�鿡 ������ ����.";
        whiteBoard = "�����ڵ��� ������ �� ����. ��� �������� X ǥ�ð� �Ǿ��ִ�. \n�ڼ��� Ȯ���ϰ� �����ø� Enter�� �����ֽʽÿ�...";
        doorLock = "��й�ȣ�� �Է��ϸ� Ż���� �� ���� �� ����.\n�Է��� ���Ͻø� Enter�� �����ֽʽÿ�...";
    }

    private void Update()
    {
        hint = $"���� ��Ʈ ��� ���� Ƚ���� �� {photonManager.hintCount} �� ���ҽ��ϴ�. \n����� ���Ͻø� Enter�� �����ֽʽÿ�...";
    }
}
