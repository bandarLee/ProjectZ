using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public bool _init = false;

    public CityZoneType lastZone;
    public int Randomzone;
    public Transform[] spawnPosition;
    public Transform[] SceneMovePosition;
    public GameObject[] CitySectors;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            if (!_init)
            {
                Init();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }

    }


    public void Init()
    {
        int randomIndex = Random.Range(0, 6);
        CityZoneType[] cityZoneTypes = (CityZoneType[])System.Enum.GetValues(typeof(CityZoneType));

        Hashtable SceneProperties = new Hashtable();
        SceneProperties.Add("CurrentScene", randomIndex);

        PhotonNetwork.LocalPlayer.SetCustomProperties(SceneProperties);

        _init = true;
        lastZone = cityZoneTypes[randomIndex];
        ActivateCitySectorsAndSpawnPlayer((int)lastZone);
    }


    public Vector3 GetSpawnPoint()
    {
        return spawnPosition[(int)lastZone].position;
    }
    private  void ActivateCitySectorsAndSpawnPlayer(int spawnSector)
    {
        ActivateCitySectors((int)lastZone);
        SpawnPlayer(spawnSector);
    }
    private void ActivateCitySectors(int activeIndex)
    {
        foreach (GameObject citysector in CitySectors)
        {
            citysector.SetActive(false);  
        }


        CitySectors[activeIndex].SetActive(true);

    }
    private void SpawnPlayer(int spawnSector)
    {
        if (!CharacterInfo.Instance._isGameStart)
        {
            GameObject newPlayer = PhotonNetwork.Instantiate("Character_Female_rigid_collid", spawnPosition[spawnSector].position, Quaternion.identity);

            // Recipe 정보 로드
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CharacterRecipe", out object characterRecipe))
            {
                Debug.Log("Character Recipe in SpawnPlayer: " + characterRecipe);

                var avatar = newPlayer.GetComponent<DynamicCharacterAvatar>();
                ApplyRecipeString(avatar, characterRecipe.ToString());

                // RPC 호출
                photonView.RPC("RPC_UpdateCharacterRecipe", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer.ActorNumber, characterRecipe.ToString());
            }
            else
            {
                Debug.LogError("CharacterRecipe not found in CustomProperties");
            }

            CharacterInfo.Instance._isGameStart = true;
        }
        else
        {
            Character.LocalPlayerInstance.GetComponent<CharacterMoveAbilityTwo>().Teleport(SceneMovePosition[CharacterInfo.Instance.SpawnDir].position);
        }
    }
    [PunRPC]
    private void RPC_UpdateCharacterRecipe(int playerID, string recipe)
    {
        // 해당 playerID의 플레이어를 찾습니다
        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
        if (targetPlayer != null)
        {
            // 새로 생성된 플레이어 객체를 찾습니다
            GameObject playerObj = PhotonView.Find(playerID).gameObject;
            if (playerObj != null)
            {
                var avatar = playerObj.GetComponent<DynamicCharacterAvatar>();
                ApplyRecipeString(avatar, recipe);
            }
        }
    }

    public static void ApplyRecipeString(DynamicCharacterAvatar avatar, string recipeString)
    {
        if (avatar != null && !string.IsNullOrEmpty(recipeString))
        {
            avatar.LoadFromRecipeString(recipeString);
            avatar.BuildCharacter();
        }
    }



    public void LoadCity(CityZoneType cityZoneType)
    {
        lastZone = cityZoneType;
        Hashtable SceneProperties = new Hashtable();
        SceneProperties.Add("CurrentScene", (int)lastZone);
        PhotonNetwork.LocalPlayer.SetCustomProperties(SceneProperties);

        ActivateCitySectorsAndSpawnPlayer((int)lastZone);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("CurrentScene"))
        {
            int newScene = (int)changedProps["CurrentScene"];
            OnSceneChanged(targetPlayer, newScene);
        }
    }

    private void OnSceneChanged(Player player, int newScene)
    {
        Debug.LogError($"Player {player.NickName} changed scene to {newScene}");

    }

}
