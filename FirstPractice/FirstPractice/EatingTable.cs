using System.Data.SqlTypes;
using System.Runtime.InteropServices;

namespace FirstPractice;

public class EatingTable
{
    private volatile bool flag = true; 
    
    private object[] forks;

    private Philosopher[] philosophers;
    
    public int PhilosophersCount { get; set; }

    public EatingTable(int philosophersCount)
    {
        PhilosophersCount = philosophersCount;

        forks = new object[philosophersCount];

        for (var i = 0; i < philosophersCount; ++i)
            forks[i] = new object();

        philosophers = new Philosopher[philosophersCount];

        for (var i = 0; i < philosophersCount; ++i)
        {
            philosophers[i] = i == philosophersCount - 1
                ? new Philosopher(forks[0], forks[i], i)
                : new Philosopher(forks[i], forks[i + 1], i);
        }
    }

    public void StartEating()
    {
        var threads = new Thread[PhilosophersCount];

        var mre = new ManualResetEvent(false);
        
        for (var i = 0; i < threads.Length; ++i)
        {
            var locali = i;

            threads[i] = new Thread(() =>
            {
                mre.WaitOne();
                while (flag)
                {
                    if (locali == PhilosophersCount - 1)
                    {
                        philosophers[locali].Eat(forks[0], forks[locali]);
                    }
                    else
                    {
                        philosophers[locali].Eat(forks[locali], forks[locali + 1]);
                    }
                    
                    philosophers[locali].Think();
                }
                
                Console.WriteLine($"{locali} philosopher end eating");
            });
        }
        
        foreach (var thread in threads)
            thread.Start();
        
        Thread.Sleep(1000);
        mre.Set();
       
        while (flag)
        {
            var test = Console.ReadKey(true);

            if (test.Key == ConsoleKey.Enter)
            {
                flag = false;
            }
        }

        foreach (var thread in threads)
            thread.Join();
    }
}
