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
    // ����
    private readonly string version = "1.0f";

    [Header("# Prefab")]
    public GameObject playerPrefab;
    public GameObject partyListPrefab;
    PhotonView pv;

    [Header("# Player")]
    public string masterName = "Test"; // ����� �̸�
    public PhotonView myPlayer;
    [SerializeField] Vector3 spawnPoint;
    public List<GameObject> playerList;

    [Header("# PartyList Info")]
    public string theme; // �׸�
    public string partyPeopleNum; // ��Ƽ ���� �� �ο� �� �ؽ�Ʈ (����Ʈ�� ���� �ؽ�Ʈ) -> "���� �ο��� / �ִ� �ο���" ����

    [Header("# MakeParty Info")]
    public List<GameObject> partyList; // ������ ����Ʈ�� ����
    public int maxPeopleNum = 1; // ��Ƽ ���鶧 �����ϴ� �ο� ��
    public int partyPageLength = 1; // �� ��Ƽ ������ �� -> list / 8 + 1 �� �����
    public TextMeshProUGUI maxPeopleNumText; // ��Ƽ ���� �� maxPeopleNum �� ���� UI
    public TextMeshProUGUI pageCountText; // partyPageLength �� �� TextMeshPro

    private void Awake()
    {
        // �׽�Ʈ�� 
        masterName = "�����";
        theme = "������";
        pv = GetComponent<PhotonView>();

        // ���� ���� �����鿡�� �ڵ����� ���� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;

        // ���� ������ �������� ���� ���
        PhotonNetwork.GameVersion = version;

        // ���� ���̵� �Ҵ�
        PhotonNetwork.NickName = masterName;

        // ���� ������ ��� Ƚ�� ���� -> �⺻���� �ʴ� 30ȸ
        Debug.Log(PhotonNetwork.SendRate);

        // ���� ����
        PhotonNetwork.ConnectUsingSettings();

        // # �÷��̾� ����Ʈ �ʱ�ȭ
        playerList = new List<GameObject>();

        // # ��Ƽ �ý��� �ʱ�ȭ
        partyList = new List<GameObject>();
        partyPeopleNum = "1 / 1";
        maxPeopleNumText.text = "1";
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

    // ���� ���� �� ����Ǵ� �ݹ��Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID); // ���ΰ� ���õ� �����͵� ����
    }



    // # �ΰ��� �Լ�

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
            maxPeopleNum = (maxPeopleNum == 5 ? 1 : ++maxPeopleNum);
        }
        else if (set == "Down")
        {
            maxPeopleNum = (maxPeopleNum == 1 ? 5 : --maxPeopleNum);
        }

        maxPeopleNumText.text = maxPeopleNum.ToString(); // ��Ƽ �ο� ���� ������ �� ���̴� UI
        partyPeopleNum = $"{1} / {maxPeopleNum}"; // ������ list �� ���Խ����� ���� �����ϴ� ���ڿ� -> 1 �� �⺻��
    }


    // ���� ���� ���� �� ����� �Լ�
    [PunRPC]
    public void LeftPhoton(int id)
    {
        // ������ ��� Ȯ��
        for (int i = 0; i < playerList.Count; i++)
        {
            PhotonView playerPV = playerList[i].GetComponent<PhotonView>();

            if (playerPV.ViewID == id)
            {
                // �÷��̾� ����Ʈ���� ���� ����
                playerList.Remove(playerPV.gameObject);

                // ��Ƽ ����Ʈ
                for(int j = 0; j < partyList.Count; j++)
                {
                    PartyList partyLogic = partyList[i].GetComponent<PartyList>();

                    // ����Ʈ���� ������ ���� ��Ƽ ����
                    if (playerPV.IsMine)
                    {
                        partyList.Remove(partyLogic.gameObject);
                    }
                    else if (partyLogic.partyPlayerIDList.Contains(id)) // �ڽ��� ���� �ִ� ��Ƽ���� ���� ����
                    {
                        partyLogic.partyPlayerIDList.Remove(id);
                    }
                }
            }
        }
    }
}
