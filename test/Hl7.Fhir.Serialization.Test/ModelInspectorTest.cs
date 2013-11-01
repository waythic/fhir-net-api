﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;

namespace Hl7.Fhir.Serialization.Test
{
    [TestClass]
    public class ModelInspectorTest
    {
        [TestMethod]
        public void TestResourceClassInspection()
        {
            var inspector = new ModelInspector();

            inspector.Import(typeof(Street));
            inspector.Import(typeof(RoadResource));
            inspector.Import(typeof(Way));
            inspector.Import(typeof(ProfiledWay));
            inspector.Import(typeof(NewStreet));
            inspector.Import(typeof(ModelInspectorTest));  // shouldn't give an exception
            inspector.Process();

            var road = inspector.FindClassMappingForResource("roAd");
            Assert.IsNotNull(road);
            Assert.AreEqual(FhirModelConstruct.Resource, road.ModelConstruct);
            Assert.AreEqual("Road", road.Name);
            Assert.IsNull(road.Profile);
            Assert.AreEqual(road.ImplementingType, typeof(RoadResource));

            var way = inspector.FindClassMappingForResource("Way");
            Assert.IsNotNull(way);
            Assert.AreEqual("Way", way.Name);
            Assert.IsNull(way.Profile);
            Assert.AreEqual(way.ImplementingType, typeof(Way));

            var pway = inspector.FindClassMappingForResource("way", "http://nu.nl/profile#street");
            Assert.IsNotNull(pway);
            Assert.AreEqual("Way", pway.Name);
            Assert.AreEqual("http://nu.nl/profile#street", pway.Profile);
            Assert.AreEqual(pway.ImplementingType, typeof(ProfiledWay));

            var pway2 = inspector.FindClassMappingForResource("way", "http://nux.nl/profile#street");
            Assert.IsNotNull(pway2);
            Assert.AreEqual("Way", pway2.Name);
            Assert.IsNull(pway2.Profile);
            Assert.AreEqual(pway2.ImplementingType, typeof(Way));

            var street = inspector.FindClassMappingForResource("Street");
            Assert.IsNotNull(street);
            Assert.AreEqual("Street", street.Name);
            Assert.IsNull(street.Profile);
            Assert.AreEqual(street.ImplementingType, typeof(NewStreet));

            var noway = inspector.FindClassMappingForResource("nonexistent");
            Assert.IsNull(noway);
        }


        [TestMethod]
        public void TestDataTypeInspection()
        {
            var inspector = new ModelInspector();

            inspector.Import(typeof(AnimalName));
            inspector.Import(typeof(NewAnimalName));
            inspector.Import(typeof(ComplexNumber));
            inspector.Import(typeof(SomeEnum));
            inspector.Import(typeof(ActResource.SomeOtherEnum));
            inspector.Process();

            var result = inspector.FindClassMappingForFhirDataType("animalname");
            Assert.IsNotNull(result);
            Assert.AreEqual(FhirModelConstruct.ComplexType, result.ModelConstruct);
            Assert.AreEqual("AnimalName", result.Name);
            Assert.IsNull(result.Profile);
            Assert.AreEqual(result.ImplementingType, typeof(NewAnimalName));

            result = inspector.FindClassMappingForFhirDataType("cOmpleX");
            Assert.IsNotNull(result);
            Assert.AreEqual(FhirModelConstruct.PrimitiveType, result.ModelConstruct);
            Assert.AreEqual("Complex", result.Name);
            Assert.IsNull(result.Profile);
            Assert.AreEqual(result.ImplementingType, typeof(ComplexNumber));

            result = inspector.FindClassMappingForFhirDataType("SomeEnum");
            Assert.IsNotNull(result);
            Assert.AreEqual(FhirModelConstruct.PrimitiveType, result.ModelConstruct);
            Assert.AreEqual("SomeEnum", result.Name);
            Assert.IsNull(result.Profile);
            Assert.AreEqual(result.ImplementingType, typeof(SomeEnum));

            result = inspector.FindClassMappingForFhirDataType("someOtherenum");
            Assert.IsNotNull(result);
            Assert.AreEqual(FhirModelConstruct.PrimitiveType, result.ModelConstruct);
            Assert.AreEqual("SomeOtherEnum", result.Name);
            Assert.IsNull(result.Profile);
            Assert.AreEqual(result.ImplementingType, typeof(ActResource.SomeOtherEnum));
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMixedDataTypeDetection()
        {
            var inspector = new ModelInspector();

            inspector.Import(typeof(Chameleon));
            inspector.Process();
        }

        [TestMethod]
        public void TestAssemblyInspection()
        {
            var inspector = new ModelInspector();

            // Inspect the HL7.Fhir.Model assembly
            inspector.Import(typeof(Resource).Assembly);
            inspector.Process();

            // Check for presence of some basic ingredients
            Assert.IsNotNull(inspector.FindClassMappingForResource("patient"));
            Assert.IsNotNull(inspector.FindClassMappingForFhirDataType("HumanName"));
            Assert.IsNotNull(inspector.FindClassMappingForFhirDataType("code"));
            Assert.IsNotNull(inspector.FindClassMappingForFhirDataType("boolean"));

            // Verify presence of nested enumerations
            Assert.IsNotNull(inspector.FindClassMappingForFhirDataType("AddressUse"));

            // Should have skipped abstract classes
            Assert.IsNull(inspector.FindClassMappingForResource("Resource"));
            Assert.IsNull(inspector.FindClassMappingForResource("Element"));

            // Code<> should still be there (the only generic type definition accepted)
            var codeOfT = inspector.FindClassMappingByImplementingType(typeof(Code<>));
            Assert.IsNotNull(codeOfT);
            Assert.AreEqual(FhirModelConstruct.PrimitiveType, codeOfT.ModelConstruct);

            // Test presence of a basic member
            // Test presence of a xxxxxElement member

        }

        [TestMethod]
        public void TestCodeOfTInspection()
        {
            var inspector = new ModelInspector();

            var x = typeof(Tester);

            // Inspect the HL7.Fhir.Model assembly
            //inspector.Import(typeof(Code<>));
            //inspector.Import(typeof(Address));
            //inspector.Process();
        }
    }


    class Open<T, X> { }

    class Half<X> : Open<int,X> { }

    class Tester
    {
        public Half<string> Y { get; set; }
    }

    /*
     * Resource classes for tests 
     */
    public class RoadResource {}
    public class Way : Resource {}
    
    [FhirResource("Way", Profile="http://nu.nl/profile#street")]
    public class ProfiledWay {}

    public class Street : Resource {}

    [FhirResource("Street")]
    public class NewStreet : Resource { }


    /* 
     * Datatype classes for tests
     */
    public class AnimalName : ComplexElement { }

    [FhirComplexType("AnimalName")]
    public class NewAnimalName { }

    [FhirPrimitiveType("Complex")]
    public class ComplexNumber : PrimitiveElement { }

    [FhirComplexType("Chameleon")]
    public class Chameleon : PrimitiveElement { }


    [FhirEnumeration("SomeEnum")]
    public enum SomeEnum { Member, AnotherMember }

    public class ActResource
    {
        [FhirEnumeration("SomeOtherEnum")]
        public enum SomeOtherEnum { Member, AnotherMember }
    }
}
