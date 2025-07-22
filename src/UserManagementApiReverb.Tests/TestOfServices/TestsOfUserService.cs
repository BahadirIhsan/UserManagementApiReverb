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
    
    // ben yazmadım tabiki ama diğerlerini yazmayı öğrendim ama bu çok complex bir yapı gibi geldi.
    [Fact]
    public async Task CreateUserAsync_ShouldRollbackTransaction_WhenExceptionOccurs()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // benzersiz test db
            .Options;

        // Interceptor'u DI'dan geçirmezsek bile override edilen DbContext içine dahil olacaksa test için yeterli
        using var context = new AppDbContext(options);
        var mapper = new Mock<IUserMapper>();
    
        // Email ve Username kontrolünü geçsin diye boş listeyle başlıyoruz
        mapper.Setup(m => m.MapFromRegisterRequest(It.IsAny<UserRequestRegister>(), It.IsAny<string>()))
            .Returns(new User { UserId = Guid.NewGuid(), UserName = "ahmet", Email = "ahmet@example.com" });

        var service = new UserService(context, mapper.Object);

        var request = new UserRequestRegister
        {
            Username = "ahmet",
            Email = "ahmet@example.com",
            Password = "1234"
        };

        // Hatalı durumu simüle etmek için mapper null dönecek (bilinçli hata yaratıyoruz)
        mapper.Setup(m => m.MapFromRegisterRequest(It.IsAny<UserRequestRegister>(), It.IsAny<string>()))
            .Returns((User)null); // buradan sonra service içinde null referansa erişilecek ve hata fırlayacak

        // Act
        Func<Task> act = async () => await service.CreateUserAsync(request);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();

        // Rollback başarılıysa Users tablosu boş olmalı
        context.Users.Count().Should().Be(0);

        // Interceptor eğer çalıştıysa AuditLogs da boş olmalı
        context.AuditLogs.Count().Should().Be(0);
    }

    
    
}