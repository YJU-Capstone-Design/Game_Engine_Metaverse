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

public class PhotonManager : MonoBehaviourPunCallbacks // 제공해주는 다양한 CallBack 함수를 쓸 수 있음.
{
    [Header("# Photon")]
    private readonly string version = "1.0f"; // 버전
    string roomName;

    [Header("# Prefab")]
    public GameObject playerPrefab;
    public GameObject partyListPrefab;
    public GameObject playerNameBoxPrefab;
    PhotonView pv;

    [Header("# Player")]
    public string masterName = "Test"; // 사용자 이름
    public PhotonView myPlayer; // 생성된 본인 캐릭터
    public PartyList myParty; // 본인이 생성/가입한 파티
    public PlayerNameBox myPlayerName; // 생성된 본인 이름 Box
    [SerializeField] Vector3 spawnPoint; // 생성된 Player(Character) 가 Spawn 되는 위치
    public List<GameObject> playerList; // 생성된 Player(Character) 가 저장되는 List

    [Header("# PartyList Info")]
    public string theme; // 테마
    public string partyPeopleNum; // 파티 생성 시 인원 수 텍스트 (리스트에 사용될 텍스트) -> "현재 인원수 / 최대 인원수" 형식
    public int myPartyMaxPeople; // 생성된 파티의 최대 인원 수 -> 게임 시작 시 새로 만들 Room 의 옵션에 필요

    [Header("# MakeParty Info")]
    public List<GameObject> partyList; // 생성된 리스트들 저장
    public int maxPeopleNum = 1; // 파티 만들때 설정하는 인원 수
    public int partyPageLength = 1; // 총 파티 페이지 수 -> list / 8 + 1 의 결과값
    public TextMeshProUGUI maxPeopleNumText; // 파티 생성 시 maxPeopleNum 을 담을 UI
    public TextMeshProUGUI pageCountText; // partyPageLength 가 들어갈 TextMeshPro

    [Header("# Common UI")]
    public GameObject loadingUI;
    public Animator loadingFadeAnim;

    private void Awake()
    {
        // 테스트용 
        masterName = "백민지";
        theme = "복현동";
        pv = GetComponent<PhotonView>();

        // 로딩 화면 활성화
        loadingFadeAnim.gameObject.SetActive(true);
        loadingFadeAnim.SetBool("FadeOut", false);
        loadingUI.SetActive(true);

        // Room 기본값 세팅
        roomName = "My Room";
        myPartyMaxPeople = 20;

        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;

        // 같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;

        // 유저 아이디 할당
        PhotonNetwork.NickName = masterName;

        // 포톤 서버와 통신 횟수 설정 -> 기본값은 초당 30회
        Debug.Log(PhotonNetwork.SendRate);

        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();

        // # 플레이어 리스트 초기화
        playerList = new List<GameObject>();

        // # 파티 시스템 초기화
        partyList = new List<GameObject>();
        partyPeopleNum = "1 / 1";
        maxPeopleNumText.text = "1";
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // 로비 접속 여부 true , false 출력 -> 접속 전이라서 false
        // $"" => string.Format() 를 줄인것 -> ""안에 있는 내용을 문자열로 반환함. 

        PhotonNetwork.JoinLobby(); // 로비 입장
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // 로비 접속 여부 true , false 출력 -> 접속 후라서 true

        Debug.Log(roomName);

        // 룸 참가
        PhotonNetwork.JoinRoom(roomName);

        //PhotonNetwork.JoinRandomRoom(); // 랜덤 매치메이킹 기능 제공
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    Debug.Log($"JoinRandom Failed {returnCode}:{message}");

    //    // 룸의 속성 정의
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = 20;              // 최대 접속자 수 : 20명
    //    roomOptions.IsOpen = true;                // 룸의 오픈 여부
    //    roomOptions.IsVisible = true;             // 로비에서 룸 목록에 노출 시킬지 여부

    //    // 룸 생성
    //    PhotonNetwork.CreateRoom(roomName, roomOptions);
    //}

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"CreateRoom Failed {returnCode}:{message}");

