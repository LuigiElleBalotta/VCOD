﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

	public class DbManager
	{
	static float GAME_VERSION = 1.0f; //viene aggiornata ad ogni modifica sostanziale (per adesso possiamo aggiornarlo in base ai progressi nel db)
    static SqliteConnection myConnection = null;
	private DbManager ()
	{
        if (!System.IO.File.Exists("Assets/DB/virus.sqlite"))
            SqliteConnection.CreateFile("Assets/DB/virus.sqlite");
        myConnection = new SqliteConnection("URI=file:Assets/DB/virus.sqlite, version=3");

        /*while (!update()) //Ciclo di aggiornamenti fino alla versione corrente (return true se DB_VERSION = GAME_VERSION)
            ;*/
	}
		
	public static void setInstance(){
		if(myConnection==null)
			new DbManager();
	}
    public static void executeQuery(string sql)
    {
        myConnection.Open();
        SqliteCommand com = new SqliteCommand(sql, myConnection);
        com.ExecuteNonQuery();
        myConnection.Close();
    }
    public static JSONObject loadWorld()
    {
        myConnection.Open();
        string query = "SELECT idSpawn, position_x, position_y, position_z, orientation_x, orientation_y, orientation_z FROM nemici_info;";
        SqliteCommand com = new SqliteCommand(query, myConnection);
        SqliteDataReader reader = com.ExecuteReader();
        string[] result = new string[6];
        int contatore = 0;
        JSONObject j = new JSONObject();
        while (reader.Read())
        {
            j.AddField("idSpawn", reader["idSpawn"].ToString());
            j.AddField("position_x", reader["position_x"].ToString());
            j.AddField("position_y", reader["position_y"].ToString());
            j.AddField("position_z", reader["position_z"].ToString());
            j.AddField("orientation_x", reader["orientation_x"].ToString());
            j.AddField("orientation_y", reader["orientation_y"].ToString());
            j.AddField("orientation_z", reader["orientation_z"].ToString());
            //result[contatore] = reader["idSpawn"] + "|" + reader["position_x"] + "|" + reader["position_y"] + "|" + reader["position_z"] + "|" + reader["orientation_x"] + "|" + reader["orientation_y"] + "|" + reader["orientation_z"];
            contatore++;
        }
        myConnection.Close();
        
        return j;
        
    }
	public static string loadPlayer(string playerName){
        myConnection.Open();
        string sql = "SELECT id FROM player WHERE savetype = 1;";
        SqliteCommand com = new SqliteCommand(sql, myConnection);
        SqliteDataReader l = com.ExecuteReader();
        int c = 0;
        while (l.Read())
        {
            c++;
        }
        l.Close();
        string result = "";
        if (c > 0)
        {
            //il player torna dal menù
            sql = "SELECT level, exp, health, maxhealth, position_x, position_y, position_z, orientation_x, orientation_y, orientation_z FROM player WHERE name='" + playerName + "' AND savetype = 1;";
            SqliteCommand comm = new SqliteCommand(sql, myConnection);
            SqliteDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                result = reader["level"] + "|" + reader["exp"] + "|" + reader["health"] + "|" + reader["maxhealth"] + "|" + reader["position_x"] + "|" + reader["position_y"] + "|" + reader["position_z"] + "|" + reader["orientation_x"] + "|" + reader["orientation_y"] + "|" + reader["orientation_z"];
            }
            reader.Close();
            //cancello il salvataggio perchè non mi serve più.
            SqliteCommand cancella = new SqliteCommand("DELETE FROM player WHERE savetype = 1;", myConnection);
            cancella.ExecuteNonQuery();
        }
        else
        {
            sql = "SELECT level, exp, expToNextLvl, health, maxhealth, position_x, position_y, position_z, orientation_x, orientation_y, orientation_z FROM player WHERE name='" + playerName + "' AND savetype = 0;";
            SqliteCommand comm = new SqliteCommand(sql, myConnection);
            SqliteDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                result = reader["level"] + "|" + reader["exp"] + "|" + reader["expToNextLvl"] + "|" + reader["health"] + "|" + reader["maxhealth"] + "|" + reader["position_x"] + "|" + reader["position_y"] + "|" + reader["position_z"] + "|" + reader["orientation_x"] + "|" + reader["orientation_y"] + "|" + reader["orientation_z"];
            }
            reader.Close();
        }
		myConnection.Close();
		return result;
	}
	//La versione del game andrà di pari passo con quella del DB, quando ci sarà una versione di gioco diversa verrà di conseguenza aggiornato anche il db(anche se non ci sono
	//aggiornamenti verrà comunque aggiornata la versione nel DB) questo metodo ci consente di tenere aggiornato il DB di qualsiasi versione di gioco e senza metter mano
	//a client MySql o mettere bottoni

    //commentato per via di errore fastidioso
	/*bool update(){
		float currentGV = 0;
		float currentDB = 0;
		string sql = "SELECT * FROM version;";
		SqliteCommand com = new SqliteCommand(sql, myConnection); 
		SqliteDataReader reader = com.ExecuteReader ();
		//----- controlla se c'è già una riga nella tabella versione, se non c'è la inserisce con la versione di gioco corrente e db con solo le tabelle create
		// e ritorna false per continuare il ciclo di aggiornamento
		if (!reader.HasRows) {
			SqliteCommand Ins = new SqliteCommand ("INSERT INTO version (game_version, db_version) VALUES(" + GAME_VERSION + ", 1.0);", myConnection);
			Ins.ExecuteNonQuery();
			if(GAME_VERSION > 1.0f)
				return false;
			return true;
		}
		else{
			if(reader.Read()){
				currentGV = reader.GetFloat(2);
				currentDB = reader.GetFloat(3);
				if(currentGV < GAME_VERSION){    //----- Aggiorna nel db la versione di gioco con quella corrente
					SqliteCommand Ins = new SqliteCommand ("UPDATE version SET game_version = " + GAME_VERSION + ");" , myConnection);
					Ins.ExecuteNonQuery();
					currentGV = GAME_VERSION;
				}

			}
	    }
		if (currentGV > currentDB) {  //Qui all'interno ci andranno i diversi update per versione di gioco
			if(currentDB == 1.0f){
				//Qui vanno inseriti gli aggiornamenti poi con la query sotto viene aggiornata la versione del db
				//SqliteCommand upda = new SqliteCommand ("UPDATE version SET db_version = 1.1 WHERE id = 1;", myConnection);
				//upda.ExecuteNonQuery();
				//currentDB = 1.1f;
			}
			else if(currentDB == 1.1f){
				//Qui vanno inseriti gli aggiornamenti poi con la query sotto viene aggiornata la versione del db
				//SqliteCommand upda = new SqliteCommand ("UPDATE version SET db_version = 1.2 WHERE id = 1;", myConnection);
				//upda.ExecuteNonQuery();
				//currentDB = 1.2f;
			}
		
		}
		if (currentDB < currentGV) //se il db non è ancora all'ultima versione ritorna false per continuare il ciclo
			return false;
		
	    return true;

	}*/

}

