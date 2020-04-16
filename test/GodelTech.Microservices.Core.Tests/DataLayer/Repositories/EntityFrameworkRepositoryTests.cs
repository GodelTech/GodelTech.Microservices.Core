using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Core.DataLayer.Repositories;
using GodelTech.Microservices.Core.DataLayer.Specifications;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.DataLayer.Repositories
{
    public class EntityFrameworkRepositoryTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly EntityFrameworkRepository<User> _underTest;

        public EntityFrameworkRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("Add_writes_to_database")
                .Options;

            _context = new TestDbContext(options);

            _context.Users.Add(new User { Id = 1, Name = "Andrei" });
            _context.Users.Add(new User { Id = 2, Name = "Raman" });
            _context.Users.Add(new User { Id = 3, Name = "Stuart" });
            _context.SaveChanges();

            _underTest = new EntityFrameworkRepository<User>(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Theory]
        [InlineData("Andrei", 1)]
        [InlineData("Raman", 2)]
        [InlineData("Stuart", 3)]
        public async Task GetByIdAsync_Should_Return_Entity_By_Id(string expectedResult, int id)
        {
            (await _underTest.GetByIdAsync(id)).Name.Should().Be(expectedResult);
        }

        [Fact]
        public async Task GetByIdAsync_When_Record_Not_Found_Should_Return_Null()
        {
            (await _underTest.GetByIdAsync(123)).Should().BeNull();
        }

        [Theory]
        [InlineData("Stuart", true)]
        [InlineData("Andrei", true)]
        [InlineData("Sveta", false)]
        public async Task ExistsAsync_Should_Return_Result_Matching_Expectations(string pattern, bool expectedResult)
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = pattern
            };

            (await _underTest.ExistsAsync(specification)).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Stuart", 3)]
        [InlineData("Andrei", 1)]
        public async Task GetAsync_When_Result_Is_Not_Empty_Should_Return_Matching_Result(string pattern, int expectedId)
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = pattern
            };

            (await _underTest.GetAsync(specification)).Id.Should().Be(expectedId);
        }

        [Theory]
        [InlineData("Stuart", 3)]
        [InlineData("Andrei", 1)]
        public async Task GetAsync_With_Projection_When_Result_Is_Not_Empty_Should_Return_Matching_Result(string pattern, int expectedId)
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = pattern
            };

            (await _underTest.GetAsync(specification, x => x.Id)).Should().Be(expectedId);
        }

        [Fact]
        public async Task GetAsync_When_Record_Not_Found_Should_Return_Null()
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = "Sveta"
            };

            (await _underTest.GetAsync(specification)).Should().BeNull();
        }

        [Fact]
        public async Task ListAsync_When_Result_Is_Not_Empty_Should_Return_Matching_Items_In_Valid_Sort_Order()
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = "a",
                SortByNameAsc = true
            };

            (await _underTest.ListAsync(specification)).Should().BeEquivalentTo(new[]
            {
                    new User { Id = 2, Name = "Raman" },
                    new User { Id = 3, Name = "Stuart" }
                });
        }

        [Fact]
        public async Task ListAsync_When_Paging_Used_Should_Return_Valid_Portion_Of_Data()
        {
            var specification = new UserSeachSpecification
            {
                SortByNameAsc = false
            };

            (await _underTest.ListAsync(specification, 2, 1)).Should().BeEquivalentTo(new User { Id = 1, Name = "Andrei" });
        }

        [Fact]
        public async Task ListAsync_With_Projection_When_Paging_Used_Should_Return_Valid_Portion_Of_Data()
        {
            var specification = new UserSeachSpecification
            {
                SortByNameAsc = false
            };

            (await _underTest.ListAsync(specification, x => x.Id, 2, 1)).Should().BeEquivalentTo(1);
        }

        [Fact]
        public async Task ListAsync_When_Records_Not_Found_Should_Return_Empty_Result()
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = "Dragon"
            };

            (await _underTest.ListAsync(specification)).Should().BeEmpty();
        }

        [Fact]
        public async Task CountAsync_When_Result_Is_Not_Empty_Should_Return_Matching_Items_In_Valid_Sort_Order()
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = "a",
                SortByNameAsc = true
            };

            (await _underTest.CountAsync(specification)).Should().Be(2);
        }

        [Fact]
        public async Task CountAsync_When_Records_Not_Found_Should_Return_Zero()
        {
            var specification = new UserSeachSpecification
            {
                NamePattern = "Dragon"
            };

            (await _underTest.CountAsync(specification)).Should().Be(0);
        }

        [Fact]
        public async Task AddAsync_Should_Add_New_Record_To_Database()
        {
            var user = new User
            {
                Id = 4,
                Name = "Dragon"
            };

            await _underTest.AddAsync(user);

            (await _underTest.GetByIdAsync(user.Id)).Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task UpdateAsync_Should_Add_New_Record_To_Database()
        {
            var user = await _underTest.GetByIdAsync(1);

            user.Name = "NewName";

            await _underTest.UpdateAsync(user);

            (await _underTest.GetByIdAsync(user.Id)).Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task DeleteAsync_Should_Add_New_Record_To_Database()
        {
            var user = await _underTest.GetByIdAsync(1);

            await _underTest.DeleteAsync(user);

            (await _underTest.GetByIdAsync(user.Id)).Should().BeNull();
        }

        #region Helper classes

        public class TestDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }

            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class UserSeachSpecification : Specification<User>
        {
            public string NamePattern { get; set; }
            public bool SortByNameAsc { get; set; }

            public override IQueryable<User> AddPredicates(IQueryable<User> query)
            {
                if (!string.IsNullOrWhiteSpace(NamePattern))
                    query = query.Where(x => x.Name.Contains(NamePattern));

                return query;
            }

            public override IQueryable<User> AddSorting(IQueryable<User> query)
            {
                return SortByNameAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            }
        }

        #endregion
    }

}
