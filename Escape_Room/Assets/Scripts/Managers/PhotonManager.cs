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
using UnityEngine.TextCore.Text;
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
    public PartyList myParty; // ������ ����/������ ��Ƽ
    public PlayerNameBox myPlayerName; // ������ ���� �̸� Box
    [SerializeField] Vector3 spawnPoint; // ������ Player(Character) �� Spawn �Ǵ� ��ġ
    public List<GameObject> playerList; // ������ Player(Character) �� ����Ǵ� List

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

        Debug.Log(roomName);

        // �� ����
        PhotonNetwork.JoinRoom(roomName);

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
        Debug.Log($"CreateRoom Failed {returnCode}:{message}");

        // �� ����
        PhotonNetwork.JoinRoom(roomName);
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

        // Player Name Box ����
        PhotonNetwork.Instantiate(playerNameBoxPrefab.name, LobbyUIManager.Instance.playerNameBoxParent.transform.position, Quaternion.identity);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRoom Failed {returnCode}:{message}");

        // ���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;              // �ִ� ������ �� : 20
        roomOptions.IsOpen = true;                // ���� ���� ����
        roomOptions.IsVisible = true;             // �κ񿡼� �� ��Ͽ� ���� ��ų�� ����

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    // Room �� ������ �� ����Ǵ� �ݹ��Լ�
    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby();
    }

    // ���� ���� �� ����Ǵ� �ݹ��Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        pv.RPC("LeftPhoton", RpcTarget.All, otherPlayer.ActorNumber, true); // ���ΰ� ���õ� �����͵� ����
    }



    // -----------------------------------------------------------------------------------



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
        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, false); // ���ΰ� ���õ� �����͵� ����

        // ����  mini ��Ƽ UI ��Ȱ��ȭ
        LobbyUIManager.Instance.miniPartyUI.SetActive(false);

        // myParty �ʱ�ȭ
        myParty = null;
    }

    public void GameStartButton()
    {
        roomName = myPlayer.ViewID.ToString(); // ���� ���� Room �̸� ����

        pv.RPC("GameStart", RpcTarget.All, this.roomName);
    }

    [PunRPC]
    // ���� ���� ��ư
    public void GameStart(string roomName)
    {
        // �ڽ��� ��Ƽ�� ������ ������ ����� ���� ��쿡�� �۵�
        if (myParty.partyPlayerIDList.Contains(int.Parse(roomName)))
        {
            Debug.Log("Aaa");

            this.roomName = roomName; // ������ Room �̸� ����

            pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, true); // ���ΰ� ���õ� �����͵� ����

            PhotonNetwork.LeaveRoom();

            //PhotonNetwork.JoinLobby();

            foreach (GameObject lobbyUI in LobbyUIManager.Instance.activeUIBoxs)
            {
                if (lobbyUI.activeInHierarchy)
                {
                    lobbyUI.SetActive(false);
                }
            }

            // ���� mini ��Ƽ UI ��Ȱ��ȭ
            LobbyUIManager.Instance.miniPartyUI.SetActive(false);

            // List �� �ʱ�ȭ
            LobbyUIManager.Instance.partyPlayerList.Clear();
            partyList.Clear();
        }
    }


    // ���� ���� ���� �Ǵ� ��Ƽ Ż�� �� ����� �Լ�
    [PunRPC]
    public void LeftPhoton(int ActorNum, bool leftPhoton)
    {
        // ������ ��� Ȯ��
        for (int i = 0; i < playerList.Count; i++)
        {
            PhotonView playerPV = playerList[i].GetComponent<PhotonView>();

            // ���� ���� ���� �ÿ��� �÷��̾� ����Ʈ�� Scene���� ���� Character ����
            if (playerPV.ViewID / 1000 == ActorNum && leftPhoton)
            {
                LobbyUIManager.Instance.playerNameBoxList.Remove(myPlayerName.gameObject);

                playerList.Remove(playerPV.gameObject);
                Destroy(playerPV.gameObject);
            }

            // ��Ƽ ����Ʈ
            for (int j = 0; j < partyList.Count; j++)
            {
                PartyList partyLogic = partyList[j].GetComponent<PartyList>();
                PhotonView partyPV = partyList[j].GetComponent<PhotonView>();

                // �ڽ��� ���� �ִ� ��Ƽ ã��
                if (partyLogic.partyPlayerIDList.Contains(playerPV.ViewID) && playerPV.ViewID / 1000 == ActorNum)
                {
                    // �ο� UI �ǽð� ����
                    partyPV.RPC("SynchronizationPeopleNum", RpcTarget.AllBuffered, playerPV.ViewID, partyLogic.partyPlayerIDList.Count, partyLogic.maxPeopleNum, false);

                    // ������ ��Ƽ���� ������ ���� �� ���ο��Լ� ��� ������ ����
                    if (partyPV.ViewID / 1000 == ActorNum)
                    {
                        for (int k = 0; k < partyLogic.lobbyUIManager.partyPlayerList.Count; k++)
                        {
                            if (partyLogic.lobbyUIManager.partyPlayerList[k] != null)
                            {
                                PlayerNameBox playerNameBoxLogic = partyLogic.lobbyUIManager.partyPlayerList[k].GetComponent<PlayerNameBox>();

                                // ��Ƽ���� �� ���� Player Name Box ����ġ
                                playerNameBoxLogic.transform.SetParent(LobbyUIManager.Instance.playerNameBoxParent);
                                playerNameBoxLogic.GetComponent<RectTransform>().localPosition = Vector2.zero;

                                // ��Ƽ���� �� ���� Photon Manager �� myParty �ʱ�ȭ
                                playerNameBoxLogic.lobbyUIManager.photonManager.myParty = null;

                                // ��Ƽ���� �� ���� mini ��Ƽ UI ��Ȱ��ȭ
                                playerNameBoxLogic.lobbyUIManager.miniPartyUI.SetActive(false);

                                // Player Name Box ��Ȱ��ȭ
                                playerNameBoxLogic.gameObject.SetActive(false); 
                            }

                            // ����Ʈ �ʱ�ȭ
                            if (partyLogic.lobbyUIManager.partyPlayerList.Count - 1 == k)
                            {
                                partyLogic.lobbyUIManager.partyPlayerList.Clear();
                            }
                        }

                        // ����Ʈ�� Scene ���� ������ ���� ��Ƽ ����
                        Destroy(partyList[j].gameObject);
                        partyList.Remove(partyList[j]);
                    }
                    else // ������ ��Ƽ���� �ƴ� ��쿡�� ������ �����͸� ����
                    {
                        // ������ mini ��Ƽ UI ���� ���� PlayerNameBox �� playerNameBoxParent �ڽ� ������Ʈ�� ����ġ
                        for (int k = 0; k < partyLogic.lobbyUIManager.partyPlayerList.Count; k++)
                        {
                            PhotonView partyPlayerNameBoxPV = partyLogic.lobbyUIManager.partyPlayerList[k].GetComponent<PhotonView>();

                            if (partyPlayerNameBoxPV.ViewID / 1000 == ActorNum)
                            {
                                // Player Name Box ����ġ
                                partyPlayerNameBoxPV.transform.SetParent(LobbyUIManager.Instance.playerNameBoxParent);
                                partyPlayerNameBoxPV.GetComponent<RectTransform>().localPosition = Vector2.zero;
                                partyPlayerNameBoxPV.gameObject.SetActive(false);

                                // partyPlayerList ���� Player Name Box �� index ����
                                partyLogic.lobbyUIManager.partyPlayerList.Remove(partyLogic.lobbyUIManager.partyPlayerList[k]);
                            }
                        }
                    }
                }
            }
        }
    }
}
