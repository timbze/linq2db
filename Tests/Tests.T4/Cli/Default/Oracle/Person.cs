// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------

using LinqToDB.Mapping;

#pragma warning disable 1573, 1591
#nullable enable

namespace Cli.Default.Oracle
{
	[Table("Person")]
	public class Person
	{
		[Column("PersonID"  , IsPrimaryKey = true )] public decimal PersonId   { get; set; } // NUMBER
		[Column("FirstName" , CanBeNull    = false)] public string  FirstName  { get; set; } = null!; // VARCHAR2(50)
		[Column("LastName"  , CanBeNull    = false)] public string  LastName   { get; set; } = null!; // VARCHAR2(50)
		[Column("MiddleName"                      )] public string? MiddleName { get; set; } // VARCHAR2(50)
		[Column("Gender"                          )] public char    Gender     { get; set; } // CHAR(1)

		#region Associations
		/// <summary>
		/// Fk_Doctor_Person backreference
		/// </summary>
		[Association(ThisKey = nameof(PersonId), OtherKey = nameof(Doctor.PersonId))]
		public Doctor? FkDoctor { get; set; }

		/// <summary>
		/// Fk_Patient_Person backreference
		/// </summary>
		[Association(ThisKey = nameof(PersonId), OtherKey = nameof(Patient.PersonId))]
		public Patient? FkPatient { get; set; }
		#endregion
	}
}
