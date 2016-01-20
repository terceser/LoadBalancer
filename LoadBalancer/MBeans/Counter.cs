using NetMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoadBalancer.MBeans
{
    public class Counter : CounterMBean
    {
        private int _counter;

        public int Value
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnCounterChanged();
            }
        }
        public void Reset()
        {
            _counter = 0;
            OnCounterChanged();
        }
        public void Add(int amount)
        {
            _counter += amount;
            OnCounterChanged();
        }
        public event EventHandler<NotificationEventArgs> CounterChanged;

        private void OnCounterChanged()
        {
            if (CounterChanged != null)
            {
                CounterChanged(this, new NotificationEventArgs("Counter changed", _counter));
            }
        }
    }
}