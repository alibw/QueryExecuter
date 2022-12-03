using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using CliWrap;
using CliWrap.Buffered;

namespace QueryExecuter;

public static class QueryCompiler
{
    private const string QueriesPath = @"C:\Users\Ali\Documents\LINQPad Queries\Queries To Use";
    private const string LinqPadPath = @"C:\Program Files\LINQPad7\lprun7.exe";
    
    public static void CompileQueries()
    {
        foreach (var file in Directory.GetFiles(QueriesPath))
        {
            if (!CheckCachedAssemblyApproval(file))
                CompileQuery(file);
        }
    }
    
    public static object CompileAndRunQuery(string fileName)
    {
        var filePath = $@"{QueriesPath}\{fileName}.linq";
        if (!CheckCachedAssemblyApproval(filePath))
            return RunQueryWithLpRun(filePath);
        
        return RunQueryWithAssembly(filePath);
    }

    private static object RunQueryWithAssembly(string filePath)
    {
        var assemblyPath = GetCachedAssemblyPath(filePath);
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        var type = assembly.GetType("UserQuery");
        var instance = Activator.CreateInstance(type);
        var mainMethod = type.GetMethod("Main",BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            mainMethod.Invoke(instance, null);
        }
        catch (Exception e)
        {
            return new
            {
                QueryName = filePath,
                Result = "Failed",
                Error = e.Message
            };
        }

        return new
        {
            QueryName = filePath,
            Result = "Success"
        };
    }

    private static async Task<object> RunQueryWithLpRun(string filePath)
    {
        var result = await Cli.Wrap(LinqPadPath)
            .WithArguments(new []{filePath})
            .ExecuteBufferedAsync();
        return Task.FromResult(new
        {
            QueryName = filePath,
            Result = result.StandardOutput,
            Error = result.StandardError
        });
    }

    private static async void CompileQuery(string filePath)
    {
        var result = await Cli.Wrap(LinqPadPath)
            .WithArguments(new[] { "-compileonly",filePath})
            .ExecuteBufferedAsync();
    }

    private static string GetCachedAssemblyPath(string filePath)
    {
        string queryPath = filePath;
        var s = SHA1.Create();
        string assemblyFile = FormatHex(s.ComputeHash(Encoding.UTF8.GetBytes(filePath.ToLowerInvariant() + "." +
                                                                             false + "." +
                                                                             RuntimeInformation.ProcessArchitecture)));
        string assemblyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LINQPad\\CompilationCache", assemblyFile + ".dll");
        return assemblyPath;
    }

    private static bool CheckCachedAssemblyApproval(string filePath)
    {
        var assemblyPath = GetCachedAssemblyPath(filePath);
        if (assemblyPath != "")
        {
            var queryLastModifiedDate = File.GetLastWriteTime(filePath);
            var assemblyLastModifiedDate = File.GetLastWriteTime(assemblyPath);
            var compareResult = DateTime.Compare(queryLastModifiedDate, assemblyLastModifiedDate);
            if (compareResult > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private static string FormatHex(byte[] data)
    {
        return string.Concat(data.Select((byte b) => b.ToString("X2")).ToArray());
    }
}