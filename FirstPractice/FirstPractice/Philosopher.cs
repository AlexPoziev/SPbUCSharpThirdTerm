namespace FirstPractice;

public class Philosopher
{
    public int PhilosopherNumber { get; set; }

    private readonly Random random = new Random();

    private readonly int maxActionTime = 200;

    public Philosopher(object firstFork, object secondFork, int philosopherNumber)
    {
        PhilosopherNumber = philosopherNumber;
    }
        
    public void Eat(object firstFork, object secondFork)
    {
        Console.WriteLine($"{PhilosopherNumber} philosopher is trying to get the first fork");

        lock (firstFork)
        {
            Console.WriteLine($"{PhilosopherNumber} philosopher is trying to get the second fork");

            lock (secondFork)
            {
                Console.WriteLine($"{PhilosopherNumber} philosopher is eating");
                Thread.Sleep(random.Next(maxActionTime));
            }
        }
    }

    public void Think()
    {
        Console.WriteLine($"{PhilosopherNumber} philosopher is thinking");
        Thread.Sleep(random.Next(maxActionTime));
    }
}