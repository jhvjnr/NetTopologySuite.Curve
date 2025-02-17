﻿using NetTopologySuite.Geometries;
using NUnit.Framework;
using System;

namespace NetTopologySuite.Test.Geometries
{
    [TestFixture(0d, 5E-4)]
    [TestFixture(0.001d, 5E-7)]
    public class CompoundCurveImplTest : CurveGeometryImplTest
    {
        public CompoundCurveImplTest(double arcSegmentLength, double lengthTolerance)
            :base(arcSegmentLength, lengthTolerance)
        {
        }

        protected override Geometry CreateGeometry()
        {
            var pts = new[] {
                new Coordinate(-2, 0), new Coordinate(0, 2), new Coordinate(2, 0),
                new Coordinate(4, -2), new Coordinate(6, 0) };

            return Factory.CreateCompoundCurve(new Curve[]
            {
                Factory.CreateCircularString(pts),
                Factory.CreateLineString(new[] {new Coordinate(6, 0), new Coordinate(10, 0)})
            });
        }

        [Test]
        public override void TestIsEmpty()
        {
            var geoms = Array.Empty<Curve>();

            var cc1 = Factory.CreateCompoundCurve();
            var cc2 = Factory.CreateCompoundCurve(geoms);

            Assert.That(cc1.IsEmpty);
            Assert.That(cc2.IsEmpty);
        }

        [Test]
        public void TestQuestionMark()
        {
            // Arrange
            var ca = new Circle(0, 6, 2).GetCircularArc(110, 60, 270);
            var cs = Factory.CreateCircularString(new[] {ca.P0, ca.P1, ca.P2});
            var ls = Factory.CreateLineString(new[] {new Coordinate(0, 4), new Coordinate(0, 2)});

            // Act
            CompoundCurve cc = null;
            Assert.That(() => cc = Factory.CreateCompoundCurve(new Curve[] {cs, ls}), Throws.Nothing);

            Assert.That(cc, Is.Not.Null);

            Assert.That(cc.Length, Is.EqualTo(cs.Length + ls.Length).Within(LengthTolerance));
            Assert.That(cc.Linearize().Length / cc.Length, Is.GreaterThanOrEqualTo(0.98));
        }

        [Test]
        public override void TestIsSimple()
        {
            // Not simple
            var ca = new Circle(0, 12, 4).GetCircularArc(180, 90, 0);
            var geom = Factory.CreateCompoundCurve(new Curve[]
            {
                Factory.CreateCircularString(new[] {ca.P0, ca.P1, ca.P2}),
                Factory.CreateLineString(new[] {ca.P2, new Coordinate(ca.P0.X, ca.P1.Y)}),
            });
            Assert.That(geom.IsSimple, Is.False);

            // Simple
            geom = Factory.CreateCompoundCurve(new Curve[]
            {
                Factory.CreateCircularString(new[] {ca.P0, ca.P1, ca.P2}),
                Factory.CreateLineString(new[] {ca.P2, new Coordinate(0, 8)}),
            });
            Assert.That(geom.IsSimple, Is.True);
        }

        [Test]
        public override void TestIsValid()
        {
            Assert.Inconclusive("Not yet implemented");
        }
    }
}
