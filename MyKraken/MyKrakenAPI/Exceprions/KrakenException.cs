using MyKraken.MyKrakenAPI.Models;
using System;
using System.Runtime.Serialization;

namespace MyKraken.MyKrakenAPI.Exceptions
{
    [Serializable]
    public class KrakenException : Exception
    {
        #region Properties
        public ResponseBase Response { get; }
        #endregion

        #region Constructors
        public KrakenException() { }
        public KrakenException(string message) : base(message) { }
        public KrakenException(string message, Exception innerException) : base(message, innerException) { }
        public KrakenException(string message, ResponseBase response) : base(message)
        {
            Response = response;
        }
        protected KrakenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion

        #region Public Methods
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            info.AddValue("Response", Response);
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
