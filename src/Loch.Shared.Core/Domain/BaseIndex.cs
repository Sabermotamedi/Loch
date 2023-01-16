using Loch.Shared.Core.Attribiutes;

namespace Loch.Shared.Core.Domain;

public  class BaseIndex
{
    public string _tp { get; set; }
    public string rv { get; set; }
    public int _Type { get; set; }
    public Guid Id { get; set; }
    public Guid BizdomainId { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsDeleted { get; set; }
    [SearchParam]
    public string CodeString { get; set; }
    public int VisibilityType { get; set; }
    public int _rv { get; set; }
}