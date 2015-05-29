using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Shared
{
    public static class EventHandlerExtensions
    {
        public static void Raise<T>(this PropertyChangedEventHandler handler, object sender, Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");

            var body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
            
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(body.Member.Name));
            }
        }
    }
}
