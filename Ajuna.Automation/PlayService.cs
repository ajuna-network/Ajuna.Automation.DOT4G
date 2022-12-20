using System.Threading;
using System.Threading.Tasks;
using Ajuna.Automation.AI;
using Ajuna.Automation.Model;
using Ajuna.NetApi.Model.Types;

namespace Ajuna.Automation;

public class PlayService
{
    private readonly string _nodeUrl;
    private readonly string _workerUrl;
    private readonly string _mrEnclave;
    private readonly string _shard;

    public PlayService(string nodeUrl, string workerUrl, string mrEnclave, string shard)
    {
        _nodeUrl = nodeUrl;
        _workerUrl = workerUrl;
        this._mrEnclave = mrEnclave;
        _shard = shard;
    }
    
    
    public async Task PlayAsync(Account account, CancellationToken token)
    {
        var nodeClient = new NodeClient(account, _nodeUrl);
        var workerClient = new WorkerClient(account, _workerUrl, _shard, _mrEnclave);

        var logic = new StraightAI();

        var client = new PlayBot(nodeClient, workerClient, logic);
        await client.RunAsync(token);
    }
}