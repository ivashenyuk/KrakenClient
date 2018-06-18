namespace MyKraken.MyKrakenAPI.Models
{
    public class ServerTimeResult
    {
        public int UnixTime;
        public string Time;
    }
    public class ServerTimeResponse : ResponseBase
    {
        public ServerTimeResult Result;
    }
}
