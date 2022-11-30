using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using Grpc.Core;

namespace QueryExecuter.Services;

public class ExecuterService : QueryExecuter.QueryExecuterBase
{
    private const string path = @"C:\Users\Administrator\Documents\LINQPad Queries\Queries To Use";
    public override async Task<QueryResultList> ExecuteQuery(QueryParameter request, ServerCallContext context)
    {
        QueryResultList resultList = new QueryResultList();
        foreach (var file in request.Names)
        {
            QueryCompiler.CompileAndRunQuery(file);
        }
        return await Task.FromResult(resultList);
    }
}