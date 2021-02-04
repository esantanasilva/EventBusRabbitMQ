using EventBus.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceA.ServiceEvents.Events
{
    public class HelloWorldEvent : Event
    {
        public string Message {get;set;}
    }
}
