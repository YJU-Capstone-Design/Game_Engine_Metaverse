using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.ComponentModel;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [Header("# UI Boxs")]
    public GameObject[] activeUIBoxs;
    [SerializeField] RectTransform[] partyListPos; // 파티 리스트들 Position 값

    [Header("# Party System")]
    public int partyPageCount = 1; // 현재 페이지 위치

    public PhotonManager photonManager;

    private void Awake()
    {
        // 페이지 수 동기화
        CheckPartyPageLength();
    }

    private void Update()
    {
        // 켜져있는 오브젝트 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIBoxs)
            {
                if (obj.activeInHierarchy) { obj.SetActive(false); partyPageCount = 1; }
                photonManager.maxPeopleNum = 1;
                photonManager.maxPeopleNumText.text = "1";
            }
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
        }

        // 만들어진 방 list 들 position 값 조정
        if (photonManager.partyList.Count > 0)
        {
            SetListPos();
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
    }

    void MakeParty()
    {
        activeUIBoxs[1].SetActive(false);
        photonManager.MakePartyRoom(); // 리스트 만드는 함수 호출
        SetActivePartyList(); // 리스트 활성화 세팅
                              // 페이지 수 동기화
        CheckPartyPageLength();
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

    // 파티 매칭 시스템 페이지 버튼
    public void PartyPageButton(string dir)
    {
        if (dir == "Right") { partyPageCount = (partyPageCount == photonManager.partyPageLength ? photonManager.partyPageLength : ++partyPageCount); }
        else if(dir == "Left") { partyPageCount = (partyPageCount == 1 ? 1 : --partyPageCount); }

        // 페이지 수 동기화
        CheckPartyPageLength();

        // 페이지에 맞게 리스트 활성화
        SetActivePartyList();
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

    // 게임 종료 버튼
    public void ExitGameButton()
    {
        Application.Quit();
    }
}
