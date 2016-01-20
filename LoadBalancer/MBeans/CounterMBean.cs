using NetMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoadBalancer.MBeans
{
    public interface CounterMBean
    {
        int Value { get; set; }
        void Reset();
        void Add(int amount);
        [MBeanNotification("sample.counter")]
        event EventHandler<NotificationEventArgs> CounterChanged;
    }
}