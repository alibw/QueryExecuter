using Grpc.Core;
using NUnit.Framework;
using QueryExecuter;

namespace QueryExecuterClient;

[TestFixture]
public class Test
{
    [Test]
    public void ExecuteQuery()
    {
        var channel = new Channel("localhost:5282", ChannelCredentials.Insecure);
        var client = new QueryExecuter.QueryExecuter.QueryExecuterClient(channel);
        var result = client.ExecuteQuery(new QueryParameter()
        {
            Names = { "MyQuery"}
        });
    }
    
}