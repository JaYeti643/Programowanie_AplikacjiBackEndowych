namespace AppCore.Models;

public class Organization : Contact
{
    public string Name { get; set; }
    public OrganizationType Type { get; set; }
    public string? KRS { get; set; }
    public string? Website { get; set; }
    public string? Mission { get; set; }
    public List<Person> Members { get; set; }
    public Person? PrimaryContact { get; set; }
    
    public override string GetDisplayName()
    {
        return Name;
    }
}

public enum OrganizationType
{
    Ngo,
    PublicInstitution,
    GovernmentAgency,
    Association,
    Foundation,
    Other

}