using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NetworkSessionManager : NetworkBehaviour
{
    public static NetworkSessionManager Instance;

    private string sessionPassword = "";

    private Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();

    private void Awake()
    {
        if (Instance == null) {   
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetSessionPassword(string password)
    {
        if (!IsServer) return;

        sessionPassword = password;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestLoginServerRpc(string password, string playerName, ulong clientId)
    {
        if (password != sessionPassword)
        {
            DenyLoginClientRpc("Contraseña incorrecta", clientId);
            return;
        }

        playerNames[clientId] = playerName;

        ApproveLoginClientRpc(clientId);
    }

    [ClientRpc]
    private void ApproveLoginClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            SceneManager.LoadScene(1);
        }
    }

    [ClientRpc]
    private void DenyLoginClientRpc(string reason, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.LogError("No se pudo iniciar sesión: " + reason);
        }
    }
}
