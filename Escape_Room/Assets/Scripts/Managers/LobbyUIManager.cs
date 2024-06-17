using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.ComponentModel;
using Photon.Realtime;
using System.Linq;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [Header("# UI Boxs")]
    public GameObject[] activeUIBoxs;
    [SerializeField] RectTransform[] partyListPos; // 파티 리스트들 Position 값
    public List<GameObject> playerNameBoxList;
    public Transform playerNameBoxParent;
    public bool interacting = false;

    [Header("# mini Party UI")]
    public GameObject miniPartyUI; // 파티 생성/참여 후 화면에 표시될 mini 파티 UI
    public TextMeshProUGUI miniPartyUITitle; // mini 파티 UI 타이틀 텍스트
    public GameObject partyPlayerListParent; // mini 파티 UI Player Name 부모 오브젝트
    public List<GameObject> partyPlayerList; // mini 파티 UI에 생성된 파티 Player NameBox 가 담길 List
    public RectTransform[] partyPlayerListPos; // mini 파티 UI의 현재 파티의 플레이어 이름이 가질 설정된 RectTransform 값
    public GameObject gameStartButton; // mini 파티 UI의 게임 시작 버튼

    [Header("# Party System")]
    public int partyPageCount = 1; // 현재 페이지 위치

    public PhotonManager photonManager;
    PhotonView pv;
    public AudioManager audioManager;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        partyPlayerList = new List<GameObject>();
        playerNameBoxList = new List<GameObject>();

        // 페이지 수 동기화
        CheckPartyPageLength();

        foreach(GameObject obj in activeUIBoxs)
        {
            obj.SetActive(false);
        }
        miniPartyUI.SetActive(false);
        gameStartButton.SetActive(false);
    }

    private void Update()
    {
        // 켜져있는 오브젝트 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllUI();

            interacting = false;
        }

        // playerNameBoxList 실시간 최신화 
        for(int i = 0; i < playerNameBoxList.Count; i++)
        {
            if (playerNameBoxList[i] == null) { playerNameBoxList.Remove(playerNameBoxList[i]); }
        }

        // 파티 매칭 UI 열기
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!activeUIBoxs[0].activeInHierarchy && !activeUIBoxs[1].activeInHierarchy)
            {
                activeUIBoxs[0].SetActive(true);
                activeUIBoxs[2].SetActive(true);
                partyPageCount = 1;
                SetActivePartyList(); // 1 페이지 리스트들 활성화

                // 페이지 수 동기화
                CheckPartyPageLength();
            }

            interacting = true;
        }

        // 만들어진 방 list 들 position 값 조정
        if (photonManager.partyList.Count > 0)
        {
            SetListPos();
        }

        // 생성된 mini 파티 UI 의 PartyPlayerList Position 값 조정
        if(partyPlayerList.Count > 0)
        {
            SetPlayerListPos();
        }

        // mini 파티 UI 인원 수 실시간 최신화
        if(photonManager.myParty != null)
        {
            SynchronizationPartyPeopleNum(photonManager.myParty.listPeopleNumText.text);
        }

        // mini 파티 UI 멤버 실시간 최신화
        if (photonManager.myParty)
        {
            SetMiniPartyPlayers();
        }
        else // 파티에 포함되지 않을 경우
        {
            if(partyPlayerList.Count > 0)
            {
                foreach (GameObject playerName in partyPlayerList)
                {
                    playerName.transform.SetParent(playerNameBoxParent.transform);
                    playerName.gameObject.SetActive(false);
                }
            }

            partyPlayerList.Clear();
        }
    }

    public void MakeRoomButton(int index)
    {
        if (index == 0) // 파티 매칭 창
        {
            activeUIBoxs[0].SetActive(false);
            activeUIBoxs[1].SetActive(true);
        }
        else if (index == 1) // 방 만들기 창
        {
            // 한사람당 파티는 하나만 생성 가능
            if(photonManager.partyList.Count == 0)
            {
                MakeParty();

                interacting = false;
            } 
            else
            {
                bool joinedParty = false;
                for (int i = 0; i < photonManager.partyList.Count; i++)
                {
                    PhotonView listPV = photonManager.partyList[i].GetComponent<PhotonView>();

                    foreach (int id in photonManager.partyList[i].GetComponent<PartyList>().partyPlayerIDList)
                    {
                        if (id == photonManager.myPlayer.ViewID) // 여기서 photonManager.myPlayer 는 버튼을 누른 사람의 플레이어
                        {
                            Debug.Log("당신은 이미 파티에 가입되어 있습니다.");
                            joinedParty = true;
                            break;
                        }
                    }

                    if (joinedParty)
                        break;

                    if (i == photonManager.partyList.Count - 1)
                    {
                        Debug.Log("파티 생성");
                        MakeParty();
                        break;
                    }
                }
            }
        }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Make Room Button");
    }

    void MakeParty()
    {
        foreach(GameObject lobbyUI in activeUIBoxs)
        {
            if(lobbyUI.activeInHierarchy)
            {
                lobbyUI.SetActive(false);
            }
        }

        photonManager.MakePartyRoom(); // 리스트 만드는 함수 호출
        SetActivePartyList(); // 리스트 활성화 세팅
        
        // 페이지 수 동기화
        CheckPartyPageLength();

        // mini 파티 UI 및 게임 시작 버튼 활성화
        miniPartyUI.SetActive(true);
        gameStartButton.SetActive(true);

        // Player NameBox 를 mini 파티 UI 로 가져오기
        for(int i = 0; i < playerNameBoxList.Count; i++)
        {
            PhotonView playerNameBoxPV = playerNameBoxList[i].GetComponent<PhotonView>();

            if (playerNameBoxPV.IsMine)
            {
                playerNameBoxPV.gameObject.SetActive(true);

                partyPlayerList.Add(playerNameBoxPV.gameObject);

                playerNameBoxPV.transform.SetParent(partyPlayerListParent.transform);
            }
        }
    }

    // mini 파티 UI Title 플레이어 인원 수 실시간 동기화 함수
    void SynchronizationPartyPeopleNum(string peopleNum)
    {
        miniPartyUITitle.text = $"파티 모집 중... {peopleNum}";
    }

    // 만들어진 방 list 들 위치 조정
    void SetListPos()
    {
        for(int i = 0; i < photonManager.partyList.Count; i++)
        {
            int index = i % 8;

            RectTransform partyRectPos = photonManager.partyList[i].GetComponent<RectTransform>();

            partyRectPos.anchorMin = partyListPos[index].anchorMin;
            partyRectPos.anchorMax = partyListPos[index].anchorMax;

            partyRectPos.offsetMin = Vector2.zero;
            partyRectPos.offsetMax = Vector2.zero;
        }
    }

    // 생성된 mini 파티 UI 의 partyPlayerList 들 위치 조정
    public void SetPlayerListPos()
    {
        for (int i = 0; i < partyPlayerList.Count; i++)
        {
            RectTransform partyRectPos = partyPlayerList[i].GetComponent<RectTransform>();

            partyRectPos.anchorMin = partyPlayerListPos[i].anchorMin;
            partyRectPos.anchorMax = partyPlayerListPos[i].anchorMax;

            partyRectPos.offsetMin = Vector2.zero;
            partyRectPos.offsetMax = Vector2.zero;
        }
    }

    // mini 파티 UI 멤버 실시간 최신화 함수
    void SetMiniPartyPlayers()
    {
        for (int i = 0; i < photonManager.myParty.partyPlayerIDList.Count; i++)
        {
            // mini 파티 UI 에 PlayerNameBox 추가
            for (int j = 0; j < photonManager.myParty.partyPlayerIDList.Count; j++)
            {
                foreach (GameObject playerName in playerNameBoxList)
                {
                    PhotonView playerNamePV = playerName.GetComponent<PhotonView>();

                    // partyPlayerIDList 에 정보가 있을 경우
                    if (playerNamePV.ViewID / 1000 == photonManager.myParty.partyPlayerIDList[j] / 1000)
                    {
                        if (!partyPlayerList.Contains(playerNamePV.gameObject))
                        {
                            partyPlayerList.Add(playerNamePV.gameObject);
                        }

                        playerNamePV.transform.SetParent(partyPlayerListParent.transform);
                        playerNamePV.gameObject.SetActive(true);
                    }
                }
            }
        }
    }


    // 파티 매칭 시스템 페이지 버튼
    public void PartyPageButton(string dir)
    {
        if (dir == "Right") { partyPageCount = (partyPageCount == photonManager.partyPageLength ? photonManager.partyPageLength : ++partyPageCount); }
        else if(dir == "Left") { partyPageCount = (partyPageCount == 1 ? 1 : --partyPageCount); }

        // 페이지 수 동기화
        CheckPartyPageLength();

        // 페이지에 맞게 리스트 활성화
        SetActivePartyList();

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Party Page Button");
    }

    // 페이지에 맞게 리스트를 활성화 함수
    void SetActivePartyList()
    {
        for (int i = 0; i < photonManager.partyList.Count; i++)
        {
            if (i >= (partyPageCount - 1) * 8 && i < partyPageCount * 8)
            {
                photonManager.partyList[i].SetActive(true);
            }
            else
            {
                photonManager.partyList[i].SetActive(false);
            }
        }
    }

    // 페이지 수 동기화
    void CheckPartyPageLength()
    {
        photonManager.partyPageLength = (photonManager.partyList.Count % 8 == 0 ? photonManager.partyList.Count / 8 : photonManager.partyList.Count / 8 + 1);
        if(photonManager.partyPageLength == 0) { photonManager.partyPageLength = 1; }
        photonManager.pageCountText.text = $"{partyPageCount} / {photonManager.partyPageLength}";
    }

    // 게임 종료 재확인 UI 활성화
    public void OpenExitGameUI()
    {
        activeUIBoxs[3].SetActive(true);
    }

    public void CloseAllUI()
    {
        foreach (GameObject obj in activeUIBoxs)
        {
            if (obj.activeInHierarchy) { obj.SetActive(false); partyPageCount = 1; }
            photonManager.maxPeopleNum = 1;
            photonManager.maxPeopleNumText.text = "1";
        }
    }
}
