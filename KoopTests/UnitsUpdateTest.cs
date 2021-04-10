using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Koop.Mapper;
using Koop.Models;
using Koop.Models.Repositories;
using NUnit.Framework;

namespace KoopTests
{
    [TestFixture]
    public class UnitsUpdateTest
    {
        private IGenericUnitOfWork _uow;
        
        [SetUp]
        public void Setup()
        {
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfiles());
            });
            IMapper mapper = mapperConfig.CreateMapper();

            _uow = new GenericUnitOfWork(new KoopDbContext(), mapper);
        }

        [Order(1)]
        [TestCaseSource(nameof(GenUnits))]
        public void CreateUnit(IEnumerable<Unit> units)
        {
            _uow.ShopRepository().UpdateUnits(units);
            _uow.SaveChanges();

            var unitNames = units.Select(p => p.UnitId).ToArray();

            foreach (var name in unitNames)
            {
                Assert.NotNull(_uow.Repository<Unit>().GetAll().SingleOrDefault(p => p.UnitId == name));
            }
        }
        
        [Order(2)]
        [TestCaseSource(nameof(ChangeUnits))]
        public void ChangeUnit(IEnumerable<Unit> units)
        {
            _uow.ShopRepository().UpdateUnits(units);
            _uow.SaveChanges();

            var unitNames = units.Select(p => p.UnitName).ToArray();

            foreach (var name in unitNames)
            {
                Assert.NotNull(_uow.Repository<Unit>().GetAll().SingleOrDefault(p => p.UnitName == name));
            }
        }
        
        [Order(3)]
        [TestCaseSource(nameof(ChangeUnits))]
        public void RemoveUnit(IEnumerable<Unit> units)
        {
            _uow.ShopRepository().RemoveUnits(units);
            _uow.SaveChanges();

            var unitNames = units.Select(p => p.UnitId).ToArray();

            foreach (var name in unitNames)
            {
                Assert.Null(_uow.Repository<Unit>().GetAll().SingleOrDefault(p => p.UnitId == name));
            }
        }

        [TearDown]
        public void Cleanup()
        {
            _uow.DbContext.Dispose();
        }

        private static IEnumerable<IEnumerable<Unit>> GenUnits
        {
            get
            {
                yield return new List<Unit>()
                {
                    new Unit() {UnitId = Guid.Parse("00000000-0000-0000-0000-000000000011"), UnitName = "mile"},
                    new Unit() {UnitId = Guid.Parse("00000000-0000-0000-0000-000000000012"), UnitName = "parsek"}
                };
            }
        }
        
        private static IEnumerable<IEnumerable<Unit>> ChangeUnits
        {
            get
            {
                yield return new List<Unit>()
                {
                    new Unit() {UnitId = Guid.Parse("00000000-0000-0000-0000-000000000011"), UnitName = "miles"},
                    new Unit() {UnitId = Guid.Parse("00000000-0000-0000-0000-000000000012"), UnitName = "parseks"}
                };
            }
        }
    }
}