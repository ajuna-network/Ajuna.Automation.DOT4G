using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ajuna.NetApi.Model.AjunaWorker
{
    public class RSAPubKey
    {
        public List<byte> N;
        public List<byte> E;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
