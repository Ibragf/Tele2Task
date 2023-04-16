using AutoFixture;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using test_task.Models;
using test_task.Services;
using test_task.Services.DataProviderService;
using test_task.Services.ResidentsClient.ResidentsClient;
using test_task.Tests.Models;

namespace test_task.Tests
{
    public class DataProviderTests
    {
        private static Fixture _fixture = new Fixture();

        private Mock<IResidentsClient> _mockClient;
        private IDataProvider _dataProvider;

        private IEnumerable<ResidentWithoutAge> _residents;
        private IEnumerable<Resident> _residentsWithAge;

        [SetUp]
        public void Setup()
        {
            Random rand = new Random();

            _residents = _fixture.CreateMany<ResidentWithoutAge>(10);
            _residentsWithAge = _residents.Select(r => new Resident { Id = r.Id, Name = r.Name, Sex = r.Sex, Age = rand.Next(0, 70) });

            var json = JsonSerializer.Serialize(_residents);

            _mockClient = new Mock<IResidentsClient>();
            _mockClient.Setup(m => m.GetResidentsAsync()).ReturnsAsync(json);
            _mockClient.Setup(m => m
                .GetResidentByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => JsonSerializer.Serialize(
                    _residentsWithAge.First(x => x.Id == id)));

            _dataProvider = new DataProvider(_mockClient.Object);
        }

        [Test]
        public async Task GIVEN_ResidentsClient_WHEN_GetResidents_method_is_invoked_THEN_all_residents_is_returned()
        {
            //Act
            var residents = await _dataProvider.GetResidentsAsync();
            var actual = _residents.ExceptBy(residents.Select(x => x.Id), x => x.Id).Count();

            //Assert
            Assert.That(actual, Is.EqualTo(0));
        }

        [Test]
        public async Task GIVEN_ResidentsClient_WHEN_GetResidents_method_is_invoked_THEN_all_residents_with_age_is_returned()
        {
            //Act
            var residents = await _dataProvider.GetResidentsAsync();

            //Assert
            Assert.That(() => !residents.Any(r => r.Age == null));
        }

        [Test]
        public async Task GIVEN_ResidentsClient_WHEN_GetResidents_method_is_invoked_THEN_2_residents_values_is_correct()
        {
            // Arrange
            var jsonString = "[{\"id\":\"1\",\"name\":\"John\",\"sex\":\"male\"},{\"id\":\"2\",\"name\":\"Jane\", \"sex\":\"female\"}]";
            var johnJson = "{\"id\":\"1\",\"name\":\"John\",\"age\":30}";
            var janeJson = "{\"id\":\"2\",\"name\":\"Jane\",\"age\":25}";
            _mockClient.Setup(m => m.GetResidentsAsync()).ReturnsAsync(jsonString);
            _mockClient.Setup(m => m.GetResidentByIdAsync("1")).ReturnsAsync(johnJson);
            _mockClient.Setup(m => m.GetResidentByIdAsync("2")).ReturnsAsync(janeJson);

            // Act
            var residents = await _dataProvider.GetResidentsAsync();
            var listResidents = residents.ToList();

            // Assert
            Assert.IsInstanceOf<List<Resident>>(listResidents);
            Assert.That(listResidents.Count(), Is.EqualTo(2));
            Assert.That(listResidents[0].Id, Is.EqualTo("1"));
            Assert.That(listResidents[0].Name, Is.EqualTo("John"));
            Assert.That(listResidents[0].Age, Is.EqualTo(30));
            Assert.That(listResidents[0].Sex, Is.EqualTo("male"));
            Assert.That(listResidents[1].Id, Is.EqualTo("2"));
            Assert.That(listResidents[1].Name, Is.EqualTo("Jane"));
            Assert.That(listResidents[1].Age, Is.EqualTo(25));
            Assert.That(listResidents[1].Sex, Is.EqualTo("female"));
        }
    }
}
