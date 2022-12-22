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


namespace AjunaNET.NetApiExt.Generated.Model.pallet_ajuna_board.pallet
{
    
    
    public enum Error
    {
        
        InvalidGameState = 0,
        
        InvalidTurn = 1,
        
        InvalidPlayers = 2,
        
        NotPlaying = 3,
        
        AlreadyInGame = 4,
        
        AlreadyQueued = 5,
        
        UnknownBoard = 6,
    }
    
    /// <summary>
    /// >> 325 - Variant[pallet_ajuna_board.pallet.Error]
    /// 
    ///			Custom [dispatch errors](https://docs.substrate.io/main-docs/build/events-errors/)
    ///			of this pallet.
    ///			
    /// </summary>
    public sealed class EnumError : BaseEnum<Error>
    {
    }
}
