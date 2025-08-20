using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface IBusinessRule
    {
        string Message { get; }
        bool IsBroken();
    }
}
