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
        }

        // °ª ¼¼ÆÃ
        masterName.text = LobbyUIManager.Instance.photonManager.masterName;
        theme.text = LobbyUIManager.Instance.photonManager.theme;
        peopleNum.text = LobbyUIManager.Instance.photonManager.peopleNum.ToString();
    }
}
