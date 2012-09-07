
using System;
using System.IO;

using Common;
using Interop.SQLite;

class Person
{
	public string Name { get; set; }
}

static class Program
{
	class User
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return String.Format("ID: {0}, Name: {1}", ID, Name);
		}
	}

	static void Main()
	{
		string filename = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
			"test.sqlite"
		);

		using (SQLite3 db = new SQLite3(filename))
		{
			try
			{
				using (var transaction = db.BeginTransaction())
				{
					db.Query("CREATE TABLE User ( ID INTEGER PRIMARY KEY, Name TEXT UNIQUE );");

					db.Query("INSERT INTO User (Name) VALUES ('John')");
					db.Query("INSERT INTO User (Name) VALUES ('Jeff')");
					db.Query("INSERT INTO User (Name) VALUES ('Jesus')");

					transaction.Commit();
				}
			}
			catch (SQLite3Exception ex)
			{
				Console.WriteLine("EXCEPTION: " + ex.Message);
			}

			using (var enumerator = db.QueryEnumerator<User>("SELECT ID, Name FROM User"))
			{
				enumerator.MoveNext();
				enumerator.MoveNext();
				Console.WriteLine(enumerator.Current);

				enumerator.Reset();
				while (enumerator.MoveNext())
					Console.WriteLine(enumerator.Current);
			}
		}
	}
}
