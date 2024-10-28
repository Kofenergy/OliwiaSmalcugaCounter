using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CounterDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public CounterDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Counter>().Wait();
    }

    public Task<List<Counter>> GetCountersAsync()
    {
        return _database.Table<Counter>().ToListAsync();
    }

    public Task<Counter> GetCounterAsync(int id)
    {
        return _database.Table<Counter>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public Task<int> SaveCounterAsync(Counter counter)
    {
        if (counter.Id != 0)
        {
            return _database.UpdateAsync(counter);
        }
        else
        {
            return _database.InsertAsync(counter);
        }
    }

    public Task<int> DeleteCounterAsync(Counter counter)
    {
        return _database.DeleteAsync(counter);
    }
}
