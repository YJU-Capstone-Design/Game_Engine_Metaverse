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

            // 페이지 수 및 텍스트 변경
            LobbyUIManager.Instance.photonManager.partyPageLength = (LobbyUIManager.Instance.photonManager.partyList.Count % 8 == 0 ? LobbyUIManager.Instance.photonManager.partyList.Count / 8 : LobbyUIManager.Instance.photonManager.partyList.Count / 8 + 1);
            LobbyUIManager.Instance.photonManager.pageCountText.text = $"{LobbyUIManager.Instance.partyPageCount} / {LobbyUIManager.Instance.photonManager.partyPageLength}";
        }
    }

    [PunRPC]
    private void Start()
    {
        // 값 세팅
        masterName.text = LobbyUIManager.Instance.photonManager.masterName;
        theme.text = LobbyUIManager.Instance.photonManager.theme;
        peopleNum.text = LobbyUIManager.Instance.photonManager.partyPeopleNum; // 1 은 나중에 파티 기능 생기면 변경
    }
}
