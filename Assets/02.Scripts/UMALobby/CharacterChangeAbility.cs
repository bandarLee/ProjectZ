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

    public IEnumerator WaitforPV()
    {
        yield return new WaitForSeconds(0.1f);
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
        yield return new WaitForSeconds(0.1f);

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
        StartCoroutine(TryApplyCharacterRecipe(player, recipeString));
    }

    private IEnumerator TryApplyCharacterRecipe(Player player, string recipeString, int maxAttempts = 3, float retryDelay = 1f)
    {
        int attempts = 0;
        while (attempts < maxAttempts)
        {
            GameObject playerObject = PhotonView.Find(player.ActorNumber)?.gameObject;
            if (playerObject != null)
            {
                DynamicCharacterAvatar avatar = playerObject.GetComponent<DynamicCharacterAvatar>();
                if (avatar != null)
                {
                    ApplyRecipeString(avatar, recipeString);
                    yield break; // �����ϸ� ����
                }
            }

            attempts++;
            Debug.LogWarning($"Failed to apply character recipe. Attempt {attempts} of {maxAttempts}.");
            yield return new WaitForSeconds(retryDelay);
        }

        Debug.LogError("Failed to apply character recipe after maximum attempts.");
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
