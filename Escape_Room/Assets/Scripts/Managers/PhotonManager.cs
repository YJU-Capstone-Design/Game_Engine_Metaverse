using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using WebSocketSharp;

public class PhotonManager : MonoBehaviourPunCallbacks // �������ִ� �پ��� CallBack �Լ��� �� �� ����.
{
    public AudioManager audioManager;

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
    [SerializeField] Vector3 lobbyspawnPoint; // ������ Player(Character) �� Lobby �� Spawn �Ǵ� ��ġ
    [SerializeField] Vector3 inGameSpawnPoint; // ������ Player(Character) �� InGame �� Spawn �Ǵ� ��ġ
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

    [Header("# Start UI")]
    public GameObject startCanvas;
    public TMP_InputField userIDInput;
    public GameObject descriptionUI;
    [SerializeField] Image descriptionMainImg;
    [SerializeField] Sprite[] descriptionImg;
    [SerializeField] GameObject desPageBtn;

    [Header("# Common UI")]
    public GameObject lobbyCanvas;
    public LobbyUIManager lobbyUIManager;
    public GameObject inGameCanvas;
    public UIManager inGameUIManager;
    public GameObject loadingUI;
    public Animator loadingFadeAnim;
    public GameObject generalBtns; // ���� ���� â �Ϲ� ��ư
    public GameObject[] reCheckUI; // ����/�κ� ��Ȯ�� UI â

    [Header("# InGame")]
    public int hintCount;

    private void Awake()
    {
        startCanvas.SetActive(true);
        lobbyCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        loadingUI.SetActive(false);
        loadingFadeAnim.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient && inGameCanvas.activeInHierarchy)
        {
            if(UIManager.Instance.playTime >= 0)
            {
                pv.RPC("TimeLimit", RpcTarget.All);
            } 
            else
            {
                UIManager.Instance.playTime = 0;
            }

            UIManager.Instance.pv.RPC("Timer", RpcTarget.AllBuffered, UIManager.Instance.playTime);
        }

        SetPlayerList();

