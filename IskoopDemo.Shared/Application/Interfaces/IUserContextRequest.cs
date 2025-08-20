using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface IUserContextRequest
    {
        int? RequestedBy { get; }
        void SetRequestedBy(int userId);
    }

    public abstract class BaseRequest : IUserContextRequest
    {
        public int? RequestedBy { get; private set; }

        public void SetRequestedBy(int userId)
        {
            RequestedBy = userId;
        }
    }
}
