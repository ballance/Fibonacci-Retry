## Fibonacci Retry
Some C# code to help with the common problem of retries.  Retries start at a short interval and then extends to longer intervals by fibonacci sequence.  Intervals, maximum overall retry time, and number of retries with default intervals can be used.  Accepts any Action block.

#### A word of caution
It is almost never a good idea to just retry something unless you fully understand why retrying is the best approach.  I caution against using this code as a further shovel to try and dig yourself out of a try/catch/ignore hole you've dug yourself into.  In the long run, it will nearly always make things worse.

### Usage
 - Code example goes here
 - Another code example goes here

### Todo.txt
 - Unit tests
 - Better documentation

#### Nuget Package

[Ballance.FibonacciRetry](https://github.com/ballance/FibonacciRetry)