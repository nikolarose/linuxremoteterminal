using System.Net.NetworkInformation;

namespace LinuxRemoteTerminal.Utils
{
    internal class PingUtil
    {
        public static float getPing(string remoteIP)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(remoteIP);
            return reply.RoundtripTime;

        }
    }
}
