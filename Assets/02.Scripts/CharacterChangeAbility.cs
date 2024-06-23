using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UMA.CharacterSystem;
using UnityEngine;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class CharacterChangeAbility : MonoBehaviourPunCallbacks
{
    void Start()
    {
        StartCoroutine(WaitforPV());

    }

    void Update()
    {
        
    }
    public IEnumerator WaitforPV()
    {
        Debug.Log("3�ʱ�ٷȴ� ä�����ƹ�Ÿ");

        yield return new WaitForSeconds(3f);
        PhotonView newPlayerPhotonView = Character.LocalPlayerInstance.PhotonView;
        if (newPlayerPhotonView == null)
        {
            Debug.LogError("New player does not have PhotonView component.");
            yield break;
        }

        int newPlayerViewID = newPlayerPhotonView.ViewID;
        Debug.Log($"New player PhotonView ID: {newPlayerViewID}");

        if (Character.LocalPlayerInstance == null)
        {
            Debug.LogError("LocalPlayerInstance is null.");
            yield break;
        }

        PhotonView localPlayerPhotonView = Character.LocalPlayerInstance.PhotonView;
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("LocalPlayerInstance does not have PhotonView component.");
            yield break;
        }

        Debug.Log("Calling ChangeAvatar RPC");
        localPlayerPhotonView.RPC(nameof(ChangeAvatar), RpcTarget.AllBuffered, newPlayerViewID);



    }

    [PunRPC]
    public void ChangeAvatar(int newPlayerViewID)
    {
        Debug.Log("ä�����ƹ�Ÿ");

        GameObject newPlayer = PhotonView.Find(newPlayerViewID)?.gameObject;
        if (newPlayer == null)
        {
            Debug.LogError("New player not found!");
            return;
        }

        StartCoroutine(WaitforChangeAvatar(newPlayer));

    }
    public IEnumerator WaitforChangeAvatar(GameObject newPlayer)
    {
        Debug.Log("ä�����ƹ�Ÿ �����");

        yield return new WaitForSeconds(3f);
        Debug.Log("����");

        var avatar = newPlayer.GetComponent<DynamicCharacterAvatar>();
        Player player = newPlayer.GetComponent<PhotonView>().Owner;

        // Ŀ���� ������Ƽ���� CharacterRecipe�� �����ɴϴ�.
        if (player.CustomProperties.TryGetValue("CharacterRecipe", out object characterRecipe))
        {
            ApplyRecipeString(avatar, characterRecipe.ToString());
        }
        else
        {
            Debug.LogError("CharacterRecipe not found in custom properties.");
        }
    }
    public void ApplyCharacterRecipe(Player player, string recipeString)
    {
        GameObject playerObject = PhotonView.Find(player.ActorNumber)?.gameObject;
        if (playerObject != null)
        {
            DynamicCharacterAvatar avatar = playerObject.GetComponent<DynamicCharacterAvatar>();
            ApplyRecipeString(avatar, recipeString);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.TryGetValue("CharacterRecipe", out object characterRecipe))
        {
            ApplyCharacterRecipe(targetPlayer, characterRecipe.ToString());
        }
    }
    public void ApplyRecipeString(DynamicCharacterAvatar avatar, string recipeString)
    {
        if (avatar != null && !string.IsNullOrEmpty(recipeString))
        {
            avatar.LoadFromRecipeString(recipeString);
        }
    }
}
