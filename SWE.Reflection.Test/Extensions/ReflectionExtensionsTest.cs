namespace SWE.Reflection.Test.Extensions
{
    using FluentAssertions;
    using global::Xunit;
    using SWE.Reflection.Extensions;
    using SWE.Reflection.Test.Data;
    using SWE.Xunit.Attributes;
    using System;
    using System.Linq.Expressions;

    public class ReflectionExtensionsTest
    {
        private const int IntPropertyValue = 23;

        private const double DoublePropertyValue = 34.5;

        private const string StringPropertyValue = "Original";

        private static readonly Guid GuidPropertyValue = Guid.NewGuid();

        private static readonly DateTimeOffset DateTimeOffsetPropertyValue = new DateTimeOffset(2018, 1, 2, 3, 4, 5, TimeSpan.FromHours(6));

        private ReflectionStub GetReflectionStub()
        {
            return new ReflectionStub(IntPropertyValue, DoublePropertyValue, StringPropertyValue, GuidPropertyValue, DateTimeOffsetPropertyValue);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void MemberSelector_Should_ReturnSelector()
        {
            var selector = ReflectionExtensions.MemberSelector<ReflectionStub>(typeof(ReflectionStub), nameof(ReflectionStub.IntProperty));
            var item = GetReflectionStub();
            var actual = selector(item);
            Assert.Equal(item.IntProperty, actual);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void MemberSelector_Should_ReturnSelector_WhenItemParsed()
        {
            var item = GetReflectionStub();
            var selector = ReflectionExtensions.MemberSelector<ReflectionStub>(item, nameof(ReflectionStub.IntProperty));
            var actual = selector(item);
            Assert.Equal(item.IntProperty, actual);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void MemberSelector_Should_ThrowException_WhenTypeMismatched()
        {
            var item = GetReflectionStub();
            var selector = ReflectionExtensions.MemberSelector<ReflectionStub>(item, nameof(ReflectionStub.StringProperty));
            var actual = selector(item);
            Assert.NotEqual(item.IntProperty, actual);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpression_Should_UseExpressionToSetValue_When_Int()
        {
            AssertValue(GetReflectionStub(), x => x.IntProperty, 32);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpression_Should_UseExpressionToSetValue_When_Double()
        {
            AssertValue(GetReflectionStub(), x => x.DoubleProperty, 35.4);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpression_Should_UseExpressionToSetValue_When_String()
        {
            AssertValue(GetReflectionStub(), x => x.StringProperty, "New");
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpression_Should_UseExpressionToSetValue_When_Guid()
        {
            AssertValue(GetReflectionStub(), x => x.GuidProperty, Guid.NewGuid());
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpression_Should_UseExpressionToSetValue_When_DateTimeOffset()
        {
            AssertValue(GetReflectionStub(), x => x.DateTimeOffsetProperty, new DateTimeOffset(2018, 1, 2, 3, 4, 5, TimeSpan.FromHours(5)));
        }

        private static void AssertValue<TValue>(
            ReflectionStub reflectionStub,
            Expression<Func<ReflectionStub, TValue>> selectorExpression,
            TValue expected)
            where TValue : IEquatable<TValue>
        {
            var selector = selectorExpression.Compile();
            var original = selector(reflectionStub);
            Assert.NotEqual(original, expected);

            var setterExpression = selectorExpression.SetValueExpression();
            setterExpression.Compile()(reflectionStub, expected);
            var actual = selector(reflectionStub);
            Assert.NotEqual(original, actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpressionWithValue_Should_UseExpressionToSetValue_When_Int()
        {
            AssertSetValueExpression(GetReflectionStub(), x => x.IntProperty, 32);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpressionWithValue_Should_UseExpressionToSetValue_When_Double()
        {
            AssertValue(GetReflectionStub(), x => x.DoubleProperty, 35.4);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpressionWithValue_Should_UseExpressionToSetValue_When_String()
        {
            AssertSetValueExpression(GetReflectionStub(), x => x.StringProperty, "New");
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpressionWithValue_Should_UseExpressionToSetValue_When_Guid()
        {
            AssertSetValueExpression(GetReflectionStub(), x => x.GuidProperty, Guid.NewGuid());
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void SetValueExpressionWithValue_Should_UseExpressionToSetValue_When_DateTimeOffset()
        {
            AssertSetValueExpression(GetReflectionStub(), x => x.DateTimeOffsetProperty, new DateTimeOffset(2018, 1, 2, 3, 4, 5, TimeSpan.FromHours(5)));
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void GetMemberInfo_Should_ReturnMemberInfo()
        {
            var actual = ReflectionExtensions.GetMemberInfo<ReflectionStub, int>(x => x.IntProperty);
            Assert.Equal(nameof(ReflectionStub.IntProperty), actual.Member.Name);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void GetMemberInfo_Should_ReturnMemberInfo_WhenNsted()
        {
            var actual = ReflectionExtensions.GetMemberInfo<ReflectionStub, int>(x => x.Reflection.IntProperty);
            Assert.Equal(nameof(ReflectionStub.IntProperty), actual.Member.Name);
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void GetMemberInfo_Should_ThrowArgumentNullException()
        {
            Action action = () => ReflectionExtensions.GetMemberInfo<ReflectionStub, int>(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [Category("ReflectionExtensions")]
        public void GetMemberInfo_Should_ThrowArgumentException()
        {
            Action action = () => ReflectionExtensions.GetMemberInfo<ReflectionStub, bool>(x => x.IntProperty == 2);
            action.Should().Throw<ArgumentException>();
        }

        private static void AssertSetValueExpression<TValue>(ReflectionStub reflectionStub, Expression<Func<ReflectionStub, TValue>> selectorExpression, TValue expected)
            where TValue : IEquatable<TValue>
        {
            var selector = selectorExpression.Compile();
            var original = selector(reflectionStub);
            Assert.NotEqual(original, expected);

            var setterExpression = selectorExpression.SetValueExpression(expected);
            setterExpression.Compile()(reflectionStub);
            var actual = selector(reflectionStub);
            Assert.NotEqual(original, actual);
            Assert.Equal(expected, actual);
        }
    }
}