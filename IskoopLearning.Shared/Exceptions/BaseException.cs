using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Exceptions
{
    public abstract class BaseException : Exception
    {
        public string ErrorCode { get; protected set; }
        public Dictionary<string, object> Details { get; protected set; }
        public DateTime OccurredAt { get; protected set; }

        protected BaseException(string message, string errorCode = null) : base(message)
        {
            ErrorCode = errorCode ?? GetType().Name;
            Details = new Dictionary<string, object>();
            OccurredAt = DateTime.UtcNow;
        }

        protected BaseException(string message, Exception innerException, string errorCode = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode ?? GetType().Name;
            Details = new Dictionary<string, object>();
            OccurredAt = DateTime.UtcNow;
        }

        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ErrorCode = info.GetString(nameof(ErrorCode));
            OccurredAt = info.GetDateTime(nameof(OccurredAt));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(OccurredAt), OccurredAt);
        }

        public BaseException WithDetail(string key, object value)
        {
            Details[key] = value;
            return this;
        }

        public T GetDetail<T>(string key)
        {
            return Details.ContainsKey(key) ? (T)Details[key] : default;
        }
    }
}
