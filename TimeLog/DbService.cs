// Copyright (C) 2022  Igor Krushch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Email: dev@krushch.com

using System;
using System.Data.SQLite;
using System.IO;
using TimeLog.Utils;

namespace TimeLog
{
    class DbService
    {
        private SQLiteConnection _connection;
        private bool _initialized = false;

        public DbService()
        {
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            string pathToDb = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IK", "TimeLogger");
            try
            {
                if (!Directory.Exists(pathToDb))
                    Directory.CreateDirectory(pathToDb);
            }
            catch (Exception ex)
            {
                Logger.Get().Log('E', string.Format("Failed to prepare the folder for the DB. Error:{0}", ex.ToString()));
                System.Windows.Forms.Application.Exit();
                return;
            }

            pathToDb = Path.Combine(pathToDb, "timelog.db");

            _initialized = true;

            try
            {
                _connection = new SQLiteConnection(string.Format("URI=file:{0}", pathToDb));
                _connection.Open();
            }
            catch (Exception ex)
            {
                Logger.Get().Log('E', string.Format("Failed to open the DB. Error:{0}", ex.ToString()));
                System.Windows.Forms.Application.Exit();
                return;
            }

            runMigrations();
        }

        private void runMigrations()
        {
            var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='migrations'";
            if (cmd.ExecuteScalar() == null)
            {
                initDatabase();

                runMigrationsFrom(0);
            }
            else
            {
                cmd.CommandText = "SELECT current FROM migrations";
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    var lastMigrationId = (Int32)result;
                    if (lastMigrationId < 2)
                    {
                        runMigrationsFrom(lastMigrationId);
                    }
                }
                // TODO
            }
        }


        private void runMigrationsFrom(int lastMigrationId)
        {
            Logger.Get().Log('I', string.Format("DB migrations since {0} ran.", lastMigrationId));
        }

        private void initDatabase()
        {
            var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = @"CREATE TABLE config(param_name varchar(64) PRIMARY KEY, param_value varchar(512));";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE log_entries(id int64 PRIMARY KEY, ticket_id varchar(32), comment text, time_reported varchar(64), duration varchar(64), worklog_id varchar(64), is_flushed smallint, skip_flushing smallint);";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE work_log(id integer PRIMARY KEY, time_reported varchar(64));";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE notifications(time_reported varchar(64));";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE migrations(current int);";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO migrations(current) VALUES (@current)";
            cmd.Parameters.AddWithValue("@current", 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            Logger.Get().Log('I', "DB initialized");
        }

        public SQLiteCommand createCommand()
        {
            return new SQLiteCommand(_connection);
        }
    }
}
