using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.BusinessLayer.UserServices;
using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;
using Xunit;
using MockQueryable.Moq;

namespace UserManagementApiReverb.Tests.TestOfServices;

public class TestsOfUserService
{
    private readonly Mock<AppDbContext> _mockDb;
    private readonly Mock<IUserMapper> _mockMapper;
    private readonly UserService _service;

    public TestsOfUserService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        _mockDb = new Mock<AppDbContext>(options);
        _mockMapper = new Mock<IUserMapper>();
        _service = new UserService(_mockDb.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowException_IfEmailExists()
    {
        // Arrange
        var existingUser = new User { Email = "test@example.com" };
        var fakeUsers = new List<User> { existingUser }.AsQueryable();

        var mockUserDbSet = fakeUsers.BuildMockDbSet(); // Bu DbSet<User> gibi çalışır

        _mockDb.Setup(db => db.Users).Returns(mockUserDbSet.Object);

        var request = new UserRequestRegister
        {
            Email = "test@example.com",
            Username = "testUser",
            Password = "1234"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUserAsync(request));

        exception.Message.Should().Be("Email already exists");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowException_IfUsernameExists()
    {
        var existingUser = new User { UserName = "test" };
        var fakeUsers = new List<User> { existingUser }.AsQueryable();
        
        var mockUserDbSet = fakeUsers.BuildMockDbSet();
        
        _mockDb.Setup(db => db.Users).Returns(mockUserDbSet.Object);

        var request = new UserRequestRegister
        {
            Email = "test@example.com",
            Username = "test",
            Password = "1234"
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUserAsync(request));
        exception.Message.Should().Be("Username already exists");
    }
    
    
}