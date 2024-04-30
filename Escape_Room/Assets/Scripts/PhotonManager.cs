using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks // �������ִ� �پ��� CallBack �Լ��� �� �� ����.
{
    [Header("# Photon")]
    private readonly string version = "1.0f"; // ����
    string roomName;

    [Header("# Prefab")]
    public GameObject playerPrefab;
    public GameObject partyListPrefab;
    public GameObject playerNameBoxPrefab;
    PhotonView pv;

    [Header("# Player")]
    public string masterName = "Test"; // ����� �̸�
    public PhotonView myPlayer; // ������ ���� ĳ����
    public PartyList myParty; // ������ ���� ��Ƽ ����Ʈ
    [SerializeField] Vector3 spawnPoint;
    public List<GameObject> playerList;

    [Header("# PartyList Info")]
    public string theme; // �׸�
    public string partyPeopleNum; // ��Ƽ ���� �� �ο� �� �ؽ�Ʈ (����Ʈ�� ���� �ؽ�Ʈ) -> "���� �ο��� / �ִ� �ο���" ����
    public int myPartyMaxPeople; // ������ ��Ƽ�� �ִ� �ο� �� -> ���� ���� �� ���� ���� Room �� �ɼǿ� �ʿ�

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

        // Room �⺻�� ����
        roomName = "My Room";
        myPartyMaxPeople = 20;

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

        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = myPartyMaxPeople;              // �ִ� ������ �� : 20��
        roomOptions.IsOpen = true;                // ���� ���� ����
        roomOptions.IsVisible = true;             // �κ񿡼� �� ��Ͽ� ���� ��ų�� ����

        // �� ����
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);

        //PhotonNetwork.JoinRandomRoom(); // ���� ��ġ����ŷ ��� ����
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    Debug.Log($"JoinRandom Failed {returnCode}:{message}");

    //    // ���� �Ӽ� ����
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = 20;              // �ִ� ������ �� : 20��
    //    roomOptions.IsOpen = true;                // ���� ���� ����
    //    roomOptions.IsVisible = true;             // �κ񿡼� �� ��Ͽ� ���� ��ų�� ����

    //    // �� ����
    //    PhotonNetwork.CreateRoom(roomName, roomOptions);
    //}

    // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = myPartyMaxPeople;              // �ִ� ������ �� : 20��
        roomOptions.IsOpen = true;                // ���� ���� ����
        roomOptions.IsVisible = true;             // �κ񿡼� �� ��Ͽ� ���� ��ų�� ����

        // �� ����
        PhotonNetwork.CreateRoom(roomName, roomOptions);
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
        pv.RPC("LeftPhoton", RpcTarget.All, otherPlayer.ActorNumber, true); // ���ΰ� ���õ� �����͵� ����
    }



    // # �ΰ��� �Լ�

    // ��Ƽ ��Ī �ý������� ��(��Ƽ) �����
    public GameObject MakePartyRoom()
    {
        GameObject partyList = PhotonNetwork.Instantiate(partyListPrefab.name, transform.position, Quaternion.identity);
        myPartyMaxPeople = maxPeopleNum;

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

    // ��Ƽ Ż�� ��ư
    public void LeftParty()
    {
        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID/1000, false); // ���ΰ� ���õ� �����͵� ����

        // ����  mini ��Ƽ UI ��Ȱ��ȭ
        LobbyUIManager.Instance.miniPartyUI.SetActive(false);
    }

    public void GameStartButton()
    {
        roomName = myPlayer.ViewID.ToString(); // ���� ���� Room �̸� ����

        // ��Ƽ�� ���Ե� �÷��̾���� ���ο� ������ ����
        for (int i = myParty.partyPlayerIDList.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < playerList.Count; j++)
            {
                PhotonView playerPV = playerList[j].GetComponent<PhotonView>();

                if (playerPV.ViewID == myParty.partyPlayerIDList[i])
                {
                    playerList[j].GetComponent<PlayerMove>().photonManager.pv.RPC("GameStart", RpcTarget.All, roomName);
                    //playerList[j].GetComponent<PlayerMove>().photonManager.GameStart(roomName);
                    Debug.Log(j);
                }
            }
        }
    }

    [PunRPC]
    // ���� ���� ��ư
    public void GameStart(string roomName)
    {
        this.roomName = roomName; // ������ Room �̸� ����

        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, true); // ���ΰ� ���õ� �����͵� ����

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.ConnectUsingSettings();
    }


    // ���� ���� ���� �Ǵ� ��Ƽ Ż�� �� ����� �Լ�
    [PunRPC]
    public void LeftPhoton(int ActorNum, bool leftPhoton)
    {
        // ������ ��� Ȯ��
        for (int i = 0; i < playerList.Count; i++)
        {
            PhotonView playerPV = playerList[i].GetComponent<PhotonView>();

            // ���� ���� ���� �ÿ��� �÷��̾� ����Ʈ�� Scene���� ���� ����
            if (playerPV.ViewID / 1000 == ActorNum && leftPhoton)
            {
                playerList.Remove(playerPV.gameObject);
                Destroy(playerPV.gameObject);
            }

            // ��Ƽ ����Ʈ
            for (int j = 0; j < partyList.Count; j++)
            {
                PartyList partyLogic = partyList[j].GetComponent<PartyList>();
                PhotonView partyPV = partyList[j].GetComponent<PhotonView>();

                // �ڽ��� ���� �ִ� ��Ƽ���� ���� ����
                if (partyLogic.partyPlayerIDList.Contains(playerPV.ViewID) && playerPV.ViewID / 1000 == ActorNum) 
                {
                    // �ο� UI �ǽð� ����
                    partyPV.RPC("SynchronizationPeopleNum", RpcTarget.AllBuffered, playerPV.ViewID, partyLogic.partyPlayerIDList.Count, partyLogic.maxPeopleNum, false);

                    // mini ��Ƽ UI Title ����
                    partyPV.RPC("SynchronizationPartyPeopleNum", RpcTarget.AllBuffered, partyLogic.listPeopleNumText.text);
                }

                // ������ ��Ƽ���϶�
                if (partyPV.ViewID/1000 == ActorNum)
                {
                    // ����Ʈ�� Scene ���� ������ ���� ��Ƽ ����
                    partyList.Remove(partyLogic.gameObject);
                    Destroy(partyLogic.gameObject);

                    // ������ �� ���� mini ��Ƽ UI ��Ȱ��ȭ �� ����Ʈ �ʱ�ȭ
                    partyLogic.lobbyUIManager.miniPartyUI.SetActive(false);

                    for (int k = 0; k < partyLogic.lobbyUIManager.partyPlayerList.Count; k++)
                    {
                        if (partyLogic.lobbyUIManager.partyPlayerList[k] != null)
                        {
                            Destroy(partyLogic.lobbyUIManager.partyPlayerList[k]);
                        }

                        if(partyLogic.lobbyUIManager.partyPlayerList.Count - 1 == k)
                        {
                            partyLogic.lobbyUIManager.partyPlayerList.Clear();
                        }
                    }
                }
                else // ������ ��Ƽ���� �ƴҶ�
                {
                    // ������ mini ��Ƽ UI ���� ���� PlayerNameBox ����
                    for (int k = 0; k < partyLogic.lobbyUIManager.partyPlayerList.Count; k++)
                    {
                        PhotonView partyPlayerPV = partyLogic.lobbyUIManager.partyPlayerList[k].GetComponent<PhotonView>();

                        if (partyPlayerPV.ViewID / 1000 == ActorNum)
                        {
                            Destroy(partyPlayerPV.gameObject);
                            partyLogic.lobbyUIManager.partyPlayerList.Remove(partyLogic.lobbyUIManager.partyPlayerList[k]);
                        }
                    }
                }
            }
        }
    }
}
