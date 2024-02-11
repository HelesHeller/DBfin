using DBfin;


namespace DBfin
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public Settings Settings { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

}
