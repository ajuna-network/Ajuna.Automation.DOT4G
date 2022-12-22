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


namespace AjunaNET.NetApiExt.Generated.Model.pallet_assets.pallet
{
    
    
    public enum Event
    {
        
        Created = 0,
        
        Issued = 1,
        
        Transferred = 2,
        
        Burned = 3,
        
        TeamChanged = 4,
        
        OwnerChanged = 5,
        
        Frozen = 6,
        
        Thawed = 7,
        
        AssetFrozen = 8,
        
        AssetThawed = 9,
        
        Destroyed = 10,
        
        ForceCreated = 11,
        
        MetadataSet = 12,
        
        MetadataCleared = 13,
        
        ApprovedTransfer = 14,
        
        ApprovalCancelled = 15,
        
        TransferredApproved = 16,
        
        AssetStatusChanged = 17,
    }
    
    /// <summary>
    /// >> 39 - Variant[pallet_assets.pallet.Event]
    /// 
    ///			The [event](https://docs.substrate.io/main-docs/build/events-errors/) emitted
    ///			by this pallet.
    ///			
    /// </summary>
    public sealed class EnumEvent : BaseEnumExt<Event, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U128>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U128>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U128>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, Ajuna.NetApi.Model.Types.Primitive.U32, Ajuna.NetApi.Model.Types.Primitive.U32, Ajuna.NetApi.Model.Types.Primitive.U32, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, Ajuna.NetApi.Model.Types.Base.BaseVec<Ajuna.NetApi.Model.Types.Primitive.U8>, Ajuna.NetApi.Model.Types.Base.BaseVec<Ajuna.NetApi.Model.Types.Primitive.U8>, Ajuna.NetApi.Model.Types.Primitive.U8, Ajuna.NetApi.Model.Types.Primitive.Bool>, Ajuna.NetApi.Model.Types.Primitive.U32, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U128>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, BaseTuple<Ajuna.NetApi.Model.Types.Primitive.U32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, AjunaNET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Ajuna.NetApi.Model.Types.Primitive.U128>, Ajuna.NetApi.Model.Types.Primitive.U32>
    {
    }
}
