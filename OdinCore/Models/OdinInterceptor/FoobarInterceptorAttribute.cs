using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;

namespace OdinCore.Models.OdinInterceptor
{
    public class FoobarInterceptorAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                System.Console.WriteLine(context.ServiceMethod.Name);
                Console.WriteLine("Foobar Before service call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Foobar Service threw an exception!");
                throw;
            }
            finally
            {
                Console.WriteLine("Foobar After service call");
            }
        }
    }
}