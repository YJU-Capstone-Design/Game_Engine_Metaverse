using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks // �������ִ� �پ��� CallBack �Լ��� �� �� ����.
{
    [SerializeField] Vector3 spawnPoint;

    // ����
    private readonly string version = "1.0f";

    // ����� ���̵� �Է�
    private string userId = "Test";

    [Header("# Prefab")]
    public GameObject playerPrefab;
    public GameObject partyListPrefab;

    [Header("# Player")]
    public PhotonView myPlayer;


    [Header("# PartyList Info")]
    public string masterName; // ��Ƽ�� �̸�
    public string theme; // �׸�
    public string partyPeopleNum; // ��Ƽ ���� �� �ο� �� �ؽ�Ʈ (����Ʈ�� ���� �ؽ�Ʈ)

    [Header("# MakeParty Info")]
    public List<GameObject> partyList; // ������ ����Ʈ�� ����
    public int peopleNum = 1; // �� ���鶧 �����ϴ� �ο� ��
    public TextMeshProUGUI setPeopleNumText; // peopleNum �� �� TextMeshPro
    public int partyPageLength = 1; // �� ��Ƽ ������ �� -> list / 8 + 1 �� �����
    public TextMeshProUGUI pageCountText; // partyPageLength �� �� TextMeshPro

    private void Awake()
    {
        // ���� ���� �����鿡�� �ڵ����� ���� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;

        // ���� ������ �������� ���� ���
        PhotonNetwork.GameVersion = version;

        // ���� ���̵� �Ҵ�
        PhotonNetwork.NickName = userId;

        // ���� ������ ��� Ƚ�� ���� -> �⺻���� �ʴ� 30ȸ
        Debug.Log(PhotonNetwork.SendRate);

        // ���� ����
        PhotonNetwork.ConnectUsingSettings();


        // # ��Ƽ �ý��� �ʱ�ȭ
        setPeopleNumText.text = peopleNum.ToString();
        partyList = new List<GameObject>();
        partyPeopleNum = "1 / 1";
        // �׽�Ʈ�� 
        masterName = "�����";
        theme = "������";
    }

    // ���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // �κ� ���� ���� true , false ��� -> ���� ���̶� false
        // $"" => string.Format() �� ���ΰ� -> ""�ȿ� �ִ� ������ ���ڿ��� ��ȯ��. 

        PhotonNetwork.JoinLobby(); // �κ� ����
    }

    // �κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // �κ� ���� ���� true , false ��� -> ���� �Ķ� true

        PhotonNetwork.JoinRandomRoom(); // ���� ��ġ����ŷ ��� ����
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;              // �ִ� ������ �� : 20��
        roomOptions.IsOpen = true;                // ���� ���� ����
        roomOptions.IsVisible = true;             // �κ񿡼� �� ��Ͽ� ���� ��ų�� ����

        // �� ����
        PhotonNetwork.CreateRoom("My Room", roomOptions);
    }

    // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // �뿡 ������ ����� ���� Ȯ��
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}"); // �̸��� ������ ���
        }

        // ĳ���� ����
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);
    }

    // ��Ƽ ��Ī �ý������� ��(��Ƽ) �����
    public GameObject MakePartyRoom()
    {
        GameObject partyList = PhotonNetwork.Instantiate(partyListPrefab.name, transform.position, Quaternion.identity);

        return partyList;
    }

    // �� ���鶧 �ο� �� ���ϴ� UI ��ư
    public void SetPeopleNum(string set)
    {
        if (set == "Up")
        {
            peopleNum = (peopleNum == 5 ? 1 : ++peopleNum);

            setPeopleNumText.text = peopleNum.ToString();
        }
        else if (set == "Down")
        {
            peopleNum = (peopleNum == 1 ? 5 : --peopleNum);

            setPeopleNumText.text = peopleNum.ToString();
        }

        partyPeopleNum = $"{1} / {peopleNum}"; // 1 �� ���߿� ��Ƽ ��� ����� ����
    }
}
