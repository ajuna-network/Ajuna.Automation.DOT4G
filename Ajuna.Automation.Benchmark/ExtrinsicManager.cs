using Ajuna.NetApi.Model.Rpc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ajuna.Automation
{
    public class QueueInfo
    {
        public string State { get; private set; }

        public string ExtrinsicType { get; }

        public DateTime Created { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public bool IsSuccess => State == "Finalized";

        public bool IsFail => State == "Invalid" 
                           || State == "Dropped";

        public bool IsRunning => !IsSuccess && !IsFail;

        public bool IsFinish =>  IsSuccess || IsFail;

        public double TimeElapsed => DateTime.Now.Subtract(LastUpdated).TotalSeconds;

        public QueueInfo(string extrinsicType)
        {
            ExtrinsicType = extrinsicType;
            Created = DateTime.Now;
            LastUpdated = Created;
            State = "";
        }

        internal void Update(string state)
        {
            LastUpdated = DateTime.Now;
            State = state;
        }

    }
    
    public class ExtrinsicManager
    {
        private readonly int _ttl;
        private readonly Dictionary<string, QueueInfo> _data;

        public IEnumerable<QueueInfo> Running => _data.Values.Where(p => p.IsRunning);

        public ExtrinsicManager(int ttl)
        {
            _ttl = ttl;
            _data = new Dictionary<string, QueueInfo>();
        }

        public void Add(string subscription, string extrinsicType = null)
        {
            _data.Add(subscription, new QueueInfo(extrinsicType));

            if (_data.Count > 100)
            {
                Clean();
            }
        }

        public QueueInfo? Get(string id)
        {
            if (!_data.TryGetValue(id, out QueueInfo? queueInfo))
            {
                Log.Warning("Retrieving unregeistred or removed subscriptionId {id}", id);
                return queueInfo;
            }

            return queueInfo;
        }

        public void Clean()
        {
            var toRemove = new List<string>();
            foreach (var kvp in _data)
            {
                if (kvp.Value.TimeElapsed > _ttl)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach(var key in toRemove)
            {
                _data.Remove(key);
            }

            Log.Information("Removing {count} etrinsics", toRemove);
        }

        /// <summary>
        /// Simple extrinsic tester
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="extrinsicUpdate"></param>
        public void ActionExtrinsicUpdate(string subscriptionId, ExtrinsicStatus extrinsicUpdate)
        {
            if (!_data.TryGetValue(subscriptionId, out QueueInfo queueInfo))
            {
                Log.Warning("Unregistred or removed subscriptionId {id} got update", subscriptionId, extrinsicUpdate.ExtrinsicState);
                return;
            }

            switch (extrinsicUpdate.ExtrinsicState)
            {
                case ExtrinsicState.None:
                    if (extrinsicUpdate.InBlock?.Value.Length > 0)
                    {
                        Log.Debug("{id} InBlock {hash} got update", subscriptionId, extrinsicUpdate.InBlock.Value);
                        queueInfo.Update("InBlock");
                    }
                    else if (extrinsicUpdate.Finalized?.Value.Length > 0)
                    {
                        Log.Debug("{id} Finalized {hash} got update", subscriptionId, extrinsicUpdate.Finalized.Value);
                        queueInfo.Update("Finalized");
                    }
                    else
                    {
                        Log.Debug("{id} updated to {state}", subscriptionId, extrinsicUpdate.ExtrinsicState);
                        queueInfo.Update("None");
                    };
                    break;

                case ExtrinsicState.Future:
                    Log.Debug("{id} updated to Future", subscriptionId);
                    queueInfo.Update("Future");
                    break;

                case ExtrinsicState.Ready:
                    Log.Debug("{id} updated to Ready", subscriptionId);
                    queueInfo.Update("Ready");
                    break;

                case ExtrinsicState.Dropped:
                    Log.Debug("{id} updated to Dropped", subscriptionId);
                    queueInfo.Update("Dropped");
                    break;

                case ExtrinsicState.Invalid:
                    Log.Debug("{id} updated to Invalid", subscriptionId);
                    queueInfo.Update("Invalid");
                    break;
            }
        }
    }
}