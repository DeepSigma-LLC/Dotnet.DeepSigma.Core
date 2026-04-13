using DeepSigma.Core.Serialization;
using DeepSigma.Core.Tests.Models;
using System.IO;
using Xunit;

namespace DeepSigma.Core.Tests.Tests
{
    public class BinarySerializationTests
    {

        private static List<Person> GetTestPeople()
        {
            List<Person> people = [];
            people.Add(new Person("Alice", "Smith", "Test", "192 Main St", 23, DateTime.Parse("2000-01-01"), 20));
            people.Add(new Person("Bob", "Johnson", "Test", "193 Main St", 30, DateTime.Parse("1993-05-15"), 25));
            people.Add(new Person("Charlie", "Brown", "Test", "194 Main St", 28, DateTime.Parse("1995-10-30"), 30));
            people.Add(new Person("Diana", "White", "Test", "195 Main St", 35, DateTime.Parse("1988-07-22"), 35));
            people.Add(new Person("Ethan", "Davis", "Test", "196 Main St", 40, DateTime.Parse("1983-03-11"), 40));
            people.Add(new Person("Fiona", "Miller", "Test", "197 Main St", 27, DateTime.Parse("1996-12-05"), 45));
            people.Add(new Person("George", "Wilson", "Test", "198 Main St", 32, DateTime.Parse("1991-08-19"), 50));
            return people;
        }

        [Fact]
        public void ConvertToBinaryTest()
        {
            byte[] binary_data = BinarySerializer.Serialize(GetTestPeople(), WritePerson, 0);
            Assert.NotNull(binary_data);
        }

        [Fact]
        public void ConvertFromBinaryTest()
        {
            byte[] binary_data = BinarySerializer.Serialize(GetTestPeople(), WritePerson, 0);
            IEnumerable<Person> results = BinarySerializer.Deserialize<Person>(binary_data, ReadPerson);
            Assert.Equivalent(GetTestPeople(), results);
        }


        private static void WritePerson(BinaryWriter writer, Person person)
        {
            writer.Write(person.FirstName);
            writer.Write(person.LastName);
            writer.Write(person.Company);
            writer.Write(person.Address);
            writer.Write(person.Age);
            writer.Write(person.BirthDate.ToBinary());
            writer.Write(person.Height);
        }

        private static Person ReadPerson(BinaryReader reader)
        {
            string firstName = reader.ReadString();
            string lastName = reader.ReadString();
            string company = reader.ReadString();
            string address = reader.ReadString();
            byte age = reader.ReadByte();
            DateTime birthDate = DateTime.FromBinary(reader.ReadInt64());
            decimal heightInInches = reader.ReadDecimal();
            return new Person(firstName, lastName, company, address, age, birthDate, heightInInches);
        }

    }
}
