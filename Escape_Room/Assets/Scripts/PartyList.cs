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
    public TextMeshProUGUI listMasterNameText; // 리스트 주인을 담을 UI
    public TextMeshProUGUI listThemeText;      // 리스트 게임 테마를 담을 UI
    public TextMeshProUGUI listPeopleNumText;  // 리스트 현재 파티 인원 수 정보를 담을 UI
    public int maxPeopleNum; // 파티 생성 시 설정한 최대 인원수
    public List<int> partyPlayerIDList; // 현재 파티에 있는 플레이어 ViewID 를 담을 리스트

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
            // 리스트 세팅
            pv.RPC("SetList", RpcTarget.AllBuffered, photonManager.masterName, photonManager.theme, photonManager.partyPeopleNum, photonManager.maxPeopleNum, photonManager.myPlayer.ViewID);
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
    void SetList(string nameText, string themeText, string peopleNumText, int maxPeopleNum, int playerId)
    {
        // 옵션 세팅
        listMasterNameText.text = nameText;
        listThemeText.text = themeText;
        listPeopleNumText.text = peopleNumText;

        // 생성된 파티 옵션 세팅
        this.maxPeopleNum = maxPeopleNum;
        partyPlayerIDList.Add(playerId);

        // 파티 생성 시 설정했던 값들 초기화
        photonManager.maxPeopleNum = 1; // 파티 만들때 설정한 인원 수 초기화
        photonManager.maxPeopleNumText.text = "1";
        photonManager.partyPeopleNum = $"{1} / {photonManager.maxPeopleNum}"; // 파티 만들때 설정한 리스트 인원 수 UI 초기화
    }


    // 파티 참가 버튼
    public void JoinParty(PartyList mainObj)
    {
        bool joined = false;

        foreach (GameObject list in photonManager.partyList)
        {
            PartyList partyLogic = list.GetComponent<PartyList>();

            foreach (int id in partyLogic.partyPlayerIDList)
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

        if(partyPlayerIDList.Count >= maxPeopleNum)
        {
            Debug.Log("이미 파티가 다 찼습니다.");
        }
        else if(pv.IsMine)
        {
            Debug.Log("당신은 이 파티의 주인입니다.");
        }
        else if (!pv.IsMine && partyPlayerIDList.Count < maxPeopleNum)
        {
            pv.RPC("SynchronizationPeopleNum", RpcTarget.AllBuffered, photonManager.myPlayer.ViewID, partyPlayerIDList.Count, mainObj.maxPeopleNum, true);
        }
    }

    // 리스트의 파티 멤버 및 인원 수 실시간 동기화 함수
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
