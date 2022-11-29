using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using Grpc.Core;

namespace QueryExecuter.Services;

public class ExecuterService : QueryExecuter.QueryExecuterBase
{
    string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../"));

    public override async Task<QueryResultList> exec(QueryParameter request, ServerCallContext context)
    {
        QueryResultList resultList = new QueryResultList();
        foreach (var name in request.Names)
        {
            var result = await Cli.Wrap(@"C:\Program Files\LINQPad7\lprun7.exe")
                .WithArguments($@"{path}Queries\{name}.linq")
                .WithValidation(CommandResultValidation.None)
                .WithWorkingDirectory("C:\\Users\\Ali")
                .ExecuteBufferedAsync();
            
            var queryResult = new QueryResult()
            {
                QueryName = name,
                Result = result.StandardOutput,
                Error = result.StandardError
            };
            resultList.QueryResultList_.Add(queryResult);
        }

        return await Task.FromResult(resultList);
    }
}