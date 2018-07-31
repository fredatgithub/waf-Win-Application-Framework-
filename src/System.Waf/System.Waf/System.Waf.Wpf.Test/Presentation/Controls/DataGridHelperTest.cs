﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Waf.Presentation.Controls;

namespace Test.Waf.Presentation.Controls
{
    [TestClass]
    public class DataGridHelperTest
    {
        [TestMethod]
        public void GetDefaultTest()
        {
            AssertDefaultEqual<int>();
            AssertDefaultEqual<int?>();
            AssertDefaultEqual<string>();
            AssertDefaultEqual<KeyValuePair<int, int>>();
            AssertDefaultEqual<object>();
        }

        private static void AssertDefaultEqual<T>()
        {
            Assert.AreEqual(default(T), DataGridHelper.GetDefault(typeof(T)));
        }

        [TestMethod]
        public void GetSelectorTest()
        {
            var obj = new object();
            Assert.AreSame(obj, DataGridHelper.GetSelector<object>(null)(obj));
            Assert.AreSame(obj, DataGridHelper.GetSelector<object>("")(obj));
            Assert.IsNull(DataGridHelper.GetSelector<object>("")(null));

            var personDataModels = CreatePersonDataModels();

            AssertSelectorEqual(personDataModels, x => x?.Person?.Name, "Person.Name");
            AssertSelectorEqual(personDataModels, x => x?.Person?.Age, "Person.Age");
            AssertSelectorEqual(personDataModels, x => x?.Person, "Person");
            AssertSelectorEqual(personDataModels, x => x?.Person?.Pair, "Person.Pair");
            AssertSelectorEqual(personDataModels, x => x?.Person?.Pair.Key, "Person.Pair.Key");
            AssertSelectorEqual(personDataModels, x => x?.Person?.Pair.Value, "Person.Pair.Value");
        }

        [TestMethod, TestCategory("Performance")]
        public void GetSelectorPerformanceTest1A()  // Reference
        {
            var personDataModels = CreatePersonDataModels();
            for (int i = 0; i < 1_000_000; i++) personDataModels.OrderBy(x => x?.Person?.Name);
        }

        [TestMethod, TestCategory("Performance")]
        public void GetSelectorPerformanceTest1B()
        {
            var personDataModels = CreatePersonDataModels();
            for (int i = 0; i < 1_000_000; i++)
                personDataModels.OrderBy(DataGridHelper.GetSelector<PersonDataModel>("Person.Name"));
        }

        [TestMethod, TestCategory("Performance")]
        public void GetSelectorPerformanceTest2A()  // Reference
        {
            var personDataModels = CreatePersonDataModels();
            for (int i = 0; i < 1_000_000; i++) personDataModels.OrderBy(x => x?.Person?.Age);
        }

        [TestMethod, TestCategory("Performance")]
        public void GetSelectorPerformanceTest2B()
        {
            var personDataModels = CreatePersonDataModels();
            for (int i = 0; i < 1_000_000; i++)
                personDataModels.OrderBy(DataGridHelper.GetSelector<PersonDataModel>("Person.Age"));
        }

        private static void AssertSelectorEqual<T>(IEnumerable<T> list, Func<T, object> expected, string actual)
        {
            foreach (var item in list)
            {
                Assert.AreEqual(expected(item), DataGridHelper.GetSelector<T>(actual)(item));
            }
        }

        private PersonDataModel[] CreatePersonDataModels()
        {
            return new[]
            {
                new PersonDataModel(),
                new PersonDataModel() { Person = new Person() { Name = "Bill", Age = 100, Pair = new KeyValuePair<int, string>(100, "Bill") } },
                null,
                new PersonDataModel() { Person = new Person() { Name = "Steve", Age = 50, Pair = new KeyValuePair<int, string>(50, "Steve") } },
                new PersonDataModel() { Person = new Person() }
            };
        }

        private class PersonDataModel
        {
            public Person Person { get; set; }
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public KeyValuePair<int, string> Pair { get; set; }
        }
    }
}
