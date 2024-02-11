namespace DBfin

{
    public class Settings
    {
        public int SettingsId { get; set; }
        public string Country { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
