using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [Header("# UI Boxs")]
    public GameObject[] activeUIBoxs;

    [Header("# Text")]
    [SerializeField] TextMeshProUGUI setPeopleNum;

    [Header("# Value")]
    string masterName; // 파티장 이름
    string theme; // 테마
    int peopleNum = 1;
    [SerializeField] List<GameObject> partyList; // 생성된 리스트들 저장
    [SerializeField] RectTransform[] partyListPos; // 리스트들 Position 값

    public PhotonManager photonManager;

    private void Awake()
    {
        setPeopleNum.text = peopleNum.ToString();
        partyList = new List<GameObject>();

        // 테스트용 
        masterName = "백민지";
        theme = "복현동";
    }

    private void Update()
    {
        // 켜져있는 오브젝트 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIBoxs)
            {
                if (obj.activeInHierarchy) { obj.SetActive(false); }
            }
        }

        // 파티 매칭 UI 열기
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!activeUIBoxs[0].activeInHierarchy)
            {
                activeUIBoxs[0].SetActive(true);
            }
        }

        // 만들어진 방 list 들 position 값 조정
        if(partyList.Count > 0)
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
            activeUIBoxs[1].SetActive(false);
            GameObject party = photonManager.MakePartyRoom();
            partyList.Add(party); // 리스트에 추가

            // 값 세팅
            party.transform.SetParent(activeUIBoxs[0].transform); // 부모 설정

            PartyList partyListLogic = party.GetComponent<PartyList>();
            partyListLogic.masterName.text = masterName;
            partyListLogic.theme.text = theme;
            partyListLogic.peopleNum.text = peopleNum.ToString();
        }
    }

    // 만들어진 방 list 들 위치 조정
    void SetListPos()
    {
        for(int i = 0; i < partyList.Count; i++)
        {
            int index = i % 8;

            RectTransform partyRectPos = partyList[i].GetComponent<RectTransform>();

            partyRectPos.anchorMin = partyListPos[index].anchorMin;
            partyRectPos.anchorMax = partyListPos[index].anchorMax;

            partyRectPos.offsetMin = Vector2.zero;
            partyRectPos.offsetMax = Vector2.zero;
        }
    }


    // 방 만들때 인원 수 정하는 UI 버튼
    public void SetPeopleNum(string set)
    {
        if (set == "Up")
        {
            if (peopleNum == 5)
            {
                peopleNum = 1;
            }
            else
            {
                peopleNum++;
            }

            setPeopleNum.text = peopleNum.ToString();
        }
        else if (set == "Down")
        {
            if (peopleNum == 1)
            {
                peopleNum = 5;
            }
            else
            {
                peopleNum--;
            }

            setPeopleNum.text = peopleNum.ToString();
        }
    }
}
