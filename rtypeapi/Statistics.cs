namespace rtypeapi
{
    class Statistics
    {
        DataBase dataBase;

        public Statistics(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        internal int GetCountOnlineUsers()
        {
            int count = dataBase.GetCountOnlineUsers();
            return count;
        }
    }
}