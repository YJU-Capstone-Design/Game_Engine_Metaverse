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
    [SerializeField] GameObject partyBoxPrefab; // ��Ƽ ��� ����

    [Header("# Text")]
    [SerializeField] TextMeshProUGUI setPeopleNum;

    [Header("# Value")]
    string masterName; // ��Ƽ�� �̸�
    string theme; // �׸�
    int peopleNum = 1;
    [SerializeField] List<GameObject> partyList; // ������ ����Ʈ�� ����
    [SerializeField] RectTransform[] partyListPos; // ����Ʈ�� Position ��
   


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

        SetListPos();
    }

    public void MakeRoom(int index)
    {
        if (index == 0) // ��Ƽ ��Ī â
        {
            activeUIBoxs[0].SetActive(false);
            activeUIBoxs[1].SetActive(true);
        }
        else if (index == 1) // �� ����� â
        {
            activeUIBoxs[1].SetActive(false);
            GameObject party = PhotonNetwork.Instantiate(partyBoxPrefab.name, transform.position, Quaternion.identity);
            partyList.Add(party); // ����Ʈ�� �߰�

            // �� ����
            party.transform.SetParent(activeUIBoxs[0].transform); // ���� ��Ƽ ����� ��Ƽ ��Ī â�� �ڽ� ������Ʈ�� ���� 

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
