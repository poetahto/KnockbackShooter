using System;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object.Synchronizing;
using UniRx;

namespace Util
{
    public static class UniRxFishNetExtensions
    {
        public struct SyncVarChangeData<T>
        {
            public T Previous;
            public T Next;
            public bool AsServer;
        }
    
        public static IObservable<SyncVarChangeData<T>> ObserveChanged<T>(this SyncVar<T> syncVar)
        {
            return Observable.FromEvent<SyncVar<T>.OnChanged, SyncVarChangeData<T>>(
                action =>
                {
                    return (prev, next, server) => action(new SyncVarChangeData<T>
                    {
                        AsServer = server, 
                        Next = next, 
                        Previous = prev
                    });
                }, 
                changed => syncVar.OnChange += changed,
                changed => syncVar.OnChange -= changed
            );
        }

        public struct ClientJoinData
        {
            public NetworkConnection Connection;
            public bool AsServer;
        }

        public static IObservable<ClientJoinData> ObserveClientJoin(this ServerManager serverManager)
        {
            return Observable.FromEvent<Action<NetworkConnection, bool>, ClientJoinData>(
                action =>
                {
                    return (connection, asServer) => action(new ClientJoinData
                    {
                        Connection = connection, 
                        AsServer = asServer
                    });
                },
                action => serverManager.OnAuthenticationResult += action,
                action => serverManager.OnAuthenticationResult -= action
            );
        }

        public static IObservable<Unit> ObserveComplete(this SyncTimer timer)
        {
            return Observable.FromEvent<SyncTimer.SyncTypeChanged, Unit>(
                action =>
                {
                    return (op, prev, next, server) =>
                    {
                        if (op == SyncTimerOperation.Finished)
                            action(Unit.Default);
                    };
                },
                changed => timer.OnChange += changed,
                changed => timer.OnChange -= changed
            );
        }
    }
}