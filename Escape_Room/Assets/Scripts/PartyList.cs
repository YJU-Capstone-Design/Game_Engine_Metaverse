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

            // 리스트 생성 시, 실시간으로 페이지 수 및 텍스트 변경
            photonManager.partyPageLength = (photonManager.partyList.Count % 8 == 0 ? photonManager.partyList.Count / 8 : photonManager.partyList.Count / 8 + 1);
            photonManager.pageCountText.text = $"{lobbyUIManager.partyPageCount} / {photonManager.partyPageLength}";
        }
    }

    [PunRPC]
    void SetList(string nameText, string themeText, string peopleNumText, int setPeopleNum, int playerId)
    {
        // 옵션 세팅
        masterName.text = nameText;
        theme.text = themeText;
        peopleNum.text = peopleNumText;

        // 생성된 파티 옵션 세팅
        partyPlayerCountMax = setPeopleNum;
        partyPlayerIDList.Add(playerId);

        // 파티 생성 시 설정했던 값들 초기화
        photonManager.peopleNum = 1; // 파티 만들때 설정한 인원 수 초기화
        photonManager.setPeopleNumText.text = photonManager.peopleNum.ToString(); // 파티 만들때 설정한 파티 생성 인원 수 UI 초기화
        photonManager.partyPeopleNum = $"{1} / {photonManager.peopleNum}"; // 파티 만들때 설정한 리스트 인원 수 UI 초기화
    }


    // 파티 참가 버튼
    [PunRPC]
    public void JoinParty()
    {
        bool joined = false;

        foreach(GameObject list in photonManager.partyList)
        {
            PartyList partyLogic = list.GetComponent<PartyList>();

            foreach(int id in partyLogic.partyPlayerIDList)
            {
                if (id == photonManager.myPlayer.ViewID) // 여기서 photonManager.myPlayer 는 버튼을 누른 사람의 플레이어
                {
                    Debug.Log("당신은 이미 파티에 가입되어 있습니다.");
                    joined = true;
                    break;
                }
            }
        }

        if (joined)
            return;

        if(!pv.IsMine && partyPlayerIDList.Count < partyPlayerCountMax)
        {
            partyPlayerIDList.Add(photonManager.myPlayer.ViewID); // 여기서 photonManager.myPlayer 는 버튼을 누른 사람의 플레이어
        }
        else if(partyPlayerIDList.Count >= partyPlayerCountMax)
        {
            Debug.Log("이미 파티가 다 찼습니다.");
        }
        else if(pv.IsMine)
        {
            Debug.Log("당신은 이 파티의 주인입니다.");
        }
    }
}
