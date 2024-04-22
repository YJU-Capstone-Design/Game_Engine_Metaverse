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
    string masterName; // ��Ƽ�� �̸�
    string theme; // �׸�
    int peopleNum = 1;
    [SerializeField] List<GameObject> partyList; // ������ ����Ʈ�� ����
    [SerializeField] RectTransform[] partyListPos; // ����Ʈ�� Position ��

    public PhotonManager photonManager;

    private void Awake()
    {
        setPeopleNum.text = peopleNum.ToString();
        partyList = new List<GameObject>();

        // �׽�Ʈ�� 
        masterName = "�����";
        theme = "������";
    }

    private void Update()
    {
        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIBoxs)
            {
                if (obj.activeInHierarchy) { obj.SetActive(false); }
            }
        }

        // ��Ƽ ��Ī UI ����
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!activeUIBoxs[0].activeInHierarchy)
            {
                activeUIBoxs[0].SetActive(true);
            }
        }

        // ������� �� list �� position �� ����
        if(partyList.Count > 0)
        {
            SetListPos();
        }
    }

    public void MakeRoomButton(int index)
    {
        if (index == 0) // ��Ƽ ��Ī â
        {
            activeUIBoxs[0].SetActive(false);
            activeUIBoxs[1].SetActive(true);
        }
        else if (index == 1) // �� ����� â
        {
            activeUIBoxs[1].SetActive(false);
            GameObject party = photonManager.MakePartyRoom();
            partyList.Add(party); // ����Ʈ�� �߰�

            // �� ����
            party.transform.SetParent(activeUIBoxs[0].transform); // �θ� ����

            PartyList partyListLogic = party.GetComponent<PartyList>();
            partyListLogic.masterName.text = masterName;
            partyListLogic.theme.text = theme;
            partyListLogic.peopleNum.text = peopleNum.ToString();
        }
    }

    // ������� �� list �� ��ġ ����
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


    // �� ���鶧 �ο� �� ���ϴ� UI ��ư
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
