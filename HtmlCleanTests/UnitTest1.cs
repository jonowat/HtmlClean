
namespace HtmlCleanTests
{
    using System;
    using System.Collections.Generic;
    using HtmlClean;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ORM;

    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SimpleSelectAll()
        {
            var sql = new Orm().From("table").BuildQuery();
            Assert.AreEqual("SELECT * FROM table", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SimpleSelect()
        {
            var sql = new Orm().From("table").Select("abc").BuildQuery();
            Assert.AreEqual("SELECT abc FROM table", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SimpleSelects()
        {
            var sql = new Orm().From("table").Select("abc", "DEF").BuildQuery();
            Assert.AreEqual("SELECT abc, DEF FROM table", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SimpleSelectsArray()
        {
            var sql = new Orm().From("table").Select(new[] { "abc", "DEF" }).BuildQuery();
            Assert.AreEqual("SELECT abc, DEF FROM table", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_withSimplifiedSyntax()
        {
            var sql = new Orm().From("table").Where("columnName", "abc").BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelectorAndParameters()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).BuildQuery();
            CollectionAssert.AreEqual(new Dictionary<string, string>() { { "param0", "abc" } }, sql.Parameters);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidWithAFewSelectors()
        {
            var sql = new Orm().From("table").Where(And.Where(new Where("columnName1", "abc"), new Where("columnName2", "def"))).BuildQuery();

            Assert.AreEqual("SELECT * FROM table WHERE (columnName1 = @param0 AND columnName2 = @param1)", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidWithAFewOrSelectors()
        {
            var sql = new Orm().From("table").Where(Or.Where(new Where("columnName1", "abc"), new Where("columnName2", "def"))).BuildQuery();

            Assert.AreEqual("SELECT * FROM table WHERE (columnName1 = @param0 OR columnName2 = @param1)", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSqlWithComplexSelectors()
        {
            var sql = new Orm().From("table").Where(
                    And.Where(
                        Or.Where(
                            new Where("columnName0", "abc"), 
                            new Where("columnName1", "def"), 
                            And.Where(
                                new Where("columnName2", "ghi"), 
                                new Where("columnName3", "jkl")
                            )
                        ),
                        new Where("columnName4", "mno")
                    )
                ).BuildQuery();

            Assert.AreEqual("SELECT * FROM table WHERE ((columnName0 = @param0 OR columnName1 = @param1 OR (columnName2 = @param2 AND columnName3 = @param3)) AND columnName4 = @param4)", sql.Sql);
        }


        [TestMethod]
        public void Orm_ShouldcreateValidSql_TakeN()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).Take(2).BuildQuery();
            Assert.AreEqual("SELECT TOP 2 * FROM table WHERE columnName = @param0", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SkipN()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).Skip(2).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0 ORDER BY(SELECT NULL) OFFSET 2 ROWS", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SkipNTakeN()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).Take(2).Skip(2).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0 ORDER BY(SELECT NULL) OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY", sql.Sql);
        }


        [TestMethod]
        public void Orm_ShouldcreateValidSql_SimpleOrderAsc()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).OrderBy("column2").BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0 ORDER BY column2 ASC", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_SimpleOrderDesc()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).OrderByDesc("column2").BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0 ORDER BY column2 DESC", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_MultipleOrders()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", "abc")).OrderBy("column1").OrderByDesc("column2").BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName = @param0 ORDER BY column1 ASC, column2 DESC", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_Select_NoTable()
        {
            var sql = new Orm().Select("abc").BuildQuery();
            Assert.AreEqual("SELECT abc", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_Select_Raw()
        {
            var sql = new Orm().Select(Select.Raw("getDate()")).BuildQuery();
            Assert.AreEqual("SELECT getDate() as getDate", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_Select_RawNamed()
        {
            var sql = new Orm().Select(Select.Raw("getDate()", "date")).BuildQuery();
            Assert.AreEqual("SELECT getDate() as date", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSql_Select_Raws()
        {
            var sql = new Orm().Select(Select.Raw("getDate()"), Select.Raw("'abd'")).BuildQuery();
            Assert.AreEqual("SELECT getDate() as getDate, 'abd' as abd", sql.Sql);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Orm_ShouldcreateValidSql_Select_dangerousNames()
        {
            var sql = new Orm().Select("abc, *").BuildQuery();
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Orm_ShouldcreateValidSql_Select_dangerousNames2()
        {
            var sql = new Orm().Select(new Select("abc", "ab; DELETE * FROM Table")).BuildQuery();
        }


        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelectorWhereNotEqual()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", Where.Comparison.NotEquals, "abc")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName != @param0", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_like()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", Where.Comparison.Like, "abc")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName LIKE @param0", sql.Sql);
        }
        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_likeShort()
        {
            var sql = new Orm().From("table").Where(Where.Like("columnName", "abc")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName LIKE @param0", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_likeWildcards()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", Where.Comparison.Like, "abc*")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName LIKE @param0", sql.Sql);
            Assert.AreEqual("abc%", sql.Parameters["param0"]);
        }
        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_in()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", Where.Comparison.In, "abc", "bde")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName IN (@param0, @param1)", sql.Sql);
        }
        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_in2()
        {
            var sql = new Orm().From("table").Where(new Where("columnName", Where.Comparison.In,new[] { "abc", "bde" })).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName IN (@param0, @param1)", sql.Sql);
            CollectionAssert.AreEqual(new Dictionary<string, string>() { { "param0", "abc" }, { "param1", "bde" } }, sql.Parameters);
        }
        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_isnull()
        {
            var sql = new Orm().From("table").Where(Where.IsNull("columnName")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName IS NULL", sql.Sql);
        }
        [TestMethod]
        public void Orm_ShouldcreateValidSimpleSelector_isNotNull()
        {
            var sql = new Orm().From("table").Where(Where.IsNotNull("columnName")).BuildQuery();
            Assert.AreEqual("SELECT * FROM table WHERE columnName IS NOT NULL", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldFetchSomeData_CorrectQuery()
        {
            var sql = new Orm().From("EditorialCatalog").Take(1).BuildQuery();
            Assert.AreEqual("SELECT TOP 1 * FROM EditorialCatalog", sql.Sql);
        }

        [TestMethod]
        public void Orm_ShouldFetchSomeData()
        {
            var data = new Orm().From("EditorialCatalog").Take(1).Fetch("CONNECTION_STRING");
            Assert.AreNotEqual(0, data.Count);
        }
    }
}
