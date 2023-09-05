using pbn;
using pbn.tokens;

namespace tests;

[TestClass]
public class PbnFileTests
{
    [TestMethod]
    public void TestAppendToken()
    {
        PbnFile file = new PbnFile();
        var token = new EmptyLine();
        file.AppendToken(token);
        Assert.AreEqual(1, file.Tokens.Count);
    }

    [TestMethod]
    public void TestInsertToken()
    {
        PbnFile file = new PbnFile();
        Commentary token1 = new Commentary("Hello comment");
        EmptyLine token2 = new EmptyLine();
        Tag token3 = new Tag("My tag", "3");
        file.AppendToken(token1);
        file.AppendToken(token3);
        file.InsertToken(1, token2);
        Assert.AreEqual(3, file.Tokens.Count);
    }

    [TestMethod]
    public void TestDeleteTokenAt()
    {
        PbnFile file = new PbnFile();
        Tag token1 = new Tag("My tag", "1");
        EmptyLine token2 = new EmptyLine();
        file.AppendToken(token1);
        file.AppendToken(token2);
        file.DeleteTokenAt(0);
        Assert.AreEqual(1, file.Tokens.Count);
    }

    [TestMethod]
    public void TestReplaceToken()
    {
        PbnFile file = new PbnFile();
        CustomEscapedLine token1 = new CustomEscapedLine("Mycrypticline");
        Commentary token2 = new Commentary("Hello comment \n twice as many lines");
        Tag token3 = new Tag("MyTag", "values");
        file.AppendToken(token1);
        file.AppendToken(token2);
        file.ReplaceToken(1, token3);
        Assert.AreEqual(2, file.Tokens.Count);
    }

    [TestMethod]
    public void TestDeleteTokenByReference()
    {
        PbnFile file = new PbnFile();
        Tag token1 = new Tag("MyTag", "values");
        EmptyLine token2 = new EmptyLine();
        file.AppendToken(token1);
        file.AppendToken(token2);
        file.DeleteToken(token1);
        Assert.AreEqual(1, file.Tokens.Count);
    }

    [TestMethod]
    public void SerializationTest()
    {
        // Create a PbnFile object and add some tokens
        PbnFile pbnFile = new PbnFile();

        pbnFile.AppendToken(new Tag("MyTag", "values"));
        pbnFile.AppendToken(new EmptyLine());
        pbnFile.AppendToken(new Tag("MyTagOther", "values 2"));
        pbnFile.AppendToken(new EmptyLine());
        pbnFile.AppendToken(new CustomEscapedLine(" Mycrypticline"));

        // Serialize the PbnFile object to a StringWriter
        using (StringWriter sw = new System.IO.StringWriter())
        {
            pbnFile.Serialize(sw);

            var expected = "[MyTag \"values\"]\n\n[MyTagOther \"values 2\"]\n\n% Mycrypticline\n".ReplaceLineEndings();
            // Verify that the serialized string matches the expected output
            Assert.AreEqual(expected, sw.ToString());
        }
    }
}