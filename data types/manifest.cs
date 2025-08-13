
[Serializable]
public class PlayerManifest
{
    public List<PlayerEntry> Entries = [];

    [Serializable]
    public class PlayerEntry
    {
        public string Player;
        public string[] Files;
    }

}