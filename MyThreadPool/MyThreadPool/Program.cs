var test = new MyThreadPool.MyThreadPool(2);

var firstTask = test.Submit(() => {Thread.Sleep(100);
    return 1;
});

var secondTask = test.Submit(() => {Thread.Sleep(100);
    return 2;
});

var thirdTask = test.Submit(() => {Thread.Sleep(100);
    return 3;
});

firstTask = test.Submit(() => {Thread.Sleep(100);
    return 1;
});

secondTask = test.Submit(() => {Thread.Sleep(100);
    return 2;
});

thirdTask = test.Submit(() => {Thread.Sleep(100);
    return 3;
});
firstTask = test.Submit(() => {Thread.Sleep(100);
    return 1;
});

secondTask = test.Submit(() => {Thread.Sleep(100);
    return 2;
});

thirdTask = test.Submit(() => {Thread.Sleep(100);
    return 3;
});
firstTask = test.Submit(() => {Thread.Sleep(100);
    return 1;
});

secondTask = test.Submit(() => {Thread.Sleep(100);
    return 2;
});

thirdTask = test.Submit(() => {Thread.Sleep(100);
    return 3;
});
firstTask = test.Submit(() => {Thread.Sleep(100);
    return 1;
});

secondTask = test.Submit(() => {Thread.Sleep(100);
    return 2;
});

thirdTask = test.Submit(() => {Thread.Sleep(100);
    return 3;
});
firstTask = test.Submit(() => {Thread.Sleep(100);
    return 1;
});

secondTask = test.Submit(() => {Thread.Sleep(100);
    return 2;
});

thirdTask = test.Submit(() => {Thread.Sleep(100);
    return 3;
});


var lol = thirdTask.Result;

test.Shutdown();