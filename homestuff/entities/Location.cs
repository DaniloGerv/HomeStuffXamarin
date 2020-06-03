using System;
using Plugin.CloudFirestore.Attributes;

namespace homestuff.entities
{
    public class Location
    {
        public double latitude { get; set; }
        public double longitude{get; set;}
        public string address{get; set;}
        public string city{get; set;}
        public string state{get; set;}
        public string country{get; set;}
        public string postalCode{get; set;}
        public string name{get; set;}




        public Location() { }
    }
}
