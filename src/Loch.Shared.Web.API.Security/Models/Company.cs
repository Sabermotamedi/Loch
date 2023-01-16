namespace Loch.Shared.Web.API.Security.Models
{
    internal class Company
    {
        public Guid? Id { get; set; }

        public string Symbol { get; set; }

        public Guid? IndustryId { get; set; }

        public Guid? StateId { get; set; }
    }
}