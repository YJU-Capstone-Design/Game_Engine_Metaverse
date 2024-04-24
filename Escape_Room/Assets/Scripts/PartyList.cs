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
    public TextMeshProUGUI masterName;
    public TextMeshProUGUI theme;
    public TextMeshProUGUI peopleNum;

    PhotonView pv;
    LobbyUIManager lobbyUIManager;
    PhotonManager photonManager;

    public List<int> partyPlayerIDList;
    public int partyPlayerCountMax;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        lobbyUIManager = LobbyUIManager.Instance;
        photonManager = LobbyUIManager.Instance.photonManager;

        if (pv.IsMine) 
        {
            pv.RPC("SetList", RpcTarget.AllBuffered, photonManager.masterName, photonManager.theme, photonManager.partyPeopleNum, photonManager.peopleNum, photonManager.myPlayer.ViewID);
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
    void SetList(string nameText, string themeText, string peopleNumText, int setPeopleNum, int playerId)
    {
        // �ɼ� ����
        masterName.text = nameText;
        theme.text = themeText;
        peopleNum.text = peopleNumText;

        // ������ ��Ƽ �ɼ� ����
        partyPlayerCountMax = setPeopleNum;
        partyPlayerIDList.Add(playerId);

        // ��Ƽ ���� �� �����ߴ� ���� �ʱ�ȭ
        photonManager.peopleNum = 1; // ��Ƽ ���鶧 ������ �ο� �� �ʱ�ȭ
        photonManager.setPeopleNumText.text = photonManager.peopleNum.ToString(); // ��Ƽ ���鶧 ������ ��Ƽ ���� �ο� �� UI �ʱ�ȭ
        photonManager.partyPeopleNum = $"{1} / {photonManager.peopleNum}"; // ��Ƽ ���鶧 ������ ����Ʈ �ο� �� UI �ʱ�ȭ
    }


    // ��Ƽ ���� ��ư
    [PunRPC]
    public void JoinParty()
    {
        bool joined = false;

        foreach(GameObject list in photonManager.partyList)
        {
            PartyList partyLogic = list.GetComponent<PartyList>();

            foreach(int id in partyLogic.partyPlayerIDList)
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

        if(!pv.IsMine && partyPlayerIDList.Count < partyPlayerCountMax)
        {
            partyPlayerIDList.Add(photonManager.myPlayer.ViewID); // ���⼭ photonManager.myPlayer �� ��ư�� ���� ����� �÷��̾�
        }
        else if(partyPlayerIDList.Count >= partyPlayerCountMax)
        {
            Debug.Log("�̹� ��Ƽ�� �� á���ϴ�.");
        }
        else if(pv.IsMine)
        {
            Debug.Log("����� �� ��Ƽ�� �����Դϴ�.");
        }
    }
}
