using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : PhotonManager
{
    [Header("# UI Boxs")]
    [SerializeField] GameObject[] activeUIBoxs;

    [Header("# Prefab")]
    [SerializeField] GameObject partyBoxPrefab; // 파티 목록 상자

    [Header("# Text")]
    [SerializeField] TextMeshProUGUI setPeopleNum;

    [Header("# Value")]
    string masterName; // 파티장 이름
    string theme; // 테마
    int peopleNum = 1;
    [SerializeField] List<GameObject> partyList; // 생성된 리스트들 저장
    [SerializeField] RectTransform[] partyListPos; // 리스트들 Position 값
   


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

        SetListPos();
    }

    public void MakeRoom(int index)
    {
        if (index == 0) // 파티 매칭 창
        {
            activeUIBoxs[0].SetActive(false);
            activeUIBoxs[1].SetActive(true);
        }
        else if (index == 1) // 방 만들기 창
        {
            activeUIBoxs[1].SetActive(false);
            GameObject party = PhotonNetwork.Instantiate(partyBoxPrefab.name, transform.position, Quaternion.identity);
            partyList.Add(party); // 리스트에 추가

            // 값 세팅
            party.transform.SetParent(activeUIBoxs[0].transform); // 만든 파티 목록을 파티 매칭 창의 자식 오브젝트로 설정 

            PartyList partyListLogic = party.GetComponent<PartyList>();
            partyListLogic.masterName.text = masterName;
            partyListLogic.theme.text = theme;
            partyListLogic.peopleNum.text = peopleNum.ToString();
        }
    }

    [PunRPC]
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
