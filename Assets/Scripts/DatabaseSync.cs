using UnityEngine;
using System.Collections;
using System;
using System.Data;
using MySql.Data.MySqlClient;

public class DatabaseSync : MonoBehaviour {
	private IDbConnection dbcon;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OpenConnection () {
		string connectionString =
			"Server=localhost;" +
				"Database=unity_rv01;" +
				"User ID=root;" +
				"Password=;" +
				"Pooling=false";
		
		dbcon = new MySqlConnection(connectionString);
		dbcon.Open();
	}

	void CloseConnection () {
		dbcon.Close();
		dbcon = null;
	}

	void Test () {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"SELECT firstname, lastname " +
				"FROM employee";
		dbcmd.CommandText = sql;
		IDataReader reader = dbcmd.ExecuteReader();
		while(reader.Read()) {
			string FirstName = (string) reader["firstname"];
			string LastName = (string) reader["lastname"];
			print("Name: " +
			      FirstName + " " + LastName);
		}
		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}

	public void Update (string table, string name, string attribute, string value) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE " + table +  " SET " + attribute + " = '" + value + "' WHERE name = '" + name + "'";
		print (sql);
		dbcmd.CommandText = sql;
		int nbligne = dbcmd.ExecuteNonQuery();
		print ("nb lignes affectées " + nbligne);
		// clean up
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}

	// set or unset user's currently used object
	public void setCurrentObject (string name, int value) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE Object SET isCurrent = " + value + " WHERE name = '" + name + "'";
		print (sql);
		dbcmd.CommandText = sql;
		int nbligne = dbcmd.ExecuteNonQuery();
		print ("nb lignes affectées " + nbligne);
		// clean up
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}

	// get infos about all mockup objects
	public IDataReader getTable (string table_name) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"SELECT * FROM " + table_name;
		dbcmd.CommandText = sql;
		IDataReader reader = dbcmd.ExecuteReader();
		// clean up
		//reader.Close();
		//reader = null;
		//dbcmd.Dispose();
		//dbcmd = null;
		//CloseConnection ();	
		return reader;
	}
}
