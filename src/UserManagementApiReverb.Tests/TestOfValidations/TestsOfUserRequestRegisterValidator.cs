using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.FluentValidation;
using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;
using Xunit;

namespace UserManagementApiReverb.Tests.TestOfValidations;

public class TestsOfUserRequestRegisterValidator
{
    private readonly UserRequestRegisterValidator _validator;
    private readonly AppDbContext _mockDb;

    public TestsOfUserRequestRegisterValidator()
    {
        // In-memory DB bağlamı doğrudan bir appdbcontext örneği oluşturmaya yarar.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _mockDb = new AppDbContext(options);
        // test için örnek veri girişi yaptık sahte db'ye
        _mockDb.Users.Add(new User { Email = "test@example.com", UserName = "test" });
        _mockDb.SaveChanges();
        
        // validator example'ı
        _validator = new UserRequestRegisterValidator(_mockDb);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Already_Exists()
    {
        // arrange

        var request = new UserRequestRegister
        {
            Email = "test@example.com", // zaten veritabanında var
            Username = "baho",
            Password = "12345678",
            FirstName = "bahadir",
            LastName = "herdem"
        };
        
        // act
        var result = _validator.Validate(request);
        
        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "This email address already exists");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var request = new UserRequestRegister
        {
            Email = "", // boş email
            Username = "baho1234567",
            Password = "Bahadir23.",
            FirstName = "bahadir",
            LastName = "herdem"
        }; 
        
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email is required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid_Format()
    {
        var request = new UserRequestRegister
        {
            Email = "gecersiz-format", // invalid format
            Username = "baho",
            Password = "12345678",
            FirstName = "bahadir",
            LastName = "herdem"
        }; 
        
        var result = _validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email format is invalid");
    }

    [Fact] // her şey doğruysa valid olup olmadığı kontrolü
    public void Should_Pass_When_Email_Is_Valid_And_Unique()
    {
        var request = new UserRequestRegister
        {
            Email = "test12345@example.com", // tam anlamıyla doğru olmalı db'de de yok.
            Username = "baho",
            Password = "Bahadir23.",
            FirstName = "bahadir",
            LastName = "herdem"
        }; 
        
        var result = _validator.Validate(request);
        
        result.IsValid.Should().BeTrue();
    }
    
}