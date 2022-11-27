using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using Grpc.Core;

namespace QueryExecuter.Services;

public class ExecuterService : QueryExecuter.QueryExecuterBase
{
    string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../"));
    
    public override Task<QueryResultList> exec(QueryParameter request, ServerCallContext context)
    {
        QueryResultList resultList = new QueryResultList();
        StringBuilder sb = new StringBuilder();
        foreach (var name in request.Names)
        {
            string command = $@"""C:\Program Files\LINQPad7\lprun7.exe"" ""{path}\Queries\{name}.linq"" ";
            
            try
            {
                var process = Process.Start("cmd.exe",command);
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var queryResult = new QueryResult()
            {
                QueryName = name,
                Result = sb.ToString()
            };
            resultList.QueryResultList_.Add(queryResult);
        }
        
        return Task.FromResult(resultList);
    }
}