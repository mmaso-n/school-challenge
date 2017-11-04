namespace SchoolChallenge.Client
{
    public interface ITenantConfiguration
    {
        string Tenant { get; set; }
    }

    public class TenantConfiguration: ITenantConfiguration
    {
        public string Tenant { get; set; }
    }
}
