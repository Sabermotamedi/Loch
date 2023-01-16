namespace Loch.Shared.Web.API.Security.Models
{
    internal class Position
    {
        public Guid Id { get; set; }
        public Guid OrganizationUnitId { get; set; }
        public string PostName { get; set; }
    }
}