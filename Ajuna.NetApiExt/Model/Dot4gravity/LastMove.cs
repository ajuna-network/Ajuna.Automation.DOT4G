//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApi.Model.SpCore;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using System;
using System.Collections.Generic;


namespace Ajuna.NetApi.Model.Dot4gravity
{
    
    
    /// <summary>
    /// >> 226 - Composite[dot4gravity.LastMove]
    /// </summary>
    public sealed class LastMove : BaseType
    {
        
        /// <summary>
        /// >> player
        /// </summary>
        private Ajuna.NetApi.Model.SpCore.AccountId32 _player;
        
        /// <summary>
        /// >> side
        /// </summary>
        private Ajuna.NetApi.Model.Dot4gravity.EnumSide _side;
        
        /// <summary>
        /// >> position
        /// </summary>
        private Ajuna.NetApi.Model.Types.Primitive.U8 _position;
        
        public Ajuna.NetApi.Model.SpCore.AccountId32 Player
        {
            get
            {
                return this._player;
            }
            set
            {
                this._player = value;
            }
        }
        
        public Ajuna.NetApi.Model.Dot4gravity.EnumSide Side
        {
            get
            {
                return this._side;
            }
            set
            {
                this._side = value;
            }
        }
        
        public Ajuna.NetApi.Model.Types.Primitive.U8 Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }
        
        public override string TypeName()
        {
            return "LastMove";
        }
        
        public override byte[] Encode()
        {
            var result = new List<byte>();
            result.AddRange(Player.Encode());
            result.AddRange(Side.Encode());
            result.AddRange(Position.Encode());
            return result.ToArray();
        }
        
        public override void Decode(byte[] byteArray, ref int p)
        {
            var start = p;
            Player = new Ajuna.NetApi.Model.SpCore.AccountId32();
            Player.Decode(byteArray, ref p);
            Side = new Ajuna.NetApi.Model.Dot4gravity.EnumSide();
            Side.Decode(byteArray, ref p);
            Position = new Ajuna.NetApi.Model.Types.Primitive.U8();
            Position.Decode(byteArray, ref p);
            TypeSize = p - start;
        }
    }
}
