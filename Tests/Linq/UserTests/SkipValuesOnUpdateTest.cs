﻿using System.Linq;
using LinqToDB;
using LinqToDB.Mapping;
using NUnit.Framework;

namespace Tests.UserTests
{
	public class SkipValuesOnUpdateTest : TestBase
	{
		[Table("PR_1598_Update_Table")]
		public class TestTable
		{
			[Column("Id"), PrimaryKey]
			public int Id { get; set; }
			[Column("Name"), SkipValuesOnUpdate("John")]
			public string? Name { get; set; }
			[Column("Age"), SkipValuesOnUpdate(2, 5)]
			public int? Age { get; set; }
		}

		[Table("PR_1598_Update_Null_Table")]
		public class TestTableNull
		{
			[Column("Id"), PrimaryKey]
			public int Id { get; set; }
			[Column("Name")]
			public string? Name { get; set; }
			[Column("Age"), SkipValuesOnUpdate(null)]
			public int? Age { get; set; }
		}

		[Table("PR_1598_Update_Fluent_Table")]
		public class TestTableFluent
		{
			[Column("Id"), PrimaryKey]
			public int Id { get; set; }
			[Column("Name")]
			public string? Name { get; set; }
			[Column("Age")]
			public int? Age { get; set; }
		}

		[Table("PR_1598_Update_Enum_Table")]
		public class TestTableEnum
		{
			public enum GenderType
			{
				[MapValue(null)]
				Undefined,
				[MapValue("Male")]
				Male,
				[MapValue("Female")]
				Female
			}

			[Column("Id"), PrimaryKey]
			public int Id { get; set; }
			[Column("Name")]
			public string? Name { get; set; }
			[Column("Age")]
			public int? Age { get; set; }
			[Column("Gender"), SkipValuesOnUpdate(GenderType.Female)]
			public GenderType Gender { get; set; }
		}

		[Test]
		public void TestSkipString([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			{
				using (db.CreateLocalTable<TestTable>())
				{
					var count = db.Insert(new TestTable() { Id = 1, Name = "Manuel", Age = 14 });

					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);

					var r = db.GetTable<TestTable>().FirstOrDefault(t => t.Id == 1)!;

					Assert.IsNotNull(r);
					Assert.AreEqual(r.Name, "Manuel");

					r.Name = "Jacob";
					r.Age = 15;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTable>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 15);
					Assert.AreEqual(r.Name, "Jacob");

					r.Name = "John";
					r.Age = 22;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTable>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 22);
					Assert.AreEqual(r.Name, "Jacob");
				}
			}
		}

		[Test]
		public void TestSkipInt32([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			{
				using (db.CreateLocalTable<TestTable>())
				{

					var count = db.Insert(new TestTable() { Id = 1, Name = "Smith", Age = 2 });

					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);

					var r = db.GetTable<TestTable>().FirstOrDefault(t => t.Id == 1)!;

					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 2);

					r.Name = "Franki";
					r.Age = 15;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTable>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 15);
					Assert.AreEqual(r.Name, "Franki");

					r.Name = "Jack";
					r.Age = 2;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTable>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 15);
					Assert.AreEqual(r.Name, "Jack");
				}
			}
		}

		[Test]
		public void TestSkipNull([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			{
				using (db.CreateLocalTable<TestTableNull>())
				{
					var count = db.Insert(new TestTableNull() { Id = 1, Name = "Tommy", Age = null });

					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					var r = db.GetTable<TestTableNull>().FirstOrDefault(t => t.Id == 1)!;

					Assert.IsNotNull(r);
					Assert.IsNull(r.Age);

					r.Name = "Jack";
					r.Age = 2;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTableNull>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 2);
					Assert.AreEqual(r.Name, "Jack");

					r.Name = "Franki";
					r.Age = null;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTableNull>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 2);
					Assert.AreEqual(r.Name, "Franki");
				}
			}
		}
		[Test]
		public void TestSkipWithFluentBuilder([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			{
				var mapping = db.MappingSchema.GetFluentMappingBuilder();
				mapping.Entity<TestTableFluent>().HasSkipValuesOnUpdate(t => t.Age, 2, 5);
				using (db.CreateLocalTable<TestTableFluent>())
				{
					var count = db.Insert(new TestTableFluent() { Id = 1, Name = null, Age = 2 });

					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);

					var r = db.GetTable<TestTableFluent>().FirstOrDefault(t => t.Id == 1)!;

					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 2);

					r.Name = "Franki";
					r.Age = 18;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTableFluent>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 18);
					Assert.AreEqual(r.Name, "Franki");

					r.Name = "Jack";
					r.Age = 2;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTableFluent>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 18);
					Assert.AreEqual(r.Name, "Jack");
				}
			}
		}

		[Test]
		public void TestSkipWithEnum([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			{
				using (db.CreateLocalTable<TestTableEnum>())
				{
					var count = db.Insert(new TestTableEnum() { Id = 1, Name = "Max", Age = 20, Gender = TestTableEnum.GenderType.Female });

					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);

					var r = db.GetTable<TestTableEnum>().FirstOrDefault(t => t.Id == 1)!;

					Assert.IsNotNull(r);
					Assert.AreEqual(r.Gender, TestTableEnum.GenderType.Female);

					r.Name = "Jack";
					r.Age = 2;
					r.Gender = TestTableEnum.GenderType.Male;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTableEnum>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 2);
					Assert.AreEqual(r.Name, "Jack");
					Assert.AreEqual(r.Gender, TestTableEnum.GenderType.Male);

					r.Name = "Francine";
					r.Age = 20;
					r.Gender = TestTableEnum.GenderType.Female;
					count = db.Update(r);
					if (!context.IsAnyOf(TestProvName.AllClickHouse))
						Assert.Greater(count, 0);
					r = db.GetTable<TestTableEnum>().FirstOrDefault(t => t.Id == 1)!;
					Assert.IsNotNull(r);
					Assert.AreEqual(r.Age, 20);
					Assert.AreEqual(r.Name, "Francine");
					Assert.AreEqual(r.Gender, TestTableEnum.GenderType.Male);
				}
			}
		}
	}
}
