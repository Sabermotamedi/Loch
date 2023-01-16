namespace Loch.Shared.Web.API.Security.Models
{
    public class Permission
    {
        public Guid Id { get; set; }
        public long[] MicroPermissions => CalculatedMicroPermissions();

        public string GrantedMicroPermissions { get; set; }
        public string DeniedMicroPermissions { get; set; }

        private long[] CalculatedMicroPermissions()
        {
            var grantedMicroPermissionList = GrantedMicroPermissions?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => (long)long.Parse(s)).ToList();
            var deniedMicroPermissionList = DeniedMicroPermissions?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => (long)long.Parse(s)).ToList();

            grantedMicroPermissionList?.RemoveAll(item => deniedMicroPermissionList?.Contains(item) ?? false);

            return grantedMicroPermissionList.ToArray();
        }
    }
}