using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.ComponentModel;
using Photon.Realtime;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [Header("# UI Boxs")]
    public GameObject[] activeUIBoxs;
    [SerializeField] RectTransform[] partyListPos; // ��Ƽ ����Ʈ�� Position ��
    public GameObject miniPartyUI; // ��Ƽ ����/���� �� ȭ�鿡 ǥ�õ� mini ��Ƽ UI
    public TextMeshProUGUI miniPartyUITitle; // mini ��Ƽ UI Ÿ��Ʋ �ؽ�Ʈ
    public GameObject partyPlayerListParent;
    public List<GameObject> partyPlayerList; // ������ ��Ƽ �÷��̾� NameBox �� ��� List
    public RectTransform[] partyPlayerListPos; // ���� ��Ƽ�� �÷��̾� �̸��� �� �� Text ��

    [Header("# Party System")]
    public int partyPageCount = 1; // ���� ������ ��ġ

    public PhotonManager photonManager;
    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        partyPlayerList = new List<GameObject>();

        // ������ �� ����ȭ
        CheckPartyPageLength();

        foreach(GameObject obj in activeUIBoxs)
        {
            obj.SetActive(false);
        }
        miniPartyUI.SetActive(false);
    }

    private void Update()
    {
        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIBoxs)
            {
                if (obj.activeInHierarchy) { obj.SetActive(false); partyPageCount = 1; }
                photonManager.maxPeopleNum = 1;
                photonManager.maxPeopleNumText.text = "1";
            }
        }

        // ��Ƽ ��Ī UI ����
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!activeUIBoxs[0].activeInHierarchy && !activeUIBoxs[1].activeInHierarchy)
            {
                activeUIBoxs[0].SetActive(true);
                activeUIBoxs[2].SetActive(true);
                partyPageCount = 1;
                SetActivePartyList(); // 1 ������ ����Ʈ�� Ȱ��ȭ

                // ������ �� ����ȭ
                CheckPartyPageLength();
            }
        }

        // ������� �� list �� position �� ����
        if (photonManager.partyList.Count > 0)
        {
            SetListPos();
        }

        // ������ PartyPlayerList Position �� ����
        if(partyPlayerList.Count > 0)
        {
            SetPlayerListPos();
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
            // �ѻ���� ��Ƽ�� �ϳ��� ���� ����
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
                        if (id == photonManager.myPlayer.ViewID) // ���⼭ photonManager.myPlayer �� ��ư�� ���� ����� �÷��̾�
                        {
                            Debug.Log("����� �̹� ��Ƽ�� ���ԵǾ� �ֽ��ϴ�.");
                            joinedParty = true;
                            break;
                        }
                    }

                    if (joinedParty)
                        break;

                    if (i == photonManager.partyList.Count - 1)
                    {
                        Debug.Log("��Ƽ ����");
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
        photonManager.MakePartyRoom(); // ����Ʈ ����� �Լ� ȣ��
        SetActivePartyList(); // ����Ʈ Ȱ��ȭ ����
        
        // ������ �� ����ȭ
        CheckPartyPageLength();

        // mini ��Ƽ UI Ȱ��ȭ
        miniPartyUI.SetActive(true);

        // mini ��Ƽ UI �� �� Player Name Box ����
        PhotonNetwork.Instantiate(photonManager.playerNameBoxPrefab.name, transform.position, Quaternion.identity);
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

    // ������ mini ��Ƽ UI �� partyPlayerList �� ��ġ ����
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


    // ��Ƽ ��Ī �ý��� ������ ��ư
    public void PartyPageButton(string dir)
    {
        if (dir == "Right") { partyPageCount = (partyPageCount == photonManager.partyPageLength ? photonManager.partyPageLength : ++partyPageCount); }
        else if(dir == "Left") { partyPageCount = (partyPageCount == 1 ? 1 : --partyPageCount); }

        // ������ �� ����ȭ
        CheckPartyPageLength();

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

    // ������ �� ����ȭ
    void CheckPartyPageLength()
    {
        photonManager.partyPageLength = (photonManager.partyList.Count % 8 == 0 ? photonManager.partyList.Count / 8 : photonManager.partyList.Count / 8 + 1);
        if(photonManager.partyPageLength == 0) { photonManager.partyPageLength = 1; }
        photonManager.pageCountText.text = $"{partyPageCount} / {photonManager.partyPageLength}";
    }

    // ���� ���� ��ư
    public void ExitGameButton()
    {
        Application.Quit();
    }
}
