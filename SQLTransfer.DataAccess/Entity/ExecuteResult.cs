namespace SQLTransfer.DataAccess.Entity
{
    public class ExecuteResult
    {
        public int Id { get; set; }
        public string TableName { get; set; }

        public string Result { get; set; }

        public string Error { get; set; }
    }    
}
