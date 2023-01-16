using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.Utilities
{
  public interface IApiKeyHash
  {
    public string ComputeHash(string stringToHash, string salt = null);
  }
}
