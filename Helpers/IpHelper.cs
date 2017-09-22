using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ChatNetWS.Helpers
{
    public static class IpHelper
    {
        public static string GetLocalIp()
        {
            var nis = NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => IsValidNetwrokInterface(ni));
            var uniIps = nis.First().GetIPProperties().UnicastAddresses;
            return uniIps.First(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
        }

        static bool IsValidNetwrokInterface(NetworkInterface networkInterface) =>
        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
        networkInterface.OperationalStatus == OperationalStatus.Up;
    }
}