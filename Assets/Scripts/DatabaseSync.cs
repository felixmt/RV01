using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Data;
using MySql.Data.MySqlClient;

public class DatabaseSync : MonoBehaviour {
	private IDbConnection dbcon;
	
	// Use this for initialization
	void Start () {
		//string text = System.IO.File.ReadAllText(@".\bdd.txt");
		//print (text);

		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE Object SET isCurrent = 0";
		dbcmd.CommandText = sql;
		dbcmd.ExecuteNonQuery();
		// clean up
		dbcmd.Dispose();
		dbcmd = null;

		dbcmd = dbcon.CreateCommand();
		sql = "UPDATE Object SET current_marker_id = null";
		dbcmd.CommandText = sql;
		dbcmd.ExecuteNonQuery();
		// clean up
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OpenConnection () {
		// get connexion infos from file 
		string line;
		string[] dbInfo = new String[2];
		int counter = 0;
		System.IO.StreamReader file = 
			new System.IO.StreamReader(@".\bdd.txt");
		while((line = file.ReadLine()) != null)
		{
			dbInfo[counter] = line;
			counter++;
		}
		file.Close();
		// connexion locale
		string connectionString =
			"Server=localhost;" +
				"Database=unity_rv01;" +
				"User ID="+dbInfo[0]+";" +
				"Password="+dbInfo[1]+";" +
				"Pooling=false";
		
		dbcon = new MySqlConnection(connectionString);
		dbcon.Open();
	}

	void CloseConnection () {
		dbcon.Close();
		dbcon = null;
	}

	// set or unset user's currently used object
	public void setIsCurrent (int order, int value) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"UPDATE Object SET isCurrent = " + value + " WHERE `scenario_id` = 1 AND `order` = " + order;
		dbcmd.CommandText = sql;
		dbcmd.ExecuteNonQuery();
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
			dbcmd.ExecuteNonQuery();
			// clean up
			dbcmd.Dispose();
			dbcmd = null;
		}
		CloseConnection ();	
	}

	// get infos about all mockup objects
	public IDataReader getTableByName (string table_name) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"SELECT * FROM " + table_name + " WHERE scenario_id = 1";
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
	public string getObjectDescription (int order) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql = "SELECT description FROM Object WHERE `order` = " + order;
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

	// get coord x & z of an object
	public float[] getObjectCoord (int order) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql = "SELECT posX, posZ FROM Object WHERE `order` = " + order;
		dbcmd.CommandText = sql;
		IDataReader reader = dbcmd.ExecuteReader();
		float[] coord = new float[2];
		while(reader.Read()) {
			coord[0] = (float) reader["posX"];
			coord[1] = (float) reader["posZ"];
		}
		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		CloseConnection ();
		
		return coord;
	}

	public IDataReader getObjectByOrder (int order) {
		OpenConnection ();
		IDbCommand dbcmd = dbcon.CreateCommand();
		string sql =
			"SELECT * FROM Object WHERE `order` =  " + order + " AND scenario_id = 1";
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
