using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Blazor.Gitter.Components.MVVMFramework.ViewModel
{
    public class ViewModelBase
    {
        private Dictionary<string, object> properties = new Dictionary<string, object>();

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (!properties.ContainsKey(propertyName))
            {
                properties.Add(propertyName, default(T));
            }

            T oldValue = GetValue<T>(propertyName);
            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                properties[propertyName] = value;
            }
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (!properties.ContainsKey(propertyName))
            {
                return default(T);
            }
            else
            {
                return (T)properties[propertyName];
            }
        }

        public bool pIsBusy
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

    }
}
