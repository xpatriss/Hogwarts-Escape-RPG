using System;
using System.Data;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Network payload representing a player-initiated action request.
    /// Pattern: DTO (Data Transfer Object) — transfers command intent from client to authoritative server.
    /// </summary>
    public class ActionDTO
    {
        /// <summary>Unique session ID of the emitting player assigned by the server.</summary>
        public int PlayerId { get; set; }

        /// <summary>The gameplay action to execute.</summary>
        public CommandType Command { get; set; }

        /// <summary>Context-dependent index (e.g. inventory slot, tile item index, or attack type index).</summary>
        public int TargetIndex { get; set; }
    }
}