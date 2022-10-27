# CPU Bound、Thread、Task、Parallel
## CPU Bound 任务
当完成一个任务所花费的时间主要取决于 CPU，就是 CPU Bound 任务，也叫做 CPU 密集型任务，如界面渲染、计算表达式等。
当完成一个任务所花费的时间主要取决于外设、网络等，就是 IO Bound 任务，也叫做 IO 密集型任务，如数据库读取数据、访问接口、加载磁盘文件等。
现在 CPU 一般都是多核的，所以要充分利用 CPU 的能力，会在一个进程内使用多个线程并行进行计算任务。
在 .Net 中执行 CPU Bound 任务，一般直接使用 Thread、Task 或 Parallel。

## Thread
### 使用 Thread 执行任务
- new
  - 前台线程后台线程
- ThreadPool
### 获取 Thread 返回值、处理 Thread 返回异常
### 协助式取消任务
### 调试

## 使用 Task
### 使用 Task 执行任务
- new
- Task.Run
- Task.Factory
### 设置返回值、处理 Task 异常
### 协助式取消任务
### 子Task
- 分离式、附加式
### 后续任务
### Task.Yield、SynchronizationContext、TaskScheduler
### 调试

## 使用 Parallel
### Parallel 的分块
### 使用 Parallel 执行任务
- Parallel.Invoke Parallel.For Parallel.ForEach
- 串行变并行、并行变串行、Parallel.For Parallel.ForEach 的重载、分区局部变量、线程局部变量？
- Break、Stop