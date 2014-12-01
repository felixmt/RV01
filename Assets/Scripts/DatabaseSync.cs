using UnityEngine;
using System.Collections;
using System;
using System.Data;
using MySql.Data.MySqlClient;

public class DatabaseSync : MonoBehaviour {
	private IDbConnection dbcon;

	// Use this for initialization
	void Start () {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE Object SET isCurrent = 0";
		dbcmd.CommandText = sql;
		int nbligne = dbcmd.ExecuteNonQuery();
		// clean up
		dbcmd.Dispose();
		dbcmd = null;

		dbcmd = dbcon.CreateCommand();
		sql = "UPDATE Object SET current_marker_id = null";
		dbcmd.CommandText = sql;
		nbligne = dbcmd.ExecuteNonQuery();
		// clean up
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OpenConnection () {
		// connexion locale
		string connectionString =
			"Server=localhost;" +
				"Database=unity_rv01;" +
				"User ID=root;" +
				"Password=;" +
				"Pooling=false";

		// connexion motot.eu
		/*string connectionString = 
			"Server=http://felix.motot.eu/;" +
				"Database=alp_rv01;" +
				"User ID=alp_felix;" +
				"Password=21340_Fel;" +
				"Pooling=false";*/
		
		dbcon = new MySqlConnection(connectionString);
		dbcon.Open();
	}

	void CloseConnection () {
		dbcon.Close();
		dbcon = null;
	}

	/*public void Update (string table, string name, string attribute, string value) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE " + table +  " SET " + attribute + " = '" + value + "' WHERE name = '" + name + "'";
		dbcmd.CommandText = sql;
		int nbligne = dbcmd.ExecuteNonQuery();
		// clean up
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}*/

	// set or unset user's currently used object
	public void setCurrentObject (string name, int value) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE Object SET isCurrent = " + value + " WHERE name = '" + name + "'";
		dbcmd.CommandText = sql;
		int nbligne = dbcmd.ExecuteNonQuery();
		// clean up
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}

	public void setCurrentMarker (string objectName, string markerName) {
		int id_marker = 0;
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql = "SELECT id FROM marker WHERE name = '" + markerName + "'";
		dbcmd.CommandText = sql;
		IDataReader reader = dbcmd.ExecuteReader();
		while(reader.Read()) {
			id_marker = (int) reader["id"];
		}
		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;

		if (id_marker != 0) {
			dbcmd = dbcon.CreateCommand();
			sql = "UPDATE Object SET current_marker_id = " + id_marker + " WHERE name = '" + objectName + "'";
			dbcmd.CommandText = sql;
			int nbligne = dbcmd.ExecuteNonQuery();
			// clean up
			dbcmd.Dispose();
			dbcmd = null;
		}
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

	// get description of an object
	public string getObjectDescription (string object_name) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql = "SELECT description FROM Object WHERE name = '" + object_name + "'";
		dbcmd.CommandText = sql;
		IDataReader reader = dbcmd.ExecuteReader();
		string desc = "";
		while(reader.Read()) {
			desc = (string) reader["description"];
		}
		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();

		return desc;
	}
}
