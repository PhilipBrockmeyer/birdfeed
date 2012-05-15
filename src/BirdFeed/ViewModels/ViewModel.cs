using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BirdFeed.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Boolean ChangeAndNotify<T>(ref T field, T value, Expression<Func<T>> memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException("memberExpression");
            }

            var body = memberExpression.Body as MemberExpression;
            if (body == null)
            {

                throw new ArgumentException("Lambda must return a property.");
            }

            if (EqualityComparer<T>.Default.Equals(field, value))
            {

                return false;
            }

            field = value;

            var vmExpression = body.Expression as ConstantExpression;
            if (vmExpression != null)
            {
                LambdaExpression lambda = Expression.Lambda(vmExpression);
                Delegate vmFunc = lambda.Compile();
                object sender = vmFunc.DynamicInvoke();
                if (PropertyChanged != null)
                {
                    PropertyChanged(sender, new PropertyChangedEventArgs(body.Member.Name));
                }
            }            

            return true;
        }
    }    
}
