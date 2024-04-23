using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PartyList : MonoBehaviour
{
    public TextMeshProUGUI masterName;
    public TextMeshProUGUI theme;
    public TextMeshProUGUI peopleNum;

    [PunRPC]
    private void OnEnable()
    {
        this.transform.SetParent(LobbyUIManager.Instance.activeUIBoxs[0].transform);

        if(!LobbyUIManager.Instance.photonManager.partyList.Contains(this.gameObject))
        {
            LobbyUIManager.Instance.photonManager.partyList.Add(this.gameObject);

            // ������ �� �� �ؽ�Ʈ ����
            LobbyUIManager.Instance.photonManager.partyPageLength = (LobbyUIManager.Instance.photonManager.partyList.Count % 8 == 0 ? LobbyUIManager.Instance.photonManager.partyList.Count / 8 : LobbyUIManager.Instance.photonManager.partyList.Count / 8 + 1);
            LobbyUIManager.Instance.photonManager.pageCountText.text = $"{LobbyUIManager.Instance.partyPageCount} / {LobbyUIManager.Instance.photonManager.partyPageLength}";
        }
    }

    [PunRPC]
    private void Start()
    {
        // �� ����
        masterName.text = LobbyUIManager.Instance.photonManager.masterName;
        theme.text = LobbyUIManager.Instance.photonManager.theme;
        peopleNum.text = LobbyUIManager.Instance.photonManager.partyPeopleNum;
        Debug.Log(LobbyUIManager.Instance.photonManager.partyPeopleNum);

        // ��Ƽ ���� �� �����ߴ� ���� �ʱ�ȭ
        LobbyUIManager.Instance.photonManager.peopleNum = 1; // ��Ƽ ���鶧 ������ �ο� �� �ʱ�ȭ
        LobbyUIManager.Instance.photonManager.setPeopleNumText.text = LobbyUIManager.Instance.photonManager.peopleNum.ToString(); // ��Ƽ ���鶧 ������ ��Ƽ ���� �ο� �� UI �ʱ�ȭ
        LobbyUIManager.Instance.photonManager.partyPeopleNum = $"{1} / {LobbyUIManager.Instance.photonManager.peopleNum}"; // ��Ƽ ���鶧 ������ ����Ʈ �ο� �� UI �ʱ�ȭ
    }
}
