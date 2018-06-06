using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;

namespace Ku.Core.Extensions.DbMigration.AssemblyLocator
{
    public class MyAssemblyLocator : IAssemblyLocator
    {
        private static readonly string AssemblyRoot = typeof(MyAssemblyLocator).GetTypeInfo().Assembly.GetName().Name;
        private readonly Assembly _entryAssembly;
        private readonly DependencyContext _dependencyContext;
        private readonly DbMigrationOptions _option;

        public MyAssemblyLocator(IOptions<DbMigrationOptions> option)
        {
            _option = option.Value;
            _entryAssembly = Assembly.GetEntryAssembly();
            _dependencyContext = DependencyContext.Load(_entryAssembly);
        }

        public virtual IList<Assembly> GetAssemblies()
        {
            if (_dependencyContext == null)
            {
                // Use the entry assembly as the sole candidate.
                return new[] { _entryAssembly };
            }

            return _dependencyContext
                .RuntimeLibraries
                .Where(IsCandidateLibrary)
                .SelectMany(l => l.GetDefaultAssemblyNames(_dependencyContext))
                .Select(assembly => Assembly.Load(new AssemblyName(assembly.Name)))
                .ToArray();
        }

        private bool IsCandidateLibrary(RuntimeLibrary library)
        {
            if (_option != null && _option.PocoAssemblys.Any())
            {
                return _option.PocoAssemblys.Contains(library.Name);
            }
            else
            {
                return library.Dependencies.Any(dependency => string.Equals(AssemblyRoot, dependency.Name, StringComparison.Ordinal));
            }
        }
    }
}
