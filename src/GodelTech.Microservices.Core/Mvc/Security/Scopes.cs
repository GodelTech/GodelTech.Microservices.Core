using System;
using System.Collections.Generic;
using System.Linq;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public static class Scopes
    {
        public static readonly string ReadIntent = "r";
        public static readonly string ManageIntent = "m";

        public static string GetPermissionByScope(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(scope));

            return AnalyzerGateway.GetPermissionByScope(scope)
                   ?? Inspector.GetPermissionByScope(scope)
                   ?? Reporter.GetPermissionByScope(scope)
                   ?? RuleManager.GetPermissionByScope(scope)
                   ?? Webhooks.GetPermissionByScope(scope);
        }

        public static IEnumerable<string> GetScopes()
        {
            return AnalyzerGateway.GetScopes()
                .Concat(Inspector.GetScopes())
                .Concat(Reporter.GetScopes())
                .Concat(RuleManager.GetScopes())
                .Concat(Webhooks.GetScopes());
        }

        public static class AnalyzerGateway
        {
            private const string ServiceName = "a";

            public static readonly string ReadJobs = ServiceName + ".jobs." + ReadIntent;
            public static readonly string ManageJobs = ServiceName + ".jobs." + ManageIntent;

            private static readonly Dictionary<string, string> ScopeToPermissionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [ManageJobs] = Permissions.AnalyzerGateway.ManageJobs,
                [ReadJobs] = Permissions.AnalyzerGateway.ReadJobs
            };

            public static IEnumerable<string> GetScopes() => ScopeToPermissionMap.Keys;

            public static string GetPermissionByScope(string scope)
            {
                if (string.IsNullOrWhiteSpace(scope))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(scope));

                return ScopeToPermissionMap.TryGetValue(scope, out var permission) ? permission : null;
            }
        }

        public static class Inspector
        {
            private const string ServiceName = "i";

            public static readonly string ReadProjects = ServiceName + ".projects." + ReadIntent;
            public static readonly string ManageProjects = ServiceName + ".projects." + ManageIntent;
            public static readonly string ReadProjectInspections = ServiceName + ".projects.inspections." + ReadIntent;
            public static readonly string ManageProjectInspections = ServiceName + ".projects.inspections." + ManageIntent;
            public static readonly string ReadTools = ServiceName + ".tools." + ReadIntent;
            public static readonly string ManageTools = ServiceName + ".tools." + ManageIntent;

            private static readonly Dictionary<string, string> ScopeToPermissionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [ReadProjects] = Permissions.Inspector.ReadProjects,
                [ManageProjects] = Permissions.Inspector.ManageProjects,
                [ReadProjectInspections] = Permissions.Inspector.ReadProjectInspections,
                [ManageProjectInspections] = Permissions.Inspector.ManageProjectInspections,
                [ReadTools] = Permissions.Inspector.ReadTools,
                [ManageTools] = Permissions.Inspector.ManageTools
            };

            public static IEnumerable<string> GetScopes() => ScopeToPermissionMap.Keys;

            public static string GetPermissionByScope(string scope)
            {
                if (string.IsNullOrWhiteSpace(scope))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(scope));

                return ScopeToPermissionMap.TryGetValue(scope, out var permission) ? permission : null;
            }
        }

        public static class Reporter
        {
            private const string ServiceName = "r";

            public static readonly string ReadDataSets = ServiceName + ".datasets." + ReadIntent;
            public static readonly string ManageDataSets = ServiceName + ".datasets." + ManageIntent;
            public static readonly string ReadReports = ServiceName + ".reports." + ReadIntent;
            public static readonly string ManageReports = ServiceName + ".reports." + ManageIntent;
            public static readonly string ReadReportJobs = ServiceName + ".reports.jobs." + ReadIntent;
            public static readonly string ManageReportJobs = ServiceName + ".reports.jobs." + ManageIntent;

            private static readonly Dictionary<string, string> ScopeToPermissionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [ReadDataSets] = Permissions.Reporter.ReadDataSets,
                [ManageDataSets] = Permissions.Reporter.ManageDataSets,
                [ReadReports] = Permissions.Reporter.ReadReports,
                [ManageReports] = Permissions.Reporter.ManageReports,
                [ReadReportJobs] = Permissions.Reporter.ReadReportJobs,
                [ManageReportJobs] = Permissions.Reporter.ManageReportJobs,
            };

            public static IEnumerable<string> GetScopes() => ScopeToPermissionMap.Keys;

            public static string GetPermissionByScope(string scope)
            {
                if (string.IsNullOrWhiteSpace(scope))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(scope));

                return ScopeToPermissionMap.TryGetValue(scope, out var permission) ? permission : null;
            }
        }

        public static class RuleManager
        {
            private const string ServiceName = "rm";

            public static readonly string ReadPackages = ServiceName + ".packages." + ReadIntent;
            public static readonly string ManagePackages = ServiceName + ".packages." + ManageIntent;
            public static readonly string ReadJobs = ServiceName + ".jobs." + ReadIntent;
            public static readonly string ManageJobs = ServiceName + ".jobs." + ManageIntent;

            private static readonly Dictionary<string, string> ScopeToPermissionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [ReadPackages] = Permissions.RuleManager.ReadPackages,
                [ManagePackages] = Permissions.RuleManager.ManagePackages,
                [ReadJobs] = Permissions.RuleManager.ReadJobs,
                [ManageJobs] = Permissions.RuleManager.ManageJobs,
            };

            public static IEnumerable<string> GetScopes() => ScopeToPermissionMap.Keys;

            public static string GetPermissionByScope(string scope)
            {
                if (string.IsNullOrWhiteSpace(scope))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(scope));

                return ScopeToPermissionMap.TryGetValue(scope, out var permission) ? permission : null;
            }
        }

        public static class Webhooks
        {
            private const string ServiceName = "w";

            public static readonly string ReadEvents = ServiceName + ".events." + ReadIntent;
            public static readonly string CreateEvents = ServiceName + ".events." + ManageIntent;
            public static readonly string ReadWebhooks = ServiceName + ".webhooks." + ReadIntent;
            public static readonly string ManageWebhooks = ServiceName + ".webhooks." + ManageIntent;

            private static readonly Dictionary<string, string> ScopeToPermissionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [ReadEvents] = Permissions.Webhooks.ReadEvents,
                [CreateEvents] = Permissions.Webhooks.CreateEvents,
                [ReadWebhooks] = Permissions.Webhooks.ReadWebhooks,
                [ManageWebhooks] = Permissions.Webhooks.ManageWebhooks,
            };

            public static IEnumerable<string> GetScopes() => ScopeToPermissionMap.Keys;

            public static string GetPermissionByScope(string scope)
            {
                if (string.IsNullOrWhiteSpace(scope))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(scope));

                return ScopeToPermissionMap.TryGetValue(scope, out var permission) ? permission : null;
            }
        }
    }
}
