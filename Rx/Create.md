## 创建 Observable 
### [sample](https://github.com/yijidao/MyDemo/blob/master/RxDotNetDemo/CreateOperate.cs)
### 最简单
- Create
### 订阅时再生成
- Defer：
  - 会先生成代理，在订阅时再生成真正的Observable，所以可以减少资源占用。
  - 可以将 Hot 变成 Cold。
  - 常用于工厂方法生成 Observable。
### 从事件生成
- FromEventPattern：标准事件模式，有一个最简洁实现的重载，但是带魔法数，使用要小心。
- FromEvent：非标准事件模式。
### 从 Enumerable 生成
- ToObservable
### Observable 生成 Enumerable
- ToEnumerable：常搭配 Concat 或 StartWith使用
### Observable 生成 Dictionary
- ToDictionary：重复 key 抛异常。
### Observable 生成 Lookup
- ToLookup：重复key 不抛异常。
### 实现循环功能
- Generate
- Range
### 涉及到 IDispose 模式
- using
### 生成简单Observable，常用于单元测试
- Return
- Never
- Throw
- Empty