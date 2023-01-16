using Loch.Shared.Web.API.Security.ApiChash.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash
{
    internal static class ApiKey
    {
        private static GenericCache Pool => CacheManager.GetNamedCache(Constants.SubsystemId, "ApiKey");
        private static GenericCache ApiKeyByBizdomainPool => CacheManager.GetNamedCache(Constants.SubsystemId, "ApiKeysByBizdomain");

        internal static CrmApiKey Get(string apiKey)
        {
            var result = Pool[apiKey] as CrmApiKey;

            //if (result == null)
            //{
            //    var apiKeyHash = CommonUtility.ComputeHash(apiKey.ToLower());
            //    result = CRMUtility.ApiKey.GetFromDB(apiKeyHash);
            //    Pool.Add(apiKey, result);
            //}


            return result;
        }

        //internal static List<CrmApiKey> GetAll(Guid bizdomainId)
        //{
        //    var result = ApiKeyByBizdomainPool[bizdomainId.ToString()] as List<CrmApiKey>;

        //    if (result == null)
        //    {
        //        result = CRMUtility.ApiKey.GetAllFromDB(bizdomainId);
        //        ApiKeyByBizdomainPool.Add(bizdomainId.ToString(), result);
        //    }
        //    return result;
        //}

        //internal static void RemoveGlobal(Guid bizdomainId)
        //{
        //    ApiKeyByBizdomainPool.RemoveGlobal(bizdomainId.ToString());
        //    Pool.Clear();
        //    AccountingUtility.SettingsDataUpdate.Set(bizdomainId);
        //}
    }
}
