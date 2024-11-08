using System.Reflection;

namespace Assembly.Extensions;

public static class AssemblyExtensions
{
    public static System.Reflection.Assembly[] LoadAssemblyHierarchyFrom<T>(string prefix)
    {
        var assembly = typeof(T).Assembly;
        var loaded = new HashSet<string>();
        var container = new HashSet<System.Reflection.Assembly> { assembly };
        var prefixPredicate = CreateAssemblyPrefixPredicate(prefix);
        return LoadAssemblies(assembly, loaded, container, prefixPredicate);
    }

    public static System.Reflection.Assembly[] LoadAssemblyHierarchyFrom(this Type type, string prefix)
    {
        var assembly = type.Assembly;
        var loaded = new HashSet<string>();
        var container = new HashSet<System.Reflection.Assembly> { assembly };
        var prefixPredicate = CreateAssemblyPrefixPredicate(prefix);
        return LoadAssemblies(assembly, loaded, container, prefixPredicate);
    }

    public static System.Reflection.Assembly[] LoadAssemblyHierarchyFrom(
        this System.Reflection.Assembly assembly,
        string prefix
    )
    {
        var loaded = new HashSet<string>();
        var container = new HashSet<System.Reflection.Assembly> { assembly };
        var prefixPredicate = CreateAssemblyPrefixPredicate(prefix);
        return LoadAssemblies(assembly, loaded, container, prefixPredicate);
    }

    public static System.Reflection.Assembly[] LoadAssemblyHierarchyFrom(
        this System.Reflection.Assembly assembly,
        Func<AssemblyName, bool> predicate
    )
    {
        var loaded = new HashSet<string>();
        var container = new HashSet<System.Reflection.Assembly> { assembly };
        return LoadAssemblies(assembly, loaded, container, predicate);
    }

    private static System.Reflection.Assembly[] LoadAssemblies(
        System.Reflection.Assembly current,
        ISet<string> loaded,
        ICollection<System.Reflection.Assembly> container,
        Func<AssemblyName, bool> predicate
    )
    {
        var referencedAssemblies = current.GetReferencedAssemblies().Where(predicate);

        foreach (var referencedAssembly in referencedAssemblies)
        {
            var referencedAssemblyName = referencedAssembly.ToString();
            if (loaded.Contains(referencedAssemblyName))
            {
                continue;
            }

            var assembly = System.Reflection.Assembly.Load(referencedAssembly);
            LoadAssemblies(assembly, loaded, container, predicate);
            loaded.Add(referencedAssemblyName);
            container.Add(assembly);
        }

        return container.ToArray();
    }

    private static Func<AssemblyName, bool> CreateAssemblyPrefixPredicate(string prefix) =>
        assemblyName => assemblyName.Name!.StartsWith(prefix);
}
