using System.Diagnostics;
using System.Net.NetworkInformation;

namespace CustomIdGeneration
{
    public class Provider
    {
        public static byte[] GetProcessId()
        {
            var processId = BitConverter.GetBytes(Process.GetCurrentProcess().Id);

            if (processId.Length < 2)
                throw new InvalidOperationException("Current Process Id is of insufficient length");

            return processId;
        }

        public static byte[] GetWorkerId(int index)
        {
            var network = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    || x.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet
                    || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx
                    || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT)
                .Select(x => x.GetPhysicalAddress())
                .Where(x => x != null)
                .Select(x => x.GetAddressBytes())
                .Where(x => x.Length == 6)
                .Skip(index)
                .FirstOrDefault();

            if (network == null)
                throw new InvalidOperationException("Unable to find usable network adapter for unique address");

            return network;
        }
    }
}
