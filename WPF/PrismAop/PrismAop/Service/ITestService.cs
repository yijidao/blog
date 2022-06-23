using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PrismAop.Attributes;

namespace PrismAop.Service
{
    public interface ITestService
    {
        /// <summary>
        /// 一个查询大量数据的接口
        /// </summary>
        /// <returns></returns>
        [Cache]
        Task<string[]> GetLargeData();
        [Cache]

        Task<string> Get(params string[] s);
        [Cache]

        Task<string[]> Get(List<string> s);

        [Cache(expiration:1)]
        Task<DateTime> TestExpiration();

        [Cache]
        Task<object?> TestNull();
    }

    public class TestService : ITestService
    {
        public async Task<string[]> GetLargeData()
        {
            await Task.Delay(2000);
            var result = new[]{"大","量","数","据"};
            Debug.WriteLine("从接口查询数据");
            return result;
        }

        public Task<string> Get(params string[] s)
        {
            return Task.FromResult(string.Join(",", s));
        }

        public Task<string[]> Get(List<string> s)
        {
            return Task.FromResult(s.ToArray());

        }

        public Task<DateTime> TestExpiration()
        {
            return Task.FromResult(DateTime.Now);
        }

        public Task<object?> TestNull()
        {
            return Task.FromResult<object?>(null);
        }
    }

    public interface ITestService2
    {
        /// <summary>
        /// 一个查询大量数据的接口
        /// </summary>
        /// <returns></returns>
        [Cache]
        Task<string[]> GetLargeData();
    }

    public class TestService2 : ITestService2
    {
        public async Task<string[]> GetLargeData()
        {
            await Task.Delay(2000);
            var result = new[] { "大", "量", "数", "据" };
            Debug.WriteLine("从接口查询数据");
            return result;
        }
    }
}
