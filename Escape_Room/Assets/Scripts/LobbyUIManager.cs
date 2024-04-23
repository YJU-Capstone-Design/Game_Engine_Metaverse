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
    [SerializeField] RectTransform[] partyListPos; // ��Ƽ ����Ʈ�� Position ��

    [Header("# Party System")]
    [SerializeField] TextMeshProUGUI pageCountText;
    int partyPageLength = 1; // �� ��Ƽ ������ �� -> list / 8 + 1 �� �����
    int partyPageCount = 1; // ���� ������ ��ġ

    public PhotonManager photonManager;

    private void Awake()
    {
        // ��Ƽ UI ������ �� �ʱ�ȭ
        pageCountText.text = $"{partyPageCount} / {partyPageLength}";
    }

    private void Update()
    {
        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIBoxs)
            {
                if (obj.activeInHierarchy) { obj.SetActive(false); partyPageCount = 1; }
            }
        }

        // ��Ƽ ��Ī UI ����
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!activeUIBoxs[0].activeInHierarchy && !activeUIBoxs[1].activeInHierarchy)
            {
                activeUIBoxs[0].SetActive(true);
                partyPageCount = 1;
                SetActivePartyList(); // 1 ������ ����Ʈ�� Ȱ��ȭ

                pageCountText.text = $"{partyPageCount} / {partyPageLength}";
            }
        }

        // ������� �� list �� position �� ����
        if (photonManager.partyList.Count > 0)
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
            photonManager.MakePartyRoom(); // ����Ʈ ����� �Լ� ȣ��
            SetActivePartyList(); // ����Ʈ Ȱ��ȭ ����
            photonManager.peopleNum = 1;

            partyPageLength = (photonManager.partyList.Count % 8 == 0 ? photonManager.partyList.Count / 8 : photonManager.partyList.Count / 8 + 1);
        }
    }

    // ������� �� list �� ��ġ ����
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

    // ��Ƽ ��Ī �ý��� ������ ��ư
    public void PartyPageButton(string dir)
    {
        partyPageLength = (photonManager.partyList.Count % 8 == 0 ? photonManager.partyList.Count / 8 : photonManager.partyList.Count / 8 + 1);
        if (dir == "Right") { partyPageCount = (partyPageCount == partyPageLength ? partyPageLength : ++partyPageCount); }
        else if(dir == "Left") { partyPageCount = (partyPageCount == 1 ? 1 : --partyPageCount); }

        pageCountText.text = $"{partyPageCount} / {partyPageLength}";

        // �������� �°� ����Ʈ Ȱ��ȭ
        SetActivePartyList();
    }

    // �������� �°� ����Ʈ�� Ȱ��ȭ �Լ�
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
}
