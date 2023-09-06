using pbn;
using pbn.tokens;

namespace tests;

[TestClass]
public class PbnFileTests
{
    [TestMethod]
    public void TestAppendToken()
    {
        var file = new PbnFile();
        var token = new EmptyLine();
        file.AppendToken(token);
        Assert.AreEqual(1, file.Tokens.Count);
    }

    [TestMethod]
    public void TestInsertToken()
    {
        var file = new PbnFile();
        var token1 = new Commentary("Hello comment");
        var token2 = new EmptyLine();
        var token3 = new Tag("My tag", "3");
        file.AppendToken(token1);
        file.AppendToken(token3);
        file.InsertToken(1, token2);
        Assert.AreEqual(3, file.Tokens.Count);
    }

    [TestMethod]
    public void TestDeleteTokenAt()
    {
        var file = new PbnFile();
        var token1 = new Tag("My tag", "1");
        var token2 = new EmptyLine();
        file.AppendToken(token1);
        file.AppendToken(token2);
        file.DeleteTokenAt(0);
        Assert.AreEqual(1, file.Tokens.Count);
    }

    [TestMethod]
    public void TestReplaceToken()
    {
        var file = new PbnFile();
        var token1 = new CustomEscapedLine("Mycrypticline");
        var token2 = new Commentary("Hello comment \n twice as many lines");
        var token3 = new Tag("MyTag", "values");
        file.AppendToken(token1);
        file.AppendToken(token2);
        file.ReplaceToken(1, token3);
        Assert.AreEqual(2, file.Tokens.Count);
    }

    [TestMethod]
    public void TestDeleteTokenByReference()
    {
        var file = new PbnFile();
        var token1 = new Tag("MyTag", "values");
        var token2 = new EmptyLine();
        file.AppendToken(token1);
        file.AppendToken(token2);
        file.DeleteToken(token1);
        Assert.AreEqual(1, file.Tokens.Count);
    }

    [TestMethod]
    public void SerializationTest()
    {
        // Create a PbnFile object and add some tokens
        var pbnFile = new PbnFile();

        pbnFile.AppendToken(new Tag("MyTag", "values"));
        pbnFile.AppendToken(new EmptyLine());
        pbnFile.AppendToken(new Tag("MyTagOther", "values 2"));
        pbnFile.AppendToken(new EmptyLine());
        pbnFile.AppendToken(new CustomEscapedLine(" Mycrypticline"));

        // Serialize the PbnFile object to a StringWriter
        using var sw = new StringWriter();
        pbnFile.Serialize(sw);

        var expected = "[MyTag \"values\"]\n\n[MyTagOther \"values 2\"]\n\n% Mycrypticline\n".ReplaceLineEndings();
        // Verify that the serialized string matches the expected output
        Assert.AreEqual(expected, sw.ToString());
    }
}