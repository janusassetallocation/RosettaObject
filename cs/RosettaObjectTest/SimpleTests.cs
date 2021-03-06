﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RosettaObject;

namespace StdJsonTest
{
    [TestFixture]
    public class SimpleTests
    {
        public static JToken LoadJson(string name)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("RosettaObjectTest.std_objects.json");
            using (StreamReader file = new StreamReader(s))
            using (var reader = new JsonTextReader(file))
            {
                return JToken.ReadFrom(reader);
            }
        }

        public static JToken StdRObjs = LoadJson("std_objects.json");

        public static object[] StdCsObjects =
        {
            true, false,
            0, 1, 73,
            0.0, 3.142,
            "", "How now brown cow?",
            new DateTime(2015, 12, 25),

            new int[0],
            new[] {0, 1, 2},
            new[,] {{0, 1}, {2, 3}},
            new double[0],
            new double[] {0, 1, 2},
            new double[,] {{0, 1}, {2, 3}},
            new List<object>(),
            new List<object> {0, 1, 2},
            new List<object> {"a", "b", "c"},
            new List<object> {new List<object> {0}, new List<object> {0, 1}},
            new Dictionary<string, object>(),
            new Dictionary<string, object> {{"a", 1}},
            new Dictionary<string, object> {{"a", new List<object> {1.0}}, {"b", new List<object>() {2.0}}},

            Index.Create(new[] {1, 2}),
            Index.Create(new[] {"A", "B"}),
            Index.Create(new[] {new DateTime(2015, 12, 25), new DateTime(2015, 12, 26),}),

            Frame.FromColumns(new[]
            {
                new KeyValuePair<string, Series<int, double>>("A", new Series<int, double>(new[] {0, 1}, new[] {1.0, 3.0})),
                new KeyValuePair<string, Series<int, double>>("B", new Series<int, double>(new[] {0, 1}, new[] {2.0, 4.0}))
            }) //pd.DataFrame([[1., 2.], [3., 4.]], [0, 1], ["A", "B"])
        };

        public static List<object[]> StdTestData = StdRObjs.Zip(StdCsObjects, (a, b) => new[] {a, b}).ToList();

        [Test, TestCaseSource(nameof(StdTestData))]
        public void SimpleTestToJson(JToken expectedRObj, object csobj)
        {
            JObject actualRobj = ConvertorSet.Default.ToJson(csobj);
            Assert.IsTrue(JToken.DeepEquals(expectedRObj, actualRobj));
        }

        [Test, TestCaseSource(nameof(StdTestData))]
        public void SimpleTestFromJson(JToken robj, object expectedCsObj)
        {
            object actualCsObj = ConvertorSet.Default.FromJson((JObject)robj);
            Assert.AreEqual(expectedCsObj, actualCsObj);
        }
    }
}
