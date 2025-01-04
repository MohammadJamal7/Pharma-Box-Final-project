using Graduation_Project.Controllers;
using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class PharmacistControllerTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly UserManager<ApplicationUser> _mockUserManager;
    private readonly SignInManager<ApplicationUser> _mockSignInManager;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private readonly PharmacistController _controller;

    public PharmacistControllerTests()
    {
        _mockContext = new Mock<ApplicationDbContext>();
        _mockUserManager = MockUserManager(new List<ApplicationUser>());
        _mockSignInManager = MockSignInManager(_mockUserManager);
        _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

        _controller = new PharmacistController(
            _mockWebHostEnvironment.Object,
            _mockContext.Object,
            _mockUserManager,
            Mock.Of<RoleManager<IdentityRole>>(),
            _mockSignInManager
        );
    }

    [Fact]
    public async Task LoginPost_SuccessfulLogin_RedirectsToProfile()
    {
        // Arrange
        var model = new PharmacistLogin { Email = "test@example.com", Password = "Password123!" };
        var signInResult = Microsoft.AspNetCore.Identity.SignInResult.Success;
        object value = _mockSignInManager.Setup(s => s.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false)).ReturnsAsync(signInResult);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Profile", redirectResult.ActionName);
    }

    private static UserManager<TUser> MockUserManager<TUser>(IList<TUser> users) where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        userManager.Setup(x => x.Users).Returns(users.AsQueryable());
        return userManager.Object;
    }

    private static SignInManager<TUser> MockSignInManager<TUser>(UserManager<TUser> userManager) where TUser : class
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        return new SignInManager<TUser>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
    }
}
