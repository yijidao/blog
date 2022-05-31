﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using PrismAop.Attributes;

namespace PrismAop.Interceptors
{
    public class CacheInterceptor : AsyncInterceptorBase
    {
        private readonly IMemoryCache _memoryCache;

        public CacheInterceptor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var attribute = invocation.Method.GetCustomAttribute<CacheAttribute>();
            if (attribute == null)
            {
                return await proceed(invocation, proceedInfo).ConfigureAwait(false);
            }

            var cacheKey = attribute.CacheKey ?? GenerateKey(invocation);
            if (_memoryCache.TryGetValue(cacheKey, out TResult cacheValue))
            {
                if (cacheValue is string[] array)
                {
                    Debug.WriteLine($"[Cache]  Key: {cacheKey}, Value: {string.Join(',', array)}");
                }

                return cacheValue;
            }
            else
            {
                cacheValue = await proceed(invocation, proceedInfo).ConfigureAwait(false);
                // 空字符不缓存

                if (!string.IsNullOrWhiteSpace(cacheValue as string))
                {
                    _memoryCache.Set(cacheKey, cacheValue);
                }
                // 空集合不缓存

                else if (cacheValue is IEnumerable eAble )
                {
                    var e = eAble.GetEnumerator();
                    if (e.MoveNext())
                    {
                        e.Reset();
                        _memoryCache.Set(cacheKey, cacheValue);
                    }
                }
                // 空对象不缓存

                else if (cacheValue != null)
                {
                    _memoryCache.Set(cacheKey, cacheValue);
                }
                return cacheValue;
            }
        }

        private string GenerateKey(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var className = (methodInfo.DeclaringType ?? this.GetType()).FullName;
            var methodName = methodInfo.Name;
            var arguments = string.Empty;
            var parameters = methodInfo.GetParameters();

            if (parameters.Any() && parameters.Length == invocation.Arguments.Length)
            {
                // 格式化 {name : value, name2 : value2 }
                arguments =
                    $"{{ {string.Join(", ", parameters.Select((arg, index) => this.FormatArgumentString(arg, invocation.Arguments[index])))} }}";
            }

            // Create a cache key using the retrieved info.
            return $"{className}.{methodName}({arguments})";
        }

        private string FormatArgumentString(ParameterInfo argument, object value)
        {
            string formatted;
            switch (value)
            {
                case null:
                    formatted = "null";
                    break;
                case IEnumerable iEnumerable:
                {
                    var list = (from object o in iEnumerable select o == null ? "null" : o.ToString()).ToList();
                    formatted = string.Join(",", list);
                    break;
                }
                default:
                    formatted = value.ToString();
                    break;
            }


            //if (typeof(IEnumerable).IsAssignableFrom(argument.ParameterType))
            //{

            //    if (argument.ParameterType == typeof(string[]))
            //    {

            //    }
            //    else if(argument.ParameterType.GenericTypeArguments.FirstOrDefault() == typeof(string))
            //    {

            //    }
            //}




            // Convert value to string and remove line breaks.
            //var stringValue = Convert.ToString(value)
            //    ?.Replace("\r", "\\r")
            //    .Replace("\n", "\\n");

            // Wrap value in quotes if it's a string
            //var formatted = argument.ParameterType == typeof(string)
            //    ? string.Concat("\"", stringValue, "\"")
            //    : stringValue;

            return $"{argument.Name}({argument.ParameterType.FullName}) : {formatted}";
        }
    }
}
