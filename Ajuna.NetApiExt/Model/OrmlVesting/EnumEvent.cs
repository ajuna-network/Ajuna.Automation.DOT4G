//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Ajuna.NetApi.Model.OrmlVesting;
using Ajuna.NetApi.Model.SpCore;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using System;
using System.Collections.Generic;


namespace Ajuna.NetApi.Model.OrmlVesting
{
    
    
    public enum Event
    {
        
        VestingScheduleAdded,
        
        Claimed,
        
        VestingSchedulesUpdated,
    }
    
    /// <summary>
    /// >> 36 - Variant[orml_vesting.module.Event]
    /// 
    ///			The [event](https://docs.substrate.io/v3/runtime/events-and-errors) emitted
    ///			by this pallet.
    ///			
    /// </summary>
    public sealed class EnumEvent : BaseEnumExt<Event, BaseTuple<Ajuna.NetApi.Model.SpCore.AccountId32, Ajuna.NetApi.Model.SpCore.AccountId32, Ajuna.NetApi.Model.OrmlVesting.VestingSchedule>, BaseTuple<Ajuna.NetApi.Model.SpCore.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U128>, Ajuna.NetApi.Model.SpCore.AccountId32>
    {
    }
}