        // ���� ���� UI ESC ����
        if (descriptionUI.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                descriptionUI.SetActive(false);
                descriptionMainImg.sprite = descriptionImg[0];
                desPageBtn.SetActive(false);
            }
        }
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

        // �ε� ȭ�� ��Ȱ��ȭ
        StartCoroutine("Loading", false);

        // ��Ȳ�� �°� Canvas Ȱ��ȭ
        if (roomName == "Lobby")
        {
            // Lobby UI Ȱ��ȭ
            lobbyCanvas.SetActive(true);
            lobbyUIManager.gameObject.SetActive(true);
            lobbyUIManager.interacting = false;

            // InGameCanvas UI ��Ȱ��ȭ
            inGameCanvas.SetActive(false);
            inGameUIManager.gameObject.SetActive(false);

            // BGM
            audioManager.bgmAudio.clip = audioManager.bgmClips[0];
            audioManager.bgmAudio.Play();
        } 
        else
        {
            // InGameCanvas UI Ȱ��ȭ
            inGameCanvas.SetActive(true);
            inGameUIManager.gameObject.SetActive(true);

            // Lobby UI ��Ȱ��ȭ
            lobbyCanvas.SetActive(false);
            lobbyUIManager.gameObject.SetActive(false);
            lobbyUIManager.interacting = false;

            // ��Ʈ Ƚ�� �ʱ�ȭ
            hintCount = 2;

            // ���� �ʱ�ȭ
            UIManager.Instance.InGameSetting();

            // BGM
            audioManager.bgmAudio.clip = audioManager.bgmClips[1];
            audioManager.bgmAudio.Play();
        }

        // ĳ���� ����
        if(roomName == "Lobby")
        {
            PhotonNetwork.Instantiate(playerPrefab.name, lobbyspawnPoint, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate(playerPrefab.name, inGameSpawnPoint, Quaternion.identity, 0);
        }

        // Player Name Box ����
        PhotonNetwork.Instantiate(playerNameBoxPrefab.name, lobbyUIManager.playerNameBoxParent.transform.position, Quaternion.identity);
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

    // ���� ���� ��ư
    public void JoinGameButton()
    {
        if (userIDInput.text.IsNullOrEmpty())
        {
            Debug.Log("���̵� �Է����ּ���.");
        }
        else
        {
            JoinGame();
        }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Join Game Button");
    }

    // ���� ���� �Լ�
    private void JoinGame()
    {
        // �׽�Ʈ�� 
        masterName = userIDInput.text;
        theme = "������";
        pv = GetComponent<PhotonView>();

        // ���� ���� ȭ�� ��Ȱ��ȭ �� InputField �ʱ�ȭ
        startCanvas.SetActive(false);
        userIDInput.text = "";

        // �ε� ȭ�� Ȱ��ȭ
        StartCoroutine("Loading", true);

        // Room �⺻�� ����
        roomName = "Lobby";
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

    // ��Ƽ ��Ī �ý������� ��(��Ƽ) �����
    public GameObject MakePartyRoom()
    {
        GameObject partyList = PhotonNetwork.Instantiate(partyListPrefab.name, transform.position, Quaternion.identity);
        myPartyMaxPeople = maxPeopleNum;

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Make Party Button");

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

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("SetPeople Num Button");
    }

    // ��Ƽ Ż�� ��ư
    public void LeftParty()
    {
        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, false); // ���ΰ� ���õ� �����͵� ����

        // ����  mini ��Ƽ UI ��Ȱ��ȭ
        lobbyUIManager.miniPartyUI.SetActive(false);
        lobbyUIManager.gameStartButton.SetActive(false);

        // myParty �ʱ�ȭ
        myParty = null;

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Left party Button");
    }

    public void GameStartButton()
    {
        roomName = myPlayer.ViewID.ToString(); // ���� ���� Room �̸� ����

        pv.RPC("GameStart", RpcTarget.All, this.roomName, this.myPlayer.name);

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Game Start Button");
    }

    [PunRPC]
    // ���� ���� ��ư
    public void GameStart(string roomName, string bossName)
    {
        // �ڽ��� ��Ƽ�� ������ ������ ����� ���� ��쿡�� �۵�
        if (myParty.partyPlayerIDList.Contains(int.Parse(roomName)))
        {
            this.roomName = roomName + bossName; // ������ Room �̸� ����

            pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, true); // ���ΰ� ���õ� �����͵� ����

            PhotonNetwork.LeaveRoom();

            // �ε� ȭ�� Ȱ��ȭ
            StartCoroutine("Loading", true);

            foreach (GameObject lobbyUI in lobbyUIManager.activeUIBoxs)
            {
                if (lobbyUI.activeInHierarchy)
                {
                    lobbyUI.SetActive(false);
                }
            }

            // ���� mini ��Ƽ UI ��Ȱ��ȭ
            lobbyUIManager.miniPartyUI.SetActive(false);
            lobbyUIManager.gameStartButton.SetActive(false);

            // List �� �ʱ�ȭ
            lobbyUIManager.partyPlayerList.Clear();
            partyList.Clear();

            // LobbyCanvas ��Ȱ��ȭ
            lobbyCanvas.SetActive(false);
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
                // Player Name Box �� List ���� ����
                for(int j = 0; j < lobbyUIManager.playerNameBoxList.Count; j++)
                {
                    PhotonView playerNamePV = lobbyUIManager.playerNameBoxList[j].GetComponent<PhotonView>();

                    if(playerNamePV.ViewID / 1000 == ActorNum)
                    {
                        lobbyUIManager.playerNameBoxList.Remove(lobbyUIManager.playerNameBoxList[j]);
                    }
                }
                
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
                        for (int k = partyLogic.partyPlayerIDList.Count - 1; k > -1; k--)
                        {
                            for(int l = 0; l < lobbyUIManager.playerNameBoxList.Count; l++)
                            {
                                PhotonView playerNamePV = lobbyUIManager.playerNameBoxList[l].GetComponent<PhotonView>();

                                if(playerNamePV.ViewID/1000 == partyLogic.partyPlayerIDList[k]/1000 && playerNamePV.IsMine)
                                {
                                    // ��Ƽ���� �� ���� Player Name Box ����ġ
                                    playerNamePV.transform.SetParent(lobbyUIManager.playerNameBoxParent);
                                    playerNamePV.GetComponent<RectTransform>().localPosition = Vector2.zero;

                                    // ��Ƽ���� �� ���� Photon Manager �� myParty �ʱ�ȭ
                                    playerNamePV.GetComponent<PlayerNameBox>().lobbyUIManager.photonManager.myParty = null;

                                    // ��Ƽ���� �� ���� mini ��Ƽ UI ��Ȱ��ȭ
                                    playerNamePV.GetComponent<PlayerNameBox>().lobbyUIManager.miniPartyUI.SetActive(false);

                                    // Player Name Box ��Ȱ��ȭ
                                    playerNamePV.gameObject.SetActive(false);
                                }
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
                                // partyPlayerList ���� ���� ����
                                partyLogic.lobbyUIManager.partyPlayerList.Remove(partyLogic.lobbyUIManager.partyPlayerList[k]);

                                // Player Name Box ����ġ
                                partyPlayerNameBoxPV.transform.SetParent(lobbyUIManager.playerNameBoxParent);
                                partyPlayerNameBoxPV.GetComponent<RectTransform>().localPosition = Vector2.zero;
                                partyPlayerNameBoxPV.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator Loading(bool startLoading)
    {
        if(startLoading) 
        {
            // Fade ȭ�� Ȱ��ȭ
            loadingFadeAnim.gameObject.SetActive(true);
            loadingFadeAnim.SetBool("FadeOut", false);

            yield return new WaitForSeconds(0.15f);

            // �ε� ȭ�� Ȱ��ȭ
            loadingUI.SetActive(true);

            AudioManager.Instance.bgmAudio.Stop();
        }
        else
        {
            // �ε� ȭ�� ��Ȱ��ȭ
            loadingUI.SetActive(false);
            loadingFadeAnim.SetBool("FadeOut", true);

            yield return new WaitForSeconds(0.15f);

            // Fade ȭ�� ��Ȱ��ȭ
            loadingFadeAnim.gameObject.SetActive(false);
        }
    }

    // �ǽð� Player List �ֽ�ȭ
    public void SetPlayerList()
    {
        // Player List ����
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] == null)
            {
                playerList.Remove(playerList[i]);
            }
        }
    }

    // �ð� ���� �Լ�
    [PunRPC]
    void TimeLimit()
    {
        UIManager.Instance.playTime -= Time.deltaTime;
    }


    // -----------------------------------------------------------------------------------------------


    // ����ȭ�� ���� ���� ��ư
    public void SettingBtn()
    {
        descriptionUI.SetActive(true);
        OperatingBtn();

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Decription Button");

        if (lobbyCanvas.activeInHierarchy)
        {
            lobbyUIManager.interacting = true;
            Debug.Log("");
        }
    }

    // ���� ���� UI ���ǰ��� ���۹� ��ư
    public void OperatingBtn()
    {
        descriptionMainImg.sprite = descriptionImg[0];
        desPageBtn.SetActive(false);

        // �Ϲ� ��ư
        if(lobbyCanvas.activeInHierarchy || inGameCanvas.activeInHierarchy)
        {
            generalBtns.SetActive(true);
        }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Operating Button");
    }

    // ���� ���� UI ���� ���� ���� ��ư
    public void SoundBtn()
    {
        descriptionMainImg.sprite = descriptionImg[1];
        desPageBtn.SetActive(false);
        generalBtns.SetActive(false);

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Sound Button");
    }

    // ���� ���� UI ���� ���� ���� ��ư
    public void GameDesBtn()
    {
        descriptionMainImg.sprite = descriptionImg[2];
        desPageBtn.SetActive(true);
        generalBtns.SetActive(false);

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Game Des Button");
    }

    // ���� ���� UI ���� ���� ���� ������ ��ư
    public void GameDesPageBtn(int num)
    {
        if (num == 1) { descriptionMainImg.sprite = descriptionImg[2]; }
        else if (num == 2) { descriptionMainImg.sprite = descriptionImg[3]; }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Game Des Page Button");
    }

    // ���� ���� �� Lobby �� ���ư��� ��ư ���� -> UI ������ UIManager ��ũ��Ʈ�� ����
    public void BackToLobby()
    {
        roomName = "Lobby";

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Back to Lobby Button");

        UIManager.Instance.CloseAllUI();
        inGameCanvas.SetActive(false);

        PhotonNetwork.LeaveRoom();

        // �ε� ȭ�� Ȱ��ȭ
        StartCoroutine("Loading", true);
    }

    // ���� ���� ��ư
    public void ExitGameButton()
    {
        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Exit Gmae Button");

        Application.Quit();
    }

    // ���� ���� �� Lobby �� ���ư��� ��ư ��Ȯ�� UI
    public void OpenBackToLobbyUI()
    {
        reCheckUI[0].SetActive(true);

        if(inGameCanvas.activeInHierarchy)
        {
            inGameUIManager.interacting = true;
        }
        else if(lobbyCanvas.activeInHierarchy)
        {
            lobbyUIManager.interacting = true;
        }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Open Back To Lobby Button");
    }

    // ���� ���� �� ���� ���� ��ư ��Ȯ�� UI
    public void OpenExitGameUI()
    {
        reCheckUI[1].SetActive(true);

        if (inGameCanvas.activeInHierarchy)
        {
            inGameUIManager.interacting = true;
        }
        else if (lobbyCanvas.activeInHierarchy)
        {
            lobbyUIManager.interacting = true;
        }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Open Exit Button");
    }

    // ��Ȯ�� UI �ݱ� ��ư
    public void CloseReCheckUI()
    {
        reCheckUI[0].SetActive(false);
        reCheckUI[1].SetActive(false);

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("CloseReCheckUI Button");
    }
}


