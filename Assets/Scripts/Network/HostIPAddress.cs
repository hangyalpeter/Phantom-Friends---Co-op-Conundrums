using System.Net;
using UnityEngine;

public class HostIPAddress : MonoBehaviour
{
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public void DisplayHostIP()
    {
        string hostIP = GetLocalIPAddress();
        Debug.Log("Host IP Address: " + hostIP);
    }

}
