namespace ShroomCity.Models.InputModels;

public class ResearchEntryInputModel
{
    public List<KeyValuePair<string, string>> Entries { get; set; } = new List<KeyValuePair<string, string>>();
}