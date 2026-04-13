using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSigma.Core.Tests.Models
{
    internal class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public byte Age { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal Height { get; set; }
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }

        internal Person(string firstName, string lastName, string company, string address, byte age, DateTime birthDate, decimal height)
        {
            FirstName = firstName;
            LastName = lastName;
            Company = company;
            Address = address;
            Age = age;
            BirthDate = birthDate;
            Height = height;
        }
    }
}
