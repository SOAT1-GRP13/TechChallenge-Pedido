using Domain.Base.DomainObjects;

namespace Domain.Tests.Base.DomainObjects
{
    public class TestValueObject : ValueObject
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }

        public TestValueObject(string property1, int property2)
        {
            Property1 = property1;
            Property2 = property2;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Property1;
            yield return Property2;
        }
    }

    public class ValueObjectTests
    {
        [Fact]
        public void Equals_GivenDifferentValues_ShouldReturnFalse()
        {
            var valueObject1 = new TestValueObject("Value1", 1);
            var valueObject2 = new TestValueObject("Value2", 2);

            Assert.False(valueObject1.Equals(valueObject2));
        }

        [Fact]
        public void Equals_GivenSameValues_ShouldReturnTrue()
        {
            var valueObject1 = new TestValueObject("Value", 1);
            var valueObject2 = new TestValueObject("Value", 1);

            Assert.True(valueObject1.Equals(valueObject2));
        }

        [Fact]
        public void GetHashCode_GivenSameValues_ShouldReturnSameHashCode()
        {
            var valueObject1 = new TestValueObject("Value", 1);
            var valueObject2 = new TestValueObject("Value", 1);

            Assert.Equal(valueObject1.GetHashCode(), valueObject2.GetHashCode());
        }

        [Fact]
        public void EqualityOperator_GivenSameValues_ShouldReturnTrue()
        {
            var valueObject1 = new TestValueObject("Value", 1);
            var valueObject2 = new TestValueObject("Value", 1);

            Assert.True(valueObject1 == valueObject2);
        }

        [Fact]
        public void InequalityOperator_GivenDifferentValues_ShouldReturnTrue()
        {
            var valueObject1 = new TestValueObject("Value1", 1);
            var valueObject2 = new TestValueObject("Value2", 2);

            Assert.True(valueObject1 != valueObject2);
        }
    }
}
