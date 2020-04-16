using System;
using System.Collections.Generic;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public static class Permissions
    {
        private const string Read = "r";
        private const string Manage = "m";

        private const string ReadDescription = "Read";
        private const string ManageDescription = "Manage";

        private static readonly Dictionary<string, Tuple<string, Func<string, string>>> ServiceMap = new Dictionary<string, Tuple<string, Func<string, string>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new Tuple<string, Func<string, string>>("Identity", Identity.GetResourceName),
            ["a"] = new Tuple<string, Func<string, string>>("Analyzer Gateway", AnalyzerGateway.GetResourceName),
            ["i"] = new Tuple<string, Func<string, string>>("Inspector", Inspector.GetResourceName),
            ["r"] = new Tuple<string, Func<string, string>>("Reporter", Reporter.GetResourceName),
            ["rm"] = new Tuple<string, Func<string, string>>("Rule Manager", RuleManager.GetResourceName),
            ["w"] = new Tuple<string, Func<string, string>>("Webhooks", RuleManager.GetResourceName),
        };

        public static class Identity
        {
            private const string ServiceName = "id";

            public static readonly string ReadUsers = ServiceName + ".u." + Read;
            public static readonly string ManageUsers = ServiceName + ".u." + Manage;
            public static readonly string ReadRoles = ServiceName + ".r." + Read;
            public static readonly string ManageRoles = ServiceName + ".r." + Manage;
            public static readonly string ReadTenants = ServiceName + ".t." + Read;
            public static readonly string ManageTenants = ServiceName + ".t." + Manage;
            public static readonly string ReadOAuthClients = ServiceName + ".o." + Read;
            public static readonly string ManageOAuthClients = ServiceName + ".o." + Manage;

            private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["u"] = "User",
                ["r"] = "Roles",
                ["t"] = "Tenants",
                ["o"] = "OAuth Settings",
            };

            public static IEnumerable<string> GetAllPermissions()
            {
                yield return ReadUsers;
                yield return ManageUsers;
                yield return ReadRoles;
                yield return ManageRoles;
                yield return ReadTenants;
                yield return ManageTenants;
                yield return ReadOAuthClients;
                yield return ManageOAuthClients;
            }

            internal static string GetResourceName(string abbreviation)
            {
                if (string.IsNullOrWhiteSpace(abbreviation))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(abbreviation));

                return ResourceNames.TryGetValue(abbreviation, out var value) ? value : string.Empty;
            }
        }

        public static class AnalyzerGateway
        {
            private const string ServiceName = "a";

            public static readonly string ReadJobs = ServiceName + ".j." + Read;
            public static readonly string ManageJobs = ServiceName + ".j." + Manage;

            private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["j"] = "Jobs",
            };

            internal static string GetResourceName(string abbreviation)
            {
                if (string.IsNullOrWhiteSpace(abbreviation))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(abbreviation));

                return ResourceNames.TryGetValue(abbreviation, out var value) ? value : string.Empty;
            }
        }

        public static class Inspector
        {
            private const string ServiceName = "i";

            public static readonly string ReadProjects = ServiceName + ".p." + Read;
            public static readonly string ManageProjects = ServiceName + ".p." + Manage;
            public static readonly string ReadProjectInspections = ServiceName + ".p.i." + Read;
            public static readonly string ManageProjectInspections = ServiceName + ".p.i." + Manage;
            public static readonly string ReadTools = ServiceName + ".t." + Read;
            public static readonly string ManageTools = ServiceName + ".t." + Manage;

            private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["p"] = "Projects",
                ["p.i"] = "Project Inspections",
                ["t"] = "Tools",
            };

            internal static string GetResourceName(string abbreviation)
            {
                if (string.IsNullOrWhiteSpace(abbreviation))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(abbreviation));

                return ResourceNames.TryGetValue(abbreviation, out var value) ? value : string.Empty;
            }
        }

        public static class Reporter
        {
            private const string ServiceName = "r";

            public static readonly string ReadDataSets = ServiceName + ".d." + Read;
            public static readonly string ManageDataSets = ServiceName + ".d." + Manage;
            public static readonly string ReadReports = ServiceName + ".r." + Read;
            public static readonly string ManageReports = ServiceName + ".r." + Manage;
            public static readonly string ReadReportJobs = ServiceName + ".r.j." + Read;
            public static readonly string ManageReportJobs = ServiceName + ".r.j." + Manage;

            private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["d"] = "DataSets",
                ["r"] = "Reports",
                ["r.j"] = "Report Jobs",
            };

            internal static string GetResourceName(string abbreviation)
            {
                if (string.IsNullOrWhiteSpace(abbreviation))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(abbreviation));

                return ResourceNames.TryGetValue(abbreviation, out var value) ? value : string.Empty;
            }
        }

        public static class RuleManager
        {
            private const string ServiceName = "rm";

            public static readonly string ReadPackages = ServiceName + ".p." + Read;
            public static readonly string ManagePackages = ServiceName + ".p." + Manage;
            public static readonly string ReadJobs = ServiceName + ".j." + Read;
            public static readonly string ManageJobs = ServiceName + ".j." + Manage;

            private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["p"] = "Packages",
                ["j"] = "Jobs",
            };

            internal static string GetResourceName(string abbreviation)
            {
                if (string.IsNullOrWhiteSpace(abbreviation))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(abbreviation));

                return ResourceNames.TryGetValue(abbreviation, out var value) ? value : string.Empty;
            }
        }

        public static class Webhooks
        {
            private const string ServiceName = "w";

            public static readonly string ReadEvents = ServiceName + ".e." + Read;
            public static readonly string CreateEvents = ServiceName + ".e." + Manage;
            public static readonly string ReadWebhooks = ServiceName + ".w." + Read;
            public static readonly string ManageWebhooks = ServiceName + ".w." + Manage;

            private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["e"] = "Events",
                ["w"] = "Webhooks",
            };

            internal static string GetResourceName(string abbreviation)
            {
                if (string.IsNullOrWhiteSpace(abbreviation))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(abbreviation));

                return ResourceNames.TryGetValue(abbreviation, out var value) ? value : string.Empty;
            }
        }

        public static string GetDescription(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(permission));

            var resourceIndex = permission.IndexOf('.');
            if (resourceIndex == -1)
                return string.Empty;

            var serviceAbbreviation = permission.Substring(0, resourceIndex);
            if (!ServiceMap.ContainsKey(serviceAbbreviation))
                return string.Empty;

            var serviceInfo = ServiceMap[serviceAbbreviation];

            var action = permission.EndsWith(".r", StringComparison.OrdinalIgnoreCase)
                ? ReadDescription
                : permission.EndsWith(".m", StringComparison.OrdinalIgnoreCase) ? ManageDescription : string.Empty;

            if (string.IsNullOrWhiteSpace(action))
                return string.Empty;

            var resourceName = serviceInfo.Item2(permission.Substring(resourceIndex + 1, permission.Length - resourceIndex - 3));

            return serviceInfo.Item1 + " " + action + " " + resourceName;
        }
    }
}
