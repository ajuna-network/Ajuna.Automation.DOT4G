using Ajuna.NetApi.Model.Dot4gravity;

namespace Ajuna.NetApiExt.Model.AjunaWorker.Dot4G
{
    public class Dot4GMove
    {
        public int Id { get; internal set; }
        public string PlayerAddress { get; internal set; }
        public Side Side { get; internal set; }
        public int Column { get; internal set; }
    }
}