        // 룸 참가
        PhotonNetwork.JoinRoom(roomName);
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 룸에 접속한 사용자 정보 확인
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}"); // 이름과 고유값 출력
        }

        // 캐릭터 생성
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);

        // Player Name Box 생성
        PhotonNetwork.Instantiate(playerNameBoxPrefab.name, LobbyUIManager.Instance.playerNameBoxParent.transform.position, Quaternion.identity);

        // 로딩 화면 비활성화
        loadingUI.SetActive(false);
        loadingFadeAnim.SetBool("FadeOut", true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRoom Failed {returnCode}:{message}");

        // 룸의 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;              // 최대 접속자 수 : 20
        roomOptions.IsOpen = true;                // 룸의 오픈 여부
        roomOptions.IsVisible = true;             // 로비에서 룸 목록에 노출 시킬지 여부

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    // Room 을 떠났을 때 실행되는 콜백함수
    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby();
    }

    // 포톤 퇴장 시 실행되는 콜백함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        pv.RPC("LeftPhoton", RpcTarget.All, otherPlayer.ActorNumber, true); // 본인과 관련된 데이터들 삭제
    }



    // -----------------------------------------------------------------------------------



    // # 인게임 함수

    // 파티 매칭 시스템으로 방(파티) 만들기
    public GameObject MakePartyRoom()
    {
        GameObject partyList = PhotonNetwork.Instantiate(partyListPrefab.name, transform.position, Quaternion.identity);
        myPartyMaxPeople = maxPeopleNum;

        return partyList;
    }

    // 방 만들때 인원 수 정하는 UI 버튼
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

        maxPeopleNumText.text = maxPeopleNum.ToString(); // 파티 인원 수를 설정할 때 보이는 UI
        partyPeopleNum = $"{1} / {maxPeopleNum}"; // 생성될 list 에 대입시켜줄 값을 저장하는 문자열 -> 1 은 기본값
    }

    // 파티 탈퇴 버튼
    public void LeftParty()
    {
        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, false); // 본인과 관련된 데이터들 삭제

        // 본인  mini 파티 UI 비활성화
        LobbyUIManager.Instance.miniPartyUI.SetActive(false);

        // myParty 초기화
        myParty = null;
    }

    public void GameStartButton()
    {
        roomName = myPlayer.ViewID.ToString(); // 새로 만들 Room 이름 설정

        pv.RPC("GameStart", RpcTarget.All, this.roomName);
    }

    [PunRPC]
    // 게임 시작 버튼
    public void GameStart(string roomName)
    {
        // 자신의 파티에 게임을 시작한 사람이 있을 경우에만 작동
        if (myParty.partyPlayerIDList.Contains(int.Parse(roomName)))
        {
            Debug.Log("Aaa");

            this.roomName = roomName; // 참가할 Room 이름 설정

            pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID / 1000, true); // 본인과 관련된 데이터들 삭제

            PhotonNetwork.LeaveRoom();

            // 로딩 화면 활성화
            loadingFadeAnim.SetBool("FadeOut", false);
            loadingUI.SetActive(true);

            foreach (GameObject lobbyUI in LobbyUIManager.Instance.activeUIBoxs)
            {
                if (lobbyUI.activeInHierarchy)
                {
                    lobbyUI.SetActive(false);
                }
            }

            // 본인 mini 파티 UI 비활성화
            LobbyUIManager.Instance.miniPartyUI.SetActive(false);

            // List 들 초기화
            LobbyUIManager.Instance.partyPlayerList.Clear();
            partyList.Clear();
        }
    }


    // 포톤 연결 종료 또는 파티 탈퇴 시 실행될 함수
    [PunRPC]
    public void LeftPhoton(int ActorNum, bool leftPhoton)
    {
        // 나가는 사람 확인
        for (int i = 0; i < playerList.Count; i++)
        {
            PhotonView playerPV = playerList[i].GetComponent<PhotonView>();

            // 포톤 서버 종료 시에만 플레이어 리스트와 Scene에서 본인 Character 제거
            if (playerPV.ViewID / 1000 == ActorNum && leftPhoton)
            {
                // Player Name Box 를 List 에서 삭제
                for(int j = 0; j < LobbyUIManager.Instance.playerNameBoxList.Count; j++)
                {
                    PhotonView playerNamePV = LobbyUIManager.Instance.playerNameBoxList[j].GetComponent<PhotonView>();

                    if(playerNamePV.ViewID / 1000 == ActorNum)
                    {
                        LobbyUIManager.Instance.playerNameBoxList.Remove(LobbyUIManager.Instance.playerNameBoxList[j]);
                    }
                }
                
                playerList.Remove(playerPV.gameObject);
                Destroy(playerPV.gameObject);
            }

            // 파티 리스트
            for (int j = 0; j < partyList.Count; j++)
            {
                PartyList partyLogic = partyList[j].GetComponent<PartyList>();
                PhotonView partyPV = partyList[j].GetComponent<PhotonView>();

                // 자신이 속해 있는 파티 찾기
                if (partyLogic.partyPlayerIDList.Contains(playerPV.ViewID) && playerPV.ViewID / 1000 == ActorNum)
                {
                    // 인원 UI 실시간 적용
                    partyPV.RPC("SynchronizationPeopleNum", RpcTarget.AllBuffered, playerPV.ViewID, partyLogic.partyPlayerIDList.Count, partyLogic.maxPeopleNum, false);

                    // 본인이 파티장일 때에는 팀원 및 본인에게서 모든 데이터 제거
                    if (partyPV.ViewID / 1000 == ActorNum)
                    {
                        for (int k = partyLogic.partyPlayerIDList.Count - 1; k > -1; k--)
                        {
                            for(int l = 0; l < LobbyUIManager.Instance.playerNameBoxList.Count; l++)
                            {
                                PhotonView playerNamePV = LobbyUIManager.Instance.playerNameBoxList[l].GetComponent<PhotonView>();

                                if(playerNamePV.ViewID/1000 == partyLogic.partyPlayerIDList[k]/1000 && playerNamePV.IsMine)
                                {
                                    // 파티원들 및 본인 Player Name Box 원위치
                                    playerNamePV.transform.SetParent(LobbyUIManager.Instance.playerNameBoxParent);
                                    playerNamePV.GetComponent<RectTransform>().localPosition = Vector2.zero;

                                    // 파티원들 및 본인 Photon Manager 의 myParty 초기화
                                    playerNamePV.GetComponent<PlayerNameBox>().lobbyUIManager.photonManager.myParty = null;

                                    // 파티원들 및 본인 mini 파티 UI 비활성화
                                    playerNamePV.GetComponent<PlayerNameBox>().lobbyUIManager.miniPartyUI.SetActive(false);

                                    // Player Name Box 비활성화
                                    playerNamePV.gameObject.SetActive(false);
                                }
                            }
                        }

                        // 리스트와 Scene 에서 본인이 만든 파티 제거
                        Destroy(partyList[j].gameObject);
                        partyList.Remove(partyList[j]);
                    }
                    else // 본인이 파티장이 아닐 경우에는 본인의 데이터만 삭제
                    {
                        // 팀원들 mini 파티 UI 에서 본인 PlayerNameBox 만 playerNameBoxParent 자식 오브젝트로 원위치
                        for (int k = 0; k < partyLogic.lobbyUIManager.partyPlayerList.Count; k++)
                        {
                            PhotonView partyPlayerNameBoxPV = partyLogic.lobbyUIManager.partyPlayerList[k].GetComponent<PhotonView>();

                            if (partyPlayerNameBoxPV.ViewID / 1000 == ActorNum)
                            {
                                // Player Name Box 원위치
                                partyPlayerNameBoxPV.transform.SetParent(LobbyUIManager.Instance.playerNameBoxParent);
                                partyPlayerNameBoxPV.GetComponent<RectTransform>().localPosition = Vector2.zero;
                                partyPlayerNameBoxPV.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
