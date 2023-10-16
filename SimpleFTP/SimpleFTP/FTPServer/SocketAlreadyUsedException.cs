using System.Net.NetworkInformation;

namespace SimpleFTP;

public class SocketAlreadyUsedException: Exception
{
    public static void ThrowIfInUse(int port)
    {
        var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        var ipEndPoints = ipProperties.GetActiveTcpListeners();


        if (ipEndPoints.Any(endPoint => endPoint.Port == port))
        {
            throw new SocketAlreadyUsedException();
        }
    }
}