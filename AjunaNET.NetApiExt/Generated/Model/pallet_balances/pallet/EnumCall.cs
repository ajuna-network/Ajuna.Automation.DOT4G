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


namespace AjunaNET.NetApiExt.Generated.Model.pallet_balances.pallet
{
    
    
    public enum Call
    {
        
        transfer = 0,
        
        set_balance = 1,
        
        force_transfer = 2,
        
        transfer_keep_alive = 3,
        
        transfer_all = 4,
        
        force_unreserve = 5,
    }
    
    /// <summary>
    /// >> 138 - Variant[pallet_balances.pallet.Call]
    /// Contains one variant per dispatchable that can be called by an extrinsic.
    /// </summary>
    public sealed class EnumCall : BaseEnumExt<Call, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U128>>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U128>, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U128>>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U128>>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U128>>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Primitive.Bool>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Primitive.U128>>
    {
    }
}
