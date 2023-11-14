using Photon.Pun;
using UnityEngine;

public class RPCManager : MonoBehaviourPunCallbacks
{
    public static RPCManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void SendRPC(PhotonView m_photonView, string m_functionName, params object[] m_parameters)
    {
        m_photonView.RPC(m_functionName, RpcTarget.AllViaServer, m_parameters);
    }
}
