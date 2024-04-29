using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
