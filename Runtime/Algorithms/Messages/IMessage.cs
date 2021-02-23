using Plucky.Common;

namespace knockback
{
    public interface IMessage
    {
        /// <summary>
        /// type is the type of message. E.g. Damage Dealt or Death
        /// </summary>
        string type { get; }

        /// <summary>
        /// identifier is a message specific identifier for the message. This is message specific
        /// and may be any value including null. For example, entity ID of attacker.
        /// </summary>
        string identifier { get; }

        /// <summary>
        /// value is a message depedent value stored in the message. The implementing class may
        /// also support other value type as needed.
        /// </summary>
        Variant value { get; }
    }
}
