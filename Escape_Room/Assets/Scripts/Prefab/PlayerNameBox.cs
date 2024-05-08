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

    private void Start()
    {
        photonManager.myPlayerName = GetComponent<PlayerNameBox>();

        this.transform.SetParent(lobbyUIManager.playerNameBoxParent);

        if (pv.IsMine)
        {
            pv.RPC("AddMyName", RpcTarget.AllBuffered, photonManager.masterName);
        }

        this.gameObject.SetActive(false);
    }

    // 플레이어 NameBox List 에 추가
    [PunRPC]
    void AddMyName(string name)
    {
        playerNameText.text = name;

        if (!lobbyUIManager.playerNameBoxList.Contains(this.gameObject))
        {
            lobbyUIManager.playerNameBoxList.Add(this.gameObject);
        }
    }
}
