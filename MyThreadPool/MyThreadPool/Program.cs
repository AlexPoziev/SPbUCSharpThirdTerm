var test = new MyThreadPool.MyThreadPool(2);

var myTask= test.Submit(() =>
{
    Thread.Sleep(1000);
        return 2;
});

var result = myTask.Result;

test.Shutdown();

var temp = 0;