using System;
using System.Collections.Generic;

namespace PiecykM.CodeProcesor
{
    /// <summary>
    /// Klasa zbierająca w sobie wszystkie dostępne w programie grupy komend i udostępniająca je
    /// </summary>
    public static class CommandMaster
    {
        private static Dictionary<string, CommandGroup> _commandGroups = new Dictionary<string, CommandGroup>();

        static CommandMaster()
        {
            RegisterGroup(new CommandGroupFunc());
            RegisterGroup(new CommandGroupLumel());
            RegisterGroup(new CommandGroupMfia());
        }

        private static void RegisterGroup(CommandGroup group)
        {
            _commandGroups.Add(
                group.GroupName,
                group
                );
        }

        public static void ExecuteCommand(string group, string command, List<object> param) =>
            _commandGroups[group].ExecuteCommand(command, param);

        public static CommandGroup GetCommandGroup(string group) =>
            _commandGroups[group];

        public static IEnumerable<string> GetGroupNames() =>
            _commandGroups.Keys;

        public static bool ContainsGroup(string group) =>
            _commandGroups.ContainsKey(group);
    }
}
