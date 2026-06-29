using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Launcher.ViewModels
{
    // DropFilter: filter items by ID/name AND/OR rarity, then accept/reject them.
    // Both item names/IDs and rarities are optional and combined with AND logic:
    // - Items only: filter matching names/IDs
    // - Rarities only: filter matching rarities
    // - Both: filter items that match BOTH name/ID AND rarity
    public partial class MainWindowViewModel
    {
        // Items input: comma-separated for multi-word names, or space/newline-separated for single words
        // Examples: "Helm,Armor,Back" or "Thief Armor Gem,Back" or "1 2 3"
        [ObservableProperty]
        private string _itemsFilterInput = "";

        // Rarities: checkboxes for common/uncommon/rare/epic (optional)
        [ObservableProperty]
        private bool _filterRarityCommon;

        [ObservableProperty]
        private bool _filterRarityUncommon;

        [ObservableProperty]
        private bool _filterRarityRare;

        [ObservableProperty]
        private bool _filterRarityEpic;

        // Accept/Reject action: 0 = accept (whitelist), 1 = reject (blacklist)
        [ObservableProperty]
        private bool _filterActionAccept = true;

        [ObservableProperty]
        private bool _filterActionReject = false;

        // Status feedback
        [ObservableProperty]
        private string _dropFilterStatus = "Ready";

        /// <summary>
        /// Parse items input supporting multiple formats:
        /// - Comma-separated names/IDs: "Helm,Armor,Back" or "1,2,3"
        /// - Multi-word names: "Thief Armor Gem,Lucky Cape Gem" (use commas to separate multi-word items)
        /// - Single-word items: space or newline separated
        /// </summary>
        private List<string> ParseItemsInput(string input)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(input)) return result;

            // Primary separator: comma (preserves multi-word names)
            var commaParts = input.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in commaParts)
            {
                var trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    result.Add(trimmed);
                }
            }

            return result;
        }

        [RelayCommand]
        private void ApplyDropFilter()
        {
            // Gather both items and rarities simultaneously (AND logic)
            var items = new List<string>();
            var itemIds = new List<int>();
            var rarities = new List<string>();

            // Parse items/IDs if provided
            if (!string.IsNullOrWhiteSpace(ItemsFilterInput))
            {
                var parts = ParseItemsInput(ItemsFilterInput);
                foreach (var part in parts)
                {
                    if (int.TryParse(part, out int id))
                    {
                        itemIds.Add(id);
                    }
                    else if (!string.IsNullOrEmpty(part))
                    {
                        items.Add(part);
                    }
                }
            }

            // Gather selected rarities
            if (FilterRarityCommon) rarities.Add("common");
            if (FilterRarityUncommon) rarities.Add("uncommon");
            if (FilterRarityRare) rarities.Add("rare");
            if (FilterRarityEpic) rarities.Add("epic");

            // Build single payload with both filters
            var payload = new JObject
            {
                ["Items"] = JArray.FromObject(items),
                ["ItemIds"] = JArray.FromObject(itemIds),
                ["Rarities"] = JArray.FromObject(rarities),
                ["Action"] = FilterActionAccept ? "Accept" : "Reject",
            };

            _connection.SendCommand("ApplyDropFilter", payload);

            // Describe what was sent
            var desc_parts = new List<string>();
            if (items.Count > 0 || itemIds.Count > 0)
            {
                desc_parts.Add($"items ({items.Count} names + {itemIds.Count} IDs)");
            }
            if (rarities.Count > 0)
            {
                desc_parts.Add($"rarities: {string.Join(", ", rarities)}");
            }

            string desc = desc_parts.Count > 0 ? string.Join(" AND ", desc_parts) : "(no filter)";
            DropFilterStatus = $"Sent: {(FilterActionAccept ? "Accept" : "Reject")} {desc}";
        }

        [RelayCommand]
        private void ClearDropFilter()
        {
            _connection.SendCommand("ClearDropFilter", null);
            DropFilterStatus = "Filter cleared";
        }
    }
}
