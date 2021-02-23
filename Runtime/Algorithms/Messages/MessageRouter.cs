using System;
using System.Collections.Generic;

namespace knockback
{
    /// <summary>
    /// MessageRouter is a very simple and efficient pub/sub message router.
    /// 
    /// NOTE: Listeners should not hold on to the messages, they may be reused after being 
    /// broadcast.
    /// 
    /// MessageRouter is routes messages sychronously and should only be used from the main thread.
    /// You are guaranteed that all listeners will have received the message when Broadcast 
    /// returns.
    /// </summary>
    public class MessageRouter
    {
        public delegate void ListenerFunc(IMessage message);

        /// <summary>
        /// typeListeners are listeners that are listening only for a specific message type and
        /// don't care about the identifier.
        /// </summary>
        Dictionary<string, List<ListenerFunc>> typeListeners = new Dictionary<string, List<ListenerFunc>>();

        /// <summary>
        /// compositeListeners are listeners that are listening for both a message type and 
        /// identifier.
        /// </summary>
        Dictionary<Tuple<string, string>, List<ListenerFunc>> compositeListeners = new Dictionary<Tuple<string, string>, List<ListenerFunc>>();

        static MessageRouter _instance;
        public static MessageRouter instance
        {
            get
            {
                if (_instance == null) _instance = new MessageRouter();
                return _instance;
            }
        }

        public void Broadcast(IMessage msg)
        {
            if (typeListeners.TryGetValue(msg.type, out List<ListenerFunc> funcs))
            {
                for (int i = 0; i < funcs.Count; i++)
                {
                    funcs[i].Invoke(msg);
                }
            }
            if (msg.identifier != null &&
                compositeListeners.TryGetValue(new Tuple<string, string>(msg.type, msg.identifier), out List<ListenerFunc> compFuncs))
            {
                for (int i = 0; i < compFuncs.Count; i++)
                {
                    compFuncs[i].Invoke(msg);
                }
            }
        }

        public void Subscribe(string messageType, ListenerFunc func)
        {
            List<ListenerFunc> funcs;
            if (!typeListeners.TryGetValue(messageType, out funcs))
            {
                funcs = new List<ListenerFunc> { func };
                typeListeners[messageType] = funcs;
            }
            else if (!funcs.Contains(func))
            {
                funcs.Add(func);
            }
        }

        public void Subscribe(string messageType, string identifier, ListenerFunc func)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                Subscribe(messageType, func);
            }
            else
            {
                Tuple<string, string> tuple = new Tuple<string, string>(messageType, identifier);
                List<ListenerFunc> funcs;
                if (!compositeListeners.TryGetValue(tuple, out funcs))
                {
                    funcs = new List<ListenerFunc> { func };
                    compositeListeners[tuple] = funcs;
                }
                else if (!funcs.Contains(func))
                {
                    funcs.Add(func);
                }
            }
        }


    }
}
