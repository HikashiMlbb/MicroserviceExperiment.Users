using Domain.Users;

namespace DomainTests.Users;

[TestFixture]
public class UserNameTests
{
    // ========== Валидные кейсы ==========
    [Test]
    [TestCase("User1")]
    [TestCase("JohnDoe123")]
    [TestCase("ABCD")]
    [TestCase("A1234567890123456789")] // 20 символов
    public void Create_ValidUsername_ReturnsSuccessResult(string validName)
    {
        var result = UserName.Create(validName);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Value, Is.EqualTo(validName));
        });
    }

    
    
    // ========== Невалидные кейсы (длина) ==========
    [Test]
    public void Create_Null_ReturnsFailureResult()
    {
        var result = UserName.Create(null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserDomainErrors.UsernameIsInvalid));
        });
    }
    
    [Test]
    [TestCase("")]   // пустая строка
    [TestCase("A")]  // 1 символ
    [TestCase("AB")] // 2 символа
    [TestCase("ABC")] // 3 символа
    [TestCase("A12345678901234567890")] // 21 символ (превышение)
    public void Create_InvalidLength_ReturnsFailureResult(string invalidName)
    {
        var result = UserName.Create(invalidName);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserDomainErrors.UsernameIsInvalid));
        });
    }

    // ========== Невалидные кейсы (символы) ==========
    [Test]
    [TestCase("User Name")] // пробел
    [TestCase("User-Name")] // дефис
    [TestCase("User.Name")] // точка
    [TestCase("User@Name")] // спецсимвол
    [TestCase("Юзернейм")]  // не-ASCII
    [TestCase("  User  ")]  // пробелы по краям
    public void Create_InvalidCharacters_ReturnsFailureResult(string invalidName)
    {
        var result = UserName.Create(invalidName);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserDomainErrors.UsernameIsInvalid));
        });
    }

    // ========== Граничные кейсы ==========
    [Test]
    public void Create_MinLength4_WorksCorrectly()
    {
        var result1 = UserName.Create("ABCD"); // 4 символа (минимум)
        var result2 = UserName.Create("ABC");  // 3 символа (должен упасть)
        Assert.Multiple(() =>
        {
            Assert.That(result1.IsSuccess, Is.True);
            Assert.That(result2.IsSuccess, Is.False);
        });
    }

    [Test]
    public void Create_MaxLength20_WorksCorrectly()
    {
        var validName = new string('A', 20); // 20 символов (максимум)
        var invalidName = new string('A', 21); // 21 символ (должен упасть)
        
        var result1 = UserName.Create(validName);
        var result2 = UserName.Create(invalidName);
        Assert.Multiple(() =>
        {
            Assert.That(result1.IsSuccess, Is.True);
            Assert.That(result2.IsSuccess, Is.False);
        });
    }

    // ========== Дополнительные проверки ==========
    [Test]
    public void Create_TrimNotApplied_StrictCheck()
    {
        const string name = "User ";
        var result = UserName.Create(name); // пробел в конце
        
        Assert.That(result.IsSuccess, Is.False); // не должен тримиться автоматически
    }

    [Test]
    public void Create_ValueIsCorrectlyAssigned()
    {
        const string name = "TestUser123";
        var result = UserName.Create(name);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Value, Is.EqualTo(name)); // значение сохраняется точно
        });
    }
}