﻿namespace SWE.EventSourcing.Test.Factories
{
    using FluentAssertions;
    using global::Xunit;
    using SWE.EventSourcing.Extensions;
    using SWE.EventSourcing.Factories;
    using SWE.EventSourcing.Test.Data;
    using SWE.Xunit.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChangeFactoryTest
    {
        [Fact]
        [Category("ChangeFactory")]
        public void ToChangeEvent_Should_ReturnChangeForEachItem()
        {
            var product = ProductFactory.GetProduct("0");
            product.Available.Should().Be(10);

            var product1 = ProductFactory.GetProduct("1");
            var product2 = ProductFactory.GetProduct("2");

            var changes = new List<Product>
            {
                product1,
                product2
            };

            var actual = ChangeFactory.ToChangeEvent<Product, Product, Guid, int>(
                x => x.Available,
                15, changes,
                x => x.Id,
                x => x.InStock)
            .ToList();

            Assert.Equal(changes.Count, actual.Count);
            var change1 = actual[0];
            var change2 = actual.Last();

            change1.Revert(product);
            product.Available.Should().Be(15);

            change2.Apply(product);
            product.Available.Should().Be(10);
        }

        [Fact]
        [Category("ChangeFactory")]
        public void ToOrderedChangeEvent_Should_ReturnChangeForEachItem()
        {
            var product = ProductFactory.GetProduct("0");
            product.Price.Should().BeApproximately(1.0, 0.000001);

            var changes = ProductFactory.GetProductPriceChanges(product.Id);

            var actual = ChangeFactory.ToOrderedChangeEvent<Product, ProductPriceChange, string, double, DateTime>(
                x => x.Price,
                0.8,
                changes,
                x => x.Id,
                x => x.Price,
                x => x.ChangeDate).ToList();

            Assert.Equal(changes.Count, actual.Count);
            var change1 = actual[0];
            var change2 = actual.Last();

            change1.Revert(product);
            product.Price.Should().BeApproximately(0.8, 0.000001);

            change2.Apply(product);
            product.Price.Should().BeApproximately(changes.Last().Price, 0.000001);
        }
    }
}