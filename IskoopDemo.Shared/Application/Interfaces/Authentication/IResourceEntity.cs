using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Authentication
{
    public interface IResourceEntity
    {
        string Id { get; }
        string OwnerId { get; }
    }
}
