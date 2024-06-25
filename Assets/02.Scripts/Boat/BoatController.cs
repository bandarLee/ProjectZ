using UnityEngine;
using Photon.Pun;

public class BoatController : MonoBehaviourPun
{
    private bool isControlling = false;
    private GameObject player;
    private Transform cameraRoot;
    public float moveSpeed = 5f;
    public float turnSpeed = 3f;

    public Transform controlPosition; 

    private void Update()
    {
        if (isControlling && player.GetComponent<PhotonView>().IsMine)
        {
            HandleInput();

        }

    }

    private void HandleInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward * -v * moveSpeed * Time.deltaTime;
        transform.position += forward;

        float turn = h * turnSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);

        player.transform.position = controlPosition.position;
        player.transform.rotation = controlPosition.rotation;
        
    }

    public void StartControlling(GameObject player)
    {
        this.player = player;
        isControlling = true;
        cameraRoot = player.GetComponent<CharacterRotateAbility>()?.CameraRoot;
        if (cameraRoot == null)
        {
            Debug.LogError("CameraRoot not found on player.");
            return;
        }
        player.GetComponent<CharacterMoveAbilityTwo>().enabled = false;
        player.GetComponent<CharacterRotateAbility>().enabled = false;
        cameraRoot.localRotation = Quaternion.Euler(0, 0, 0);

        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }

        Character.LocalPlayerInstance._animator.SetBool("IsControl", true);
    }

    public void StopControlling(GameObject player)
    {
        isControlling = false;

        // Enable player movement and camera control
        player.GetComponent<CharacterMoveAbilityTwo>().enabled = true;
        player.GetComponent<CharacterRotateAbility>().enabled = true;

        // Enable player's Rigidbody
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }


        Character.LocalPlayerInstance._animator.SetBool("IsControl", false);
    }
}
