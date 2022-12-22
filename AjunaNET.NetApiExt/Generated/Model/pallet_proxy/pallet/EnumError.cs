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
    
    
    public enum Error
    {
        
        TooMany = 0,
        
        NotFound = 1,
        
        NotProxy = 2,
        
        Unproxyable = 3,
        
        Duplicate = 4,
        
        NoPermission = 5,
        
        Unannounced = 6,
        
        NoSelfProxy = 7,
    }
    
    /// <summary>
    /// >> 287 - Variant[pallet_proxy.pallet.Error]
    /// 
    ///			Custom [dispatch errors](https://docs.substrate.io/main-docs/build/events-errors/)
    ///			of this pallet.
    ///			
    /// </summary>
    public sealed class EnumError : BaseEnum<Error>
    {
    }
}
