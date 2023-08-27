using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace NectarRCON.Services
{
    public class GroupService : IGroupService
    {
        private static string _groupPath = Path.Combine("./groups");
        private readonly IMessageBoxService _messageBoxService;
        private readonly ILanguageService _languageService;

        private readonly Dictionary<string, Group> _groupMapping = new(); // id, group
        private readonly Dictionary<string, Group> _groupNameMapping = new(); // name, group
        public GroupService(IMessageBoxService messageBoxService, ILanguageService languageService)
        {
            _messageBoxService = messageBoxService;
            Refresh();
            _languageService = languageService;
        }

        private void Refresh()
        {
            _groupMapping.Clear();
            _groupNameMapping.Clear();
            if (Directory.Exists(_groupPath))
            {
                string[] jsonFiles = Directory.GetDirectories(_groupPath, "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    try
                    {
                        using (FileStream fs = File.OpenRead(jsonFile))
                        {
                            Group group = JsonSerializer.Deserialize<Group>(fs) ?? throw new InvalidDataException(jsonFile);
                            if (group.Id.ToLower() != Path.GetFileNameWithoutExtension(jsonFile).ToLower())
                                throw new InvalidDataException(string.Format(_languageService.GetKey("groups.exception.file_name_mismatch_exception"), jsonFile, group.Id));
                            if (_groupMapping.ContainsKey(group.Id))
                                throw new InvalidDataException(string.Format(_languageService.GetKey("groups.exception.same_group_id_exception"), _groupMapping[group.Id].Name, group.Name));
                            if (_groupNameMapping.ContainsKey(group.Name))
                                throw new InvalidDataException(string.Format(_languageService.GetKey("groups.exception.same_name_exception"), _groupMapping[group.Id].Id, group.Id));

                            _groupMapping.Add(group.Id, group);
                            _groupNameMapping.Add(group.Name, group);
                        }
                    }
                    catch (Exception ex)
                    {
                        _messageBoxService.Show(ex.Message, string.Format(_languageService.GetKey("groups.exception.invalid_data_exception"), ex.ToString()));
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(_groupPath);
            }
        }

        private void RemoveGroupInDict(string groupId)
        {
            if (_groupMapping.TryGetValue(groupId, out Group? group))
            {
                _groupMapping.Remove(groupId);
                if (_groupNameMapping.ContainsKey(group.Name))
                {
                    _groupNameMapping.Remove(group.Name);
                }
            }
        }

        public string Add(Group group)
        {
            string filePath = Path.Combine(_groupPath, group.Id + ".json");
            if (!File.Exists(filePath))
            {
                if (_groupNameMapping.ContainsKey(group.Name))
                    throw new InvalidOperationException(_languageService.GetKey("groups.exception.name_already_exists_exception"));
                if (_groupMapping.ContainsKey(group.Id))
                    throw new InvalidOperationException(_languageService.GetKey("groups.exception.group_id_exists_exception"));
                File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(group)));
                return group.Id;
            }
            throw new InvalidOperationException(_languageService.GetKey("groups.exception.group_id_exists_exception"));
        }

        public void Delete(string groupId)
        {
            string filePath = Path.Combine(_groupPath, groupId + ".json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                RemoveGroupInDict(groupId);
            }
        }

        public Group? FindGroup(string name)
        {
            _groupNameMapping.TryGetValue(name, out Group? group);
            return group;
        }

        public Group? GetGroup(string groupId)
        {
            _groupMapping.TryGetValue(groupId, out Group? group);
            return group;
        }

        public IReadOnlyList<Group> GetGroups()
            => _groupMapping.Select(s => s.Value).ToList();
    }
}
