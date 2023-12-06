using Microsoft.EntityFrameworkCore;
using Moq;
using OnlineCatalog.Data;
using OnlineCatalog.Repository;
using OnlineCatalog.Models;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Tests.RepositoryTests
{
    public class AbsenceRepositoryTests
    {
        private Mock<AppDbContext> _mockContext;
        private DbSet<Absence> _mockDbSet;
        private IAbsenceRepository _repository;
        private List<Absence> _absences = new List<Absence>
        {
            new()
            {
                AbsenceId = 1, Subject = new Subject { SubjectId = 1 }, Pupil = new Pupil{Id = 1},Date = new DateTime(2023, 06, 19)
            }
        };

        public AbsenceRepositoryTests()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockDbSet = MockDbSetHelper.CreateMockDbSet(_absences);

            _mockContext.Setup(c => c.Set<Absence>()).Returns(_mockDbSet);

            _repository = new AbsenceRepository(_mockContext.Object);
        }

        [Fact]
        public async Task AddAsync_AddsAnAbsenceToDbSet()
        {
            //Arrange
            var absence = new Absence();

            //Act
            await _repository.AddAsync(absence);

            //Assert
            Assert.Contains(absence, _absences);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAnAbsenceInContext()
        {
            // Arrange
            var absence = new Absence();

            // Act
            await _repository.UpdateAsync(absence);

            // Assert
            _mockContext.Verify(context => context.Update(absence), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntityFromDbSet()
        {
            // Arrange
            var absence = _absences.First();

            // Act
            await _repository.DeleteAsync(absence);

            // Assert
            Assert.DoesNotContain(absence, _absences);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntitiesFromDbSet()
        {
            // Arrange
            var entities = new List<Absence>
            {
                new() { AbsenceId = 1 },
            };
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(entities.Count, result.Count);
        }

        [Fact]
        public async Task GetAbsencesByPupilIdAndSubjectId_ReturnsAListOfAbsences()
        {
            //Act
            var result = await _repository.GetAbsencesByPupilIdAndSubjectId(1, 1);

            //Assert
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public async Task GetAbsencesForTheCurrentYearByPupilId_ReturnsAListOfAbsences()
        {
            //Act
            var result =
                await _repository.GetAbsencesForTheCurrentYearByPupilId(1, new DateTime(2023, 06, 10),
                    new DateTime(2023, 06, 20));

            //Assert
            Assert.Equal(1, result.Count);
        }

    }
}
