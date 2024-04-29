using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerNameBox : MonoBehaviour
{
    [Header("# Components")]
    PhotonView pv;
    TextMeshProUGUI playerNameText;
    public LobbyUIManager lobbyUIManager;
    PhotonManager photonManager;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerNameText = GetComponent<TextMeshProUGUI>();
        lobbyUIManager = LobbyUIManager.Instance;
        photonManager = LobbyUIManager.Instance.photonManager;
    }

    private void OnEnable()
    {
        this.transform.SetParent(lobbyUIManager.partyPlayerListParent.transform);

        if (pv.IsMine)
        {
            pv.RPC("AddMyName", RpcTarget.AllBuffered, photonManager.masterName);
        }
    }

    // mini 파티 UI 플레이어 List 에 추가 및 타이틀 인원 실시간 변경
    [PunRPC]
    void AddMyName(string name)
    {
        playerNameText.text = name;

        if (!lobbyUIManager.partyPlayerList.Contains(this.gameObject))
        {
            lobbyUIManager.partyPlayerList.Add(this.gameObject);
        }
    }
}
