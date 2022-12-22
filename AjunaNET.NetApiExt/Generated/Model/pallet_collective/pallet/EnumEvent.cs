//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Ajuna.NetApi.Model.Types.Base;
using System.Collections.Generic;


namespace AjunaNET.NetApiExt.Generated.Model.pallet_collective.pallet
{
    
    
    public enum Event
    {
        
        Proposed = 0,
        
        Voted = 1,
        
        Approved = 2,
        
        Disapproved = 3,
        
        Executed = 4,
        
        MemberExecuted = 5,
        
        Closed = 6,
    }
    
    /// <summary>
    /// >> 44 - Variant[pallet_collective.pallet.Event]
    /// 
    ///			The [event](https://docs.substrate.io/main-docs/build/events-errors/) emitted
    ///			by this pallet.
    ///			
    /// </summary>
    public sealed class EnumEvent : BaseEnumExt<Event, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, Ajuna.NetApi.Model.Types.Primitive.U32>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, Ajuna.NetApi.Model.Types.Primitive.Bool, Ajuna.NetApi.Model.Types.Primitive.U32, Ajuna.NetApi.Model.Types.Primitive.U32>, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, BaseTuple<AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, AjunaNET.NetApiExt.Generated.Types.Base.EnumResult>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, AjunaNET.NetApiExt.Generated.Types.Base.EnumResult>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.primitive_types.H256, Ajuna.NetApi.Model.Types.Primitive.U32, Ajuna.NetApi.Model.Types.Primitive.U32>>
    {
    }
}
