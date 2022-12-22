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


namespace AjunaNET.NetApiExt.Generated.Model.pallet_proxy.pallet
{
    
    
    public enum Call
    {
        
        proxy = 0,
        
        add_proxy = 1,
        
        remove_proxy = 2,
        
        remove_proxies = 3,
        
        create_pure = 4,
        
        kill_pure = 5,
        
        announce = 6,
        
        remove_announcement = 7,
        
        reject_announcement = 8,
        
        proxy_announced = 9,
    }
    
    /// <summary>
    /// >> 215 - Variant[pallet_proxy.pallet.Call]
    /// Contains one variant per dispatchable that can be called by an extrinsic.
    /// </summary>
    public sealed class EnumCall : BaseEnumExt<Call, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Base.BaseOpt<AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.types.proxy.EnumProxyType>, AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.EnumRuntimeCall>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.types.proxy.EnumProxyType, Ajuna.NetApi.Model.Types.Primitive.U32>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.types.proxy.EnumProxyType, Ajuna.NetApi.Model.Types.Primitive.U32>, BaseVoid, BaseTuple<AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.types.proxy.EnumProxyType, Ajuna.NetApi.Model.Types.Primitive.U32, Ajuna.NetApi.Model.Types.Primitive.U16>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.types.proxy.EnumProxyType, Ajuna.NetApi.Model.Types.Primitive.U16, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U32>, Ajuna.NetApi.Model.Types.Base.BaseCom<Ajuna.NetApi.Model.Types.Primitive.U32>>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.primitive_types.H256>, BaseTuple<AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress.EnumMultiAddress, Ajuna.NetApi.Model.Types.Base.BaseOpt<AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.types.proxy.EnumProxyType>, AjunaNET.NetApiExt.Generated.Model.ajuna_solo_runtime.EnumRuntimeCall>>
    {
    }
}
