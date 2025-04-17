using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Need;
public class Main : IPlugin
{
    private PluginInitContext _context;
    private readonly string COMMAND_OPEN = "open";

    private readonly string[] COMMANDS_DELETE = new string[2] { "delete", "remove" };

    private readonly string[] COMMANDS_RELOAD = new string[2] { "reload", "refresh" };

    private readonly int MAX_NB_CHAR_BEFORE_ELLIPSIS = 64;

    private readonly int NB_MATCH_BEFORE_ADDING_VALUE_AS_RESULT = 1;

    private Database _db;

    private PluginInitContext _pluginContext;

    public void Init(PluginInitContext context)
    {
        _context = context;
        _pluginContext = context;
        _db = new Database(_pluginContext.CurrentPluginMetadata.PluginDirectory + "\\");
        _db.Load();
    }

    public List<Result> Query(Query query)
    {
        string[] splittedQuery = query.Search.Split(new char[1] { ' ' }, 2);
        string command = splittedQuery[0];
        if (splittedQuery.Length == 1)
        {
            return QueryResultsGetFromKey(command.ToLower());
        }
        string keyToSearch = splittedQuery[1];
        if (COMMANDS_DELETE.Contains(command.ToLower()))
        {
            return QueryResultsDeleteFromKey(keyToSearch);
        }
        return QueryResultsSaveFromKeyAndValue(command, keyToSearch);
    }
    private List<Result> QueryResultsSaveFromKeyAndValue(string key, string value)
    {
        return new List<Result>
        {
            new Result
            {
                Title = "Save " + key,
                SubTitle = "Save " + key + " to " + StringHelper.TruncateWithEllipsis(value, MAX_NB_CHAR_BEFORE_ELLIPSIS),
                Action = delegate
                {
                    _db.Add(key, value);
                    return true;
                }
            }
        };
    }

    private List<KeyValuePair<string, string>> GetMatchingItems(string keyToSearch)
    {
        List<KeyValuePair<string, string>> matchingItems = _db.GetMatchingItemsFromKey(keyToSearch);
        if (matchingItems.Count <= NB_MATCH_BEFORE_ADDING_VALUE_AS_RESULT)
        {
            matchingItems.AddRange(_db.GetMatchingItemsFromValue(keyToSearch));
        }
        return matchingItems;
    }

    private List<Result> QueryResultsGetFromKey(string keyToSearch)
    {
        List<Result> queryResults = new List<Result>();
        GetMatchingItems(keyToSearch).ForEach(delegate (KeyValuePair<string, string> item)
        {
            Result item2 = new Result
            {
                Title = item.Key,
                SubTitle = StringHelper.TruncateWithEllipsis(item.Value, MAX_NB_CHAR_BEFORE_ELLIPSIS),
                Action = delegate
                {
                    // Clipboard.SetText(item.Value);
                    TextCopy.ClipboardService.SetText(item.Value);
                    return true;
                }
            };
            queryResults.Add(item2);
        });
        TryAddingOpenCommand(queryResults, keyToSearch);
        TryAddingReloadCommand(queryResults, keyToSearch);
        TryAddingDeleteSuggestionCommand(queryResults, keyToSearch);
        return queryResults;
    }

    private List<Result> QueryResultsDeleteFromKey(string splittedQuery)
    {
        List<Result> queryResults = new List<Result>();
        GetMatchingItems(splittedQuery).ForEach(delegate (KeyValuePair<string, string> item)
        {
            Result item2 = new Result
            {
                Title = "Delete " + item.Key,
                SubTitle = "Delete " + item.Key + " containing " + StringHelper.TruncateWithEllipsis(item.Value, MAX_NB_CHAR_BEFORE_ELLIPSIS),
                Action = delegate
                {
                    _db.Remove(item.Key);
                    return true;
                }
            };
            queryResults.Add(item2);
        });
        return queryResults;
    }

    private void TryAddingReloadCommand(List<Result> queryResults, string keyToSearch)
    {
        if (!(keyToSearch == "") && COMMANDS_RELOAD.Any((string cmdReload) => cmdReload.Contains(keyToSearch)))
        {
            queryResults.Insert(0, new Result
            {
                Title = "Reload database",
                SubTitle = "When it has been modified manually",
                Action = delegate
                {
                    _db.Load();
                    return true;
                }
            });
        }
    }

    private void TryAddingDeleteSuggestionCommand(List<Result> queryResults, string keyToSearch)
    {
        if (!(keyToSearch == "") && COMMANDS_DELETE.Any((string cmdDelete) => cmdDelete.Contains(keyToSearch)))
        {
            queryResults.Insert(0, new Result
            {
                Title = "Delete",
                SubTitle = "Delete an item from the database",
                Action = (ActionContext ctx) => false
            });
        }
    }

    private void TryAddingOpenCommand(List<Result> queryResults, string keyToSearch)
    {
        if (!(keyToSearch == "") && COMMAND_OPEN.Contains(keyToSearch))
        {
            queryResults.Insert(0, new Result
            {
                Title = "Open key/value database file",
                SubTitle = "Modify, see and delete entries manually",
                Action = delegate
                {
                    // Process.Start(_db.GetFullPath());
                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = _db.GetFullPath(),
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    return true;
                }
            });
        }
    }
}

