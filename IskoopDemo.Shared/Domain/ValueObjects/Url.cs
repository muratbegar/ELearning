using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class Url : ValueObject
    {
        public string Value { get; private set; }

        private Url() { }

        public Url(string value)
        {

            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("URL cannot be empty", nameof(value));
            value = value.Trim();

            if(!Uri.TryCreate(value,UriKind.Absolute,out var uri))
                throw new ArgumentException("Invalid URL format", nameof(value));

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("URL must use HTTP or HTTPS protocol", nameof(value));

            Value = value;
        }

        public Uri ToUri() => new(Value);
        public string GetDomain() => ToUri().Host;
        public bool IsSecure() => ToUri().Scheme == Uri.UriSchemeHttps;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Url url) => url?.Value;
        public static implicit operator Url(string url) => new(url);

        public override string ToString() => Value;
    }
}
