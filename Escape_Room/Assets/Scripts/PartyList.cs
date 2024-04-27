using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using static UnityEngine.UI.GridLayoutGroup;

public class PartyList : MonoBehaviour
{
    [Header("# List Info")]
    public TextMeshProUGUI listMasterNameText; // ����Ʈ ������ ���� UI
    public TextMeshProUGUI listThemeText;      // ����Ʈ ���� �׸��� ���� UI
    public TextMeshProUGUI listPeopleNumText;  // ����Ʈ ���� ��Ƽ �ο� �� ������ ���� UI
    public int maxPeopleNum; // ��Ƽ ���� �� ������ �ִ� �ο���
    public List<int> partyPlayerIDList; // ���� ��Ƽ�� �ִ� �÷��̾� ViewID �� ���� ����Ʈ

    [Header("# Components")]
    PhotonView pv;
    LobbyUIManager lobbyUIManager;
    PhotonManager photonManager;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        lobbyUIManager = LobbyUIManager.Instance;
        photonManager = LobbyUIManager.Instance.photonManager;

        if (pv.IsMine) 
        {
            // ����Ʈ ����
            pv.RPC("SetList", RpcTarget.AllBuffered, photonManager.masterName, photonManager.theme, photonManager.partyPeopleNum, photonManager.maxPeopleNum, photonManager.myPlayer.ViewID);
        }
    }

    private void OnEnable()
    {
        this.transform.SetParent(lobbyUIManager.activeUIBoxs[0].transform);

        if(!photonManager.partyList.Contains(this.gameObject))
        {
            photonManager.partyList.Add(this.gameObject);

            // ����Ʈ ���� ��, �ǽð����� ������ �� �� �ؽ�Ʈ ����
            photonManager.partyPageLength = (photonManager.partyList.Count % 8 == 0 ? photonManager.partyList.Count / 8 : photonManager.partyList.Count / 8 + 1);
            photonManager.pageCountText.text = $"{lobbyUIManager.partyPageCount} / {photonManager.partyPageLength}";
        }
    }

    [PunRPC]
    void SetList(string nameText, string themeText, string peopleNumText, int maxPeopleNum, int playerId)
    {
        // �ɼ� ����
        listMasterNameText.text = nameText;
        listThemeText.text = themeText;
        listPeopleNumText.text = peopleNumText;

        // ������ ��Ƽ �ɼ� ����
        this.maxPeopleNum = maxPeopleNum;
        partyPlayerIDList.Add(playerId);

        // ��Ƽ ���� �� �����ߴ� ���� �ʱ�ȭ
        photonManager.maxPeopleNum = 1; // ��Ƽ ���鶧 ������ �ο� �� �ʱ�ȭ
        photonManager.maxPeopleNumText.text = "1";
        photonManager.partyPeopleNum = $"{1} / {photonManager.maxPeopleNum}"; // ��Ƽ ���鶧 ������ ����Ʈ �ο� �� UI �ʱ�ȭ
    }


    // ��Ƽ ���� ��ư
    public void JoinParty(PartyList mainObj)
    {
        bool joined = false;

        foreach (GameObject list in photonManager.partyList)
        {
            PartyList partyLogic = list.GetComponent<PartyList>();

            foreach (int id in partyLogic.partyPlayerIDList)
            {
                if (id == photonManager.myPlayer.ViewID) // ���⼭ photonManager.myPlayer �� ��ư�� ���� ����� �÷��̾�
                {
                    Debug.Log("����� �̹� ��Ƽ�� ���ԵǾ� �ֽ��ϴ�.");
                    joined = true;
                    break;
                }
            }
        }

        if (joined)
            return;

        if(partyPlayerIDList.Count >= maxPeopleNum)
        {
            Debug.Log("�̹� ��Ƽ�� �� á���ϴ�.");
        }
        else if(pv.IsMine)
        {
            Debug.Log("����� �� ��Ƽ�� �����Դϴ�.");
        }
        else if (!pv.IsMine && partyPlayerIDList.Count < maxPeopleNum)
        {
            pv.RPC("SynchronizationPeopleNum", RpcTarget.AllBuffered, photonManager.myPlayer.ViewID, partyPlayerIDList.Count, mainObj.maxPeopleNum, true);
        }
    }

    // ����Ʈ�� ��Ƽ ��� �� �ο� �� �ǽð� ����ȭ �Լ�
    [PunRPC]
    void SynchronizationPeopleNum(int id, int nowPeopleNum, int maxPeoPleNum, bool add)
    {
        if(add)
        {
            partyPlayerIDList.Add(id);
            nowPeopleNum++;
            listPeopleNumText.text = $"{nowPeopleNum} / {maxPeoPleNum}";
        } 
        else if(!add)
        {
            partyPlayerIDList.Remove(id);
            nowPeopleNum--;
            listPeopleNumText.text = $"{nowPeopleNum} / {maxPeoPleNum}";
        }
    }
}
