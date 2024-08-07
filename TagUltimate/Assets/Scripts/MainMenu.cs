using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> Rooms = new List<RoomInfo>();

    public static MainMenu current;

    public GameObject[] UI;

    [Space]
    public TMP_Text MaxPlayersSliderText;
    public Slider MaxPlayersSlider;

    [Space]
    public TMP_Text RoomNameInputFieldText;
    public TMP_InputField RoomNameInputField;

    [Space]
    public TMP_Text MaxTimeSliderText;
    public Slider MaxTimeSlider;

    [Space]
    public TMP_Text TotalPlayersInLobbyText;
    public TMP_Text RoomNameText;


    [Space]
    public Transform roomListContent;

    [Space]
    public Transform playerListTeamA;
    public Transform playerListTeamB;

    [Space]
    public GameObject roomListItemPrefab;
    public GameObject PlayerListItemPrefab;


    // Start is called before the first frame update
    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(current);
        }
        else
        {
            current = this;
        }

        Open(4);
    }
    void Start()
    {

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        //Debug.Log("Connecting to Lobby");
        //PhotonNetwork.ConnectUsingSettings();
        //versionText.text = $"{Application.version}<br>Created by domibron<br>https://domibron.itch.io/expoint";
    }

    public void Open(int UIIndex)
    {
        for (int i = 0; i < UI.Length; i++)
        {
            if (i == UIIndex)
            {
                UI[i].SetActive(true);
            }
            else
            {
                UI[i].SetActive(false);
            }
        }
    }

    public void CloseAll()
    {
        for (int i = 0; i < UI.Length; i++)
        {
            UI[i].SetActive(false);
        }
    }


    void Update()
    {
        if (PhotonNetwork.InRoom)
            TotalPlayersInLobbyText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (UI[1].activeSelf)
        {
            // this logic hurts my eyes. why can we do this, whyyyyyyyyyyyyyyyyyyyy.
            if (MaxPlayersSlider.value == 0)
                MaxPlayersSliderText.text = $"Maxplayers: <b>Infinite</b>";
            else
                MaxPlayersSliderText.text = $"Maxplayers: {MaxPlayersSlider.value} / {MaxPlayersSlider.maxValue}";



            if (MaxTimeSlider.value == 0)
                MaxTimeSliderText.text = $"Match Time: <b>Infinite</b>";
            else
                MaxTimeSliderText.text = $"Match Time: {MaxTimeSlider.value} / {MaxTimeSlider.maxValue}";
        }
    }



    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to Master");
        //PhotonNetwork.JoinLobby();
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Open(0);
        //Debug.Log("Joined Lobby");
    }

    public void CreateTeamRoom()
    {
        string roomName = RoomNameInputField.text;


        if (string.IsNullOrEmpty(RoomNameInputField.text))
        {
            roomName = $"ROOM {Random.Range(0, 9999999)}";
        }


        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = (byte)MaxPlayersSlider.value
        };

        float float1 = (MaxTimeSlider.value == 0 ? 99999 : MaxTimeSlider.value); // stops cringe
        //int int1 = (MaxKillsSlider.value == 0 ? 9999 : (int)MaxKillsSlider.value); // stops early end game

        // room properties
        Hashtable RoomCustomProps = new Hashtable();
        RoomCustomProps.Add("MasterTime", float1);
        //RoomCustomProps.Add("MasterKills", int1);
        RoomCustomProps.Add("MasterCT", 600f);
        //RoomCustomProps.Add("Version", Application.version);
        roomOptions.CustomRoomProperties = RoomCustomProps;
        // https://youtu.be/aVUNiJ3MVSg



        PhotonNetwork.CreateRoom($"{roomName} - max: {roomOptions.MaxPlayers}", roomOptions);

        Open(4);
    }


    public override void OnJoinedRoom()
    {
     

        int teamCount = 0; // ? what is this ment for?
        Dictionary<int, Transform> teams = new Dictionary<int, Transform>();
        teams.Add(0, playerListTeamA);
        teams.Add(1, playerListTeamB);

        //if (Application.version != PhotonNetwork.CurrentRoom.CustomProperties["Version"].ToString())
        //{
        //    PhotonNetwork.Disconnect();
        //    ////MenuManager.Instance.OpenMenu("error");
        //    OnJoinRoomFailed(3231, "Versions are not the same!<br>their version: " + PhotonNetwork.CurrentRoom.CustomProperties["Version"].ToString() + "<br>Your version: " + Application.version);
        //}

        Open(2);
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;

        TotalPlayersInLobbyText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Player player in players)
        {
            if ((PhotonNetwork.LocalPlayer.NickName == player.NickName && !player.IsLocal) || (PhotonNetwork.LocalPlayer.NickName == string.Empty)) // see if you can put this in OnPlayerEnteredRoom
            {
                PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.LocalPlayer.NickName + Random.Range(0, 9999).ToString("0000");
            }
        }

        foreach (Transform child in playerListTeamA)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in playerListTeamB)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListTeamA).GetComponent<PlayerListItem>().SetUp(players[i]);

            Hashtable props = new Hashtable();
            props.Add("team", 0);
            players[i].CustomProperties = props;
        }

        ////startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        ////mapSelectionPanel.SetActive(PhotonNetwork.IsMasterClient);

    }

    public void SwichTeams()
    {
        int x = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"];

        if (x == 0)
        {
            Hashtable props = new Hashtable();
            props.Add("team", 1);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            foreach (Transform child in playerListTeamB)
            {
                PlayerListItem pli = child.GetComponent<PlayerListItem>();
                if (pli.player == PhotonNetwork.LocalPlayer)
                {
                    Destroy(child.gameObject);
                }
            }

            Instantiate(PlayerListItemPrefab, playerListTeamA).GetComponent<PlayerListItem>().SetUp(PhotonNetwork.LocalPlayer);
        }
        else if (x == 1)
        {
            Hashtable props = new Hashtable();
            props.Add("team", 0);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            foreach (Transform child in playerListTeamA)
            {
                PlayerListItem pli = child.GetComponent<PlayerListItem>();
                if (pli.player == PhotonNetwork.LocalPlayer)
                {
                    Destroy(child.gameObject);
                }
            }


            Instantiate(PlayerListItemPrefab, playerListTeamB).GetComponent<PlayerListItem>().SetUp(PhotonNetwork.LocalPlayer);
        }
    }


    public override void OnPlayerPropertiesUpdate(Player target, Hashtable hashtable)
    {
        if (hashtable.ContainsKey("team"))
        {
            int x = (int)hashtable["team"];

            if (x == 0)
            {
                foreach (Transform child in playerListTeamB)
                {
                    PlayerListItem PLI = child.GetComponent<PlayerListItem>();
                    if (PLI.player == target)
                    {
                        Destroy(child.gameObject);
                    }
                }
                foreach (Transform child in playerListTeamA)
                {
                    PlayerListItem PLI = child.GetComponent<PlayerListItem>();
                    if (PLI.player == target)
                    {
                        Destroy(child.gameObject);
                    }
                }

                Instantiate(PlayerListItemPrefab, playerListTeamA).GetComponent<PlayerListItem>().SetUp(target);
            }
            else if (x == 1)
            {
                foreach (Transform child in playerListTeamA)
                {
                    PlayerListItem PLI = child.GetComponent<PlayerListItem>();
                    if (PLI.player == target)
                    {
                        Destroy(child.gameObject);
                    }
                }
                foreach (Transform child in playerListTeamB)
                {
                    PlayerListItem PLI = child.GetComponent<PlayerListItem>();
                    if (PLI.player == target)
                    {
                        Destroy(child.gameObject);
                    }
                }

                Instantiate(PlayerListItemPrefab, playerListTeamB).GetComponent<PlayerListItem>().SetUp(target);
            }
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        StartCoroutine(InstaceButDelayed(newPlayer));
    }

    IEnumerator InstaceButDelayed(Player newPlayer)
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(PlayerListItemPrefab, playerListTeamA).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ////errorText.text = $"Room Creation Failed: <color=\"blue\">ERROR CODE = {returnCode} <br><color=\"yellow\">REASON: {message}";
        //MenuManager.Instance.OpenMenu("error");

        Open(0);

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        ////startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        //MenuManager.Instance.OpenMenu("loading");
        Open(4);

        //PhotonNetwork.LoadLevel(MapManager.Instance.currentMapNumber);
    }

    public void StartTeamGame()
    {
        //MenuManager.Instance.OpenMenu("loading");
        Open(4);

        //PhotonNetwork.LoadLevel(MapManager.Instance.currentMapNumber);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        //MenuManager.Instance.OpenMenu("loading");
        Open(4);

    }

    public void JoinRoom(RoomInfo info)
    {
        //if (info.CustomProperties["Version"] == null)
        //    OnJoinRoomFailed(3230, "Version was not specified (Old server)");

        if (info.IsOpen && info.PlayerCount < info.MaxPlayers)
        {
            PhotonNetwork.JoinRoom(info.Name);
            //MenuManager.Instance.OpenMenu("loading");
            Open(4);
        }
        else
        {
            //if (Application.version != info.CustomProperties["Version"].ToString())
            //	OnJoinRoomFailed(3231, "Versions are not the same!<br>their version: " + info.CustomProperties["Version"].ToString() + "<br>Your version: " + Application.version);
            OnJoinRoomFailed(3232, "Room is full or no longer exists");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //errorText.text = $"Room Join Failed: <color=\"blue\">ERROR CODE = {returnCode} <br><color=\"yellow\">REASON: {message}</color>";

        //MenuManager.Instance.OpenMenu("error");

        Open(0);
    }

    public override void OnLeftRoom()
    {
        //MenuManager.Instance.OpenMenu("title");



        Open(0);
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        base.OnRoomListUpdate(p_list);

        foreach (RoomInfo room in p_list)
        {
            for (int i = 0; i < p_list.Count; i++)
            {
                if (room.Name == p_list[i].Name)
                {
                    Rooms.Remove(room);
                    continue;
                }
            }

            if (room.PlayerCount >= room.MaxPlayers)
            {
                Rooms.Remove(room);
                continue;
            }

            if (room.RemovedFromList)
            {
                Rooms.Remove(room);
                continue;
            }

            if (room.IsVisible == false)
            {
                Rooms.Remove(room);
                continue;
            }

            if (room.IsOpen == false)
            {
                Rooms.Remove(room);
                continue;
            }

            Rooms.Add(room);
        }

        //foreach (Transform trans in roomListContent)
        //{
        //    Destroy(trans.gameObject);
        //}

        //for (int i = 0; i < roomList.Count; i++)
        //{
        //    if (roomList[i].RemovedFromList)
        //        continue;
        //    else
        //        Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        //}

        ClearRoomList();

        foreach (RoomInfo room in Rooms)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
        }
    }

    public void ClearRoomList()
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
    }







    public void QuitGame()
    {
        Application.Quit();
    }
}
