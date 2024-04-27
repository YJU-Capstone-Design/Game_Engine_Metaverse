using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks // 제공해주는 다양한 CallBack 함수를 쓸 수 있음.
{
    // 버전
    private readonly string version = "1.0f";

    [Header("# Prefab")]
    public GameObject playerPrefab;
    public GameObject partyListPrefab;
    PhotonView pv;

    [Header("# Player")]
    public string masterName = "Test"; // 사용자 이름
    public PhotonView myPlayer;
    [SerializeField] Vector3 spawnPoint;
    public List<GameObject> playerList;

    [Header("# PartyList Info")]
    public string theme; // 테마
    public string partyPeopleNum; // 파티 생성 시 인원 수 텍스트 (리스트에 사용될 텍스트) -> "현재 인원수 / 최대 인원수" 형식

    [Header("# MakeParty Info")]
    public List<GameObject> partyList; // 생성된 리스트들 저장
    public int maxPeopleNum = 1; // 파티 만들때 설정하는 인원 수
    public int partyPageLength = 1; // 총 파티 페이지 수 -> list / 8 + 1 의 결과값
    public TextMeshProUGUI maxPeopleNumText; // 파티 생성 시 maxPeopleNum 을 담을 UI
    public TextMeshProUGUI pageCountText; // partyPageLength 가 들어갈 TextMeshPro

    private void Awake()
    {
        // 테스트용 
        masterName = "백민지";
        theme = "복현동";
        pv = GetComponent<PhotonView>();

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

        PhotonNetwork.JoinRandomRoom(); // 랜덤 매치메이킹 기능 제공
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        // 룸의 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;              // 최대 접속자 수 : 20명
        roomOptions.IsOpen = true;                // 룸의 오픈 여부
        roomOptions.IsVisible = true;             // 로비에서 룸 목록에 노출 시킬지 여부

        // 룸 생성
        PhotonNetwork.CreateRoom("My Room", roomOptions);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
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
    }

    // 포톤 퇴장 시 실행되는 콜백함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        pv.RPC("LeftPhoton", RpcTarget.All, myPlayer.ViewID); // 본인과 관련된 데이터들 삭제
    }



    // # 인게임 함수

    // 파티 매칭 시스템으로 방(파티) 만들기
    public GameObject MakePartyRoom()
    {
        GameObject partyList = PhotonNetwork.Instantiate(partyListPrefab.name, transform.position, Quaternion.identity);

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


    // 포톤 연결 종료 시 실행될 함수
    [PunRPC]
    public void LeftPhoton(int id)
    {
        // 나가는 사람 확인
        for (int i = 0; i < playerList.Count; i++)
        {
            PhotonView playerPV = playerList[i].GetComponent<PhotonView>();

            if (playerPV.ViewID == id)
            {
                // 플레이어 리스트에서 본인 제거
                playerList.Remove(playerPV.gameObject);

                // 파티 리스트
                for(int j = 0; j < partyList.Count; j++)
                {
                    PartyList partyLogic = partyList[i].GetComponent<PartyList>();

                    // 리스트에서 본인이 만든 파티 제거
                    if (playerPV.IsMine)
                    {
                        partyList.Remove(partyLogic.gameObject);
                    }
                    else if (partyLogic.partyPlayerIDList.Contains(id)) // 자신이 속해 있는 파티에서 본인 제거
                    {
                        partyLogic.partyPlayerIDList.Remove(id);
                    }
                }
            }
        }
    }
}
