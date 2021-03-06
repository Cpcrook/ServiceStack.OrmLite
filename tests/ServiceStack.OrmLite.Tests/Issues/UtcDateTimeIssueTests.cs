﻿using System;
using NUnit.Framework;
using ServiceStack.Text;

namespace ServiceStack.OrmLite.Tests.Issues
{
    public class TestDate
    {
        public string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    [TestFixture]
    public class UtcDateTimeIssueTests : OrmLiteTestBase
    {
        [Test]
        public void Test_DateTime_Select()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<TestDate>();

                DateTime.UtcNow.ToJson().Print();

                db.Insert(new TestDate {
                    Name = "Test name", 
                    ExpiryDate = DateTime.UtcNow.AddHours(1)
                });

                //db.GetLastSql().Print();

                var result = db.Select<TestDate>(q => q.ExpiryDate > DateTime.UtcNow);
                db.GetLastSql().Print();

                Assert.That(result.Count, Is.EqualTo(1));

                //db.Select<TestDate>(q => q.ExpiryDate > DateTime.Now);
                //db.GetLastSql().Print();

                //db.Select<TestDate>().PrintDump();
            }
        }

        [Test]
        public void Can_Select_DateTime_with_SelectFmt()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<TestDate>();

                db.Insert(new TestDate
                {
                    Name = "1999",
                    ExpiryDate = new DateTime(1999, 01, 01)
                });
                db.Insert(new TestDate
                {
                    Name = "2000",
                    ExpiryDate = new DateTime(2000, 01, 01)
                });
                db.Insert(new TestDate
                {
                    Name = "Test name",
                    ExpiryDate = DateTime.UtcNow.AddHours(1)
                });

                var result = db.Select<TestDate>("ExpiryDate".SqlColumn() + " > @date",
                    new { date = DateTime.UtcNow });
                db.GetLastSql().Print();
                Assert.That(result.Count, Is.EqualTo(1));

                result = db.Select<TestDate>("ExpiryDate".SqlColumn() + " > @date",
                    new { date = new DateTime(1999, 01, 02) });
                db.GetLastSql().Print();
                Assert.That(result.Count, Is.EqualTo(2));
            }
        }
    }
}