using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public string playerNickName;
    public GameObject playerPrefab;
    public GameObject playerSpawnPoint;

    private void Start()
    {
        DefaultPool pool = new DefaultPool();
        pool.ResourceCache.Clear();
        pool.ResourceCache.Add(playerPrefab.name, playerPrefab);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
        //base.OnConnectedToMaster();
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.JoinOrCreateRoom("Escape Room", roomOptions, TypedLobby.Default);

        //base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        CreatePlayer();
        // base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom : " + newPlayer.NickName);
        //base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom : " + otherPlayer.NickName);
        //base.OnPlayerLeftRoom(otherPlayer);
    }

    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation);
        player.AddComponent<Player_Test>();
        
    }
}