using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Flow.Launcher.Plugin.Need
{
    internal class Database
    {
        private const char ITEM_KEY_VALUE_SEPARATOR = '=';

        private Dictionary<string, string> _keyToValue = new Dictionary<string, string>();

        private string _dbFilePath;

        private string _dbFileName = "db.txt";

        public Database(string filepath = "./")
        {
            _dbFilePath = filepath + _dbFileName;
        }

        public void Load()
        {
            try
            {
                _keyToValue = (from line in File.ReadLines(_dbFilePath)
                               where line.Length > 3 && line.Contains('=')
                               select line).Select(delegate (string line)
                           {
                               string[] array = line.Split(new char[1] { '=' }, 2);
                               return new
                               {
                                   Name = array[0],
                                   Value = array[1]
                               };
                           }).ToDictionary(item => item.Name, item => item.Value);
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to parse file " + _dbFilePath + ": " + e.Message);
            }
        }

        public void Save()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in _keyToValue)
            {
                stringBuilder.Append(item.Key).Append('=').Append(item.Value)
                    .Append(Environment.NewLine);
            }
            File.WriteAllText(_dbFilePath, stringBuilder.ToString());
        }

        public List<KeyValuePair<string, string>> GetMatchingItemsFromKey(string keyToSearch)
        {
            return _keyToValue.Where((KeyValuePair<string, string> item) => item.Key.ToLower().Contains(keyToSearch)).ToList();
        }

        public List<KeyValuePair<string, string>> GetMatchingItemsFromValue(string valueToSearch)
        {
            return _keyToValue.Where((KeyValuePair<string, string> item) => item.Value.ToLower().Contains(valueToSearch)).ToList();
        }

        public void Add(string key, string value)
        {
            _keyToValue[key] = value;
            Save();
        }

        public void Remove(string key)
        {
            _keyToValue.Remove(key);
            Save();
        }

        public string GetFullPath()
        {
            return _dbFilePath;
        }
    }
}