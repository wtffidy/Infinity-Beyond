using System.Collections.Generic;
using System.Linq;

namespace BeyondAgent.Util
{
    /// <summary>
    /// Simple drop filter that can whitelist (accept only) or blacklist (reject) items.
    /// Filters by name, ID, and/or rarity. Applied per-drop via ApplyDropFilter().
    /// </summary>
    public static class DropFilterEngine
    {
        private static List<string> _filterItemNames = new();
        private static List<int> _filterItemIds = new();
        private static List<string> _filterRarities = new();
        private static bool _acceptOnly = true; // true = accept (whitelist), false = reject (blacklist)

        /// <summary>
        /// Apply a new filter configuration. Clears previous filter.
        /// If no items/rarities specified, disables filtering entirely.
        /// </summary>
        public static void ApplyDropFilter(List<string> itemNames, List<int> itemIds, List<string> rarities, string action)
        {
            if ((itemNames?.Count > 0 || itemIds?.Count > 0 || rarities?.Count > 0))
            {
                _filterItemNames = itemNames ?? new();
                _filterItemIds = itemIds ?? new();
                _filterRarities = rarities?.Select(r => r.ToLower()).ToList() ?? new();
                _acceptOnly = action?.ToLower() == "accept";
            }
            else
            {
                DisableFilter();
            }

            var parts = new List<string>();
            if (_filterItemNames.Count > 0) parts.Add($"{_filterItemNames.Count} items");
            if (_filterItemIds.Count > 0) parts.Add($"{_filterItemIds.Count} IDs");
            if (_filterRarities.Count > 0) parts.Add($"rarities: {string.Join(", ", _filterRarities)}");

            string status;
            int total = _filterItemNames.Count + _filterItemIds.Count + _filterRarities.Count;
            if (total > 0)
            {
                string acpt = _acceptOnly ? "ACCEPT" : "REJECT";
                status = $"Apply: {acpt}";
            }
            else
            {
                status = "Disable filter";
            }
        }

        /// <summary>
        /// Check if an item drop should be allowed based on current filter.
        /// Returns true if DROP IS ALLOWED, false if filtered out.
        /// </summary>
        public static bool ShouldAllowDrop(string itemName, int itemId, string itemRarity)
        {
            if (!HasActiveFilter()) return true; // No filter = allow all

            bool matchesName = _filterItemNames.Count == 0 || _filterItemNames.Contains(itemName);
            bool matchesId = _filterItemIds.Count == 0 || _filterItemIds.Contains(itemId);
            bool matchesRarity = _filterRarities.Count == 0 || _filterRarities.Contains(itemRarity?.ToLower());

            bool isMatch = matchesName && matchesId && matchesRarity;

            return _acceptOnly ? isMatch : !isMatch;
        }

        /// <summary>
        /// Clear any active filter.
        /// </summary>
        public static void ClearFilter()
        {
            DisableFilter();
        }

        private static void DisableFilter()
        {
            _filterItemNames.Clear();
            _filterItemIds.Clear();
            _filterRarities.Clear();
            _acceptOnly = true;
        }

        private static bool HasActiveFilter() => _filterItemNames.Count > 0 || _filterItemIds.Count > 0 || _filterRarities.Count > 0;
    }
}
