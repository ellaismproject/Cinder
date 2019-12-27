﻿namespace Cinder.Events
{
    public class AddressesTransactedEvent
    {
        public string[] Addresses { get; set; }

        public static AddressesTransactedEvent Create(string[] addresses)
        {
            return new AddressesTransactedEvent {Addresses = addresses};
        }
    }
}