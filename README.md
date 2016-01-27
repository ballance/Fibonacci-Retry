## Fibonacci Retry
Some C# code to help with the common problem of retries.  Retries start at a short interval and then extends to longer intervals by fibonacci sequence.  Intervals, maximum overall retry time, and number of retries with default intervals can be used.  Accepts any Action block.

#### A word of caution
It is almost never a good idea to just retry something unless you fully understand why retrying is the best approach.  I caution against using this code as a further shovel to try and dig yourself out of a try/catch/ignore hole you've dug yourself into.  In the long run, it will nearly always make things worse.

### NuGet package
https://www.nuget.org/packages/Ballance.Retry/
    install-package Ballance.Retry

### Usage
#### Add 1+1
    var 2 = fibonacciRetry.Do(() => 1+1);

#### Fail ten times, then succeed
    var exceptions = new List<Exception>();  
    var trueAfterTenTries = new FibonacciRetry().Do<bool>(() =>
    {
        if (_tenTimeFailCounter-- > 0)
        {
            throw new ApplicationException($"Counter is at {_tenTimeFailCounter+1}, thus greater than zero");     
        }
        return true;
    }, ref exceptions, 11, 5);

#### Try to find an odd number out of randomly generated integers
    static Random rand;
    var exceptions = new List<Exception>();  
    var trueAfterTenTries = new FibonacciRetry().Do<bool>(() =>
    {
        if (_tenTimeFailCounter-- > 0)
        {
            throw new ApplicationException($"Counter is at {_tenTimeFailCounter+1}, thus greater than zero");    
        }
        return true;
    }, ref exceptions, 11, 5);
    
### Todo.txt
 - Additional Unit tests
 - Improve documentation

#### Nuget Package

[Ballance.FibonacciRetry](https://github.com/ballance/FibonacciRetry)
