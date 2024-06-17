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
    [SerializeField] RectTransform[] partyListPos; // ��Ƽ ����Ʈ�� Position ��
    public List<GameObject> playerNameBoxList;
    public Transform playerNameBoxParent;
    public bool interacting = false;

    [Header("# mini Party UI")]
    public GameObject miniPartyUI; // ��Ƽ ����/���� �� ȭ�鿡 ǥ�õ� mini ��Ƽ UI
    public TextMeshProUGUI miniPartyUITitle; // mini ��Ƽ UI Ÿ��Ʋ �ؽ�Ʈ
    public GameObject partyPlayerListParent; // mini ��Ƽ UI Player Name �θ� ������Ʈ
    public List<GameObject> partyPlayerList; // mini ��Ƽ UI�� ������ ��Ƽ Player NameBox �� ��� List
    public RectTransform[] partyPlayerListPos; // mini ��Ƽ UI�� ���� ��Ƽ�� �÷��̾� �̸��� ���� ������ RectTransform ��
    public GameObject gameStartButton; // mini ��Ƽ UI�� ���� ���� ��ư

    [Header("# Party System")]
    public int partyPageCount = 1; // ���� ������ ��ġ

    public PhotonManager photonManager;
    PhotonView pv;
    public AudioManager audioManager;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        partyPlayerList = new List<GameObject>();
        playerNameBoxList = new List<GameObject>();

        // ������ �� ����ȭ
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
        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllUI();

            interacting = false;
        }

        // playerNameBoxList �ǽð� �ֽ�ȭ 
        for(int i = 0; i < playerNameBoxList.Count; i++)
        {
            if (playerNameBoxList[i] == null) { playerNameBoxList.Remove(playerNameBoxList[i]); }
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

            interacting = true;
        }

        // ������� �� list �� position �� ����
        if (photonManager.partyList.Count > 0)
        {
            SetListPos();
        }

        // ������ mini ��Ƽ UI �� PartyPlayerList Position �� ����
        if(partyPlayerList.Count > 0)
        {
            SetPlayerListPos();
        }

        // mini ��Ƽ UI �ο� �� �ǽð� �ֽ�ȭ
        if(photonManager.myParty != null)
        {
            SynchronizationPartyPeopleNum(photonManager.myParty.listPeopleNumText.text);
        }

        // mini ��Ƽ UI ��� �ǽð� �ֽ�ȭ
        if (photonManager.myParty)
        {
            SetMiniPartyPlayers();
        }
        else // ��Ƽ�� ���Ե��� ���� ���
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

        photonManager.MakePartyRoom(); // ����Ʈ ����� �Լ� ȣ��
        SetActivePartyList(); // ����Ʈ Ȱ��ȭ ����
        
        // ������ �� ����ȭ
        CheckPartyPageLength();

        // mini ��Ƽ UI �� ���� ���� ��ư Ȱ��ȭ
        miniPartyUI.SetActive(true);
        gameStartButton.SetActive(true);

        // Player NameBox �� mini ��Ƽ UI �� ��������
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

    // mini ��Ƽ UI Title �÷��̾� �ο� �� �ǽð� ����ȭ �Լ�
    void SynchronizationPartyPeopleNum(string peopleNum)
    {
        miniPartyUITitle.text = $"��Ƽ ���� ��... {peopleNum}";
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

    // mini ��Ƽ UI ��� �ǽð� �ֽ�ȭ �Լ�
    void SetMiniPartyPlayers()
    {
        for (int i = 0; i < photonManager.myParty.partyPlayerIDList.Count; i++)
        {
            // mini ��Ƽ UI �� PlayerNameBox �߰�
            for (int j = 0; j < photonManager.myParty.partyPlayerIDList.Count; j++)
            {
                foreach (GameObject playerName in playerNameBoxList)
                {
                    PhotonView playerNamePV = playerName.GetComponent<PhotonView>();

                    // partyPlayerIDList �� ������ ���� ���
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


    // ��Ƽ ��Ī �ý��� ������ ��ư
    public void PartyPageButton(string dir)
    {
        if (dir == "Right") { partyPageCount = (partyPageCount == photonManager.partyPageLength ? photonManager.partyPageLength : ++partyPageCount); }
        else if(dir == "Left") { partyPageCount = (partyPageCount == 1 ? 1 : --partyPageCount); }

        // ������ �� ����ȭ
        CheckPartyPageLength();

        // �������� �°� ����Ʈ Ȱ��ȭ
        SetActivePartyList();

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Party Page Button");
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

    // ���� ���� ��Ȯ�� UI Ȱ��ȭ
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
