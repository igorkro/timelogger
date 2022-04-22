using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TimeLog.Utils;

namespace TimeLog.Models
{
    class ConfigRepository
    {
        public ConfigRepository()
        {
        }

        public void Load(Config config)
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"SELECT param_name, param_value FROM config";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string fieldName = reader.GetString(0).ToLower();
                    string fieldValue = reader.IsDBNull(1) ? null : reader.GetString(1);

                    switch (fieldName)
                    {
                        case "last_log_entry_id":
                            config.LastLogEntryId = fieldValue;
                            break;

                        case "last_flushed_log_entry_id":
                            config.LastFlushedLogEntryId = fieldValue;
                            break;

                        case "command_shortcut":
                            config.CommandShortcut = fieldValue;
                            break;

                        case "misc_ticket":
                            config.MiscTicket = fieldValue;
                            break;

                        case "jira_url":
                            config.JiraURL = fieldValue;
                            break;

                        case "is_access_token":
                            config.IsAccessTokenAuth = fieldValue == "1";
                            break;

                        case "show_total_logged":
                            config.ShowTotalLogged = fieldValue == "1";
                            break;
                    }
                }
            }

            Logger.Get().Log('I', "Configuration loaded.");
        }

        public void Save(Config config)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>
            {
                { "last_log_entry_id", config.LastLogEntryId },
                { "last_flushed_log_entry_id", config.LastFlushedLogEntryId },
                { "command_shortcut", config.CommandShortcut },
                { "misc_ticket", config.MiscTicket },
                { "jira_url", config.JiraURL },
                { "is_access_token", config.IsAccessTokenAuth ? "1" : "0" },
                { "show_total_logged", config.ShowTotalLogged ? "1" : "0" },
            };

            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"insert or replace into config(param_name, param_value) values (
                @param_name, @param_value
            )";
            foreach (var kv in fields) {
                command.Parameters.AddWithValue("@param_name", kv.Key);
                command.Parameters.AddWithValue("@param_value", kv.Value);
                command.Prepare();

                command.ExecuteNonQuery();

                command.Reset();
            }

            Logger.Get().Log('I', "Configuration saved.");
        }

    }
}
