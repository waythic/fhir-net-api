﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hl7.Fhir.Model;
using System.Diagnostics;

namespace Hl7.Fhir.Serialization.Test
{
    [TestClass]
    public class ResourceReadingTest
    {
        [TestMethod]
        public void TestLoadResource()
        {
            Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Hl7.Fhir.Serialization.Test.TestPatient.json");
          
            BindingConfiguration.AcceptUnknownMembers = true;

            BindingConfiguration.ModelAssemblies.Add(typeof(Resource).Assembly);

            var inspector = new ModelInspector();
            inspector.Import(typeof(Resource).Assembly);
            inspector.Process();

            s.Seek(0, SeekOrigin.Begin);
            var jsonReader = new Newtonsoft.Json.JsonTextReader(new System.IO.StreamReader(s));
            var root = JObject.Load(jsonReader);
            var reader = new ResourceReader(inspector, root);
            var result = reader.Deserialize();

            Stopwatch x = new Stopwatch();

            x.Start();

            for (int i = 0; i < 10000; i++)
            {
                s.Seek(0, SeekOrigin.Begin);
                jsonReader = new Newtonsoft.Json.JsonTextReader(new System.IO.StreamReader(s));
                root = JObject.Load(jsonReader);
                reader = new ResourceReader(inspector, root);
                result = reader.Deserialize();
            }
            x.Stop();

            Debug.WriteLine(x.ElapsedMilliseconds);
            
        }
    }
}
