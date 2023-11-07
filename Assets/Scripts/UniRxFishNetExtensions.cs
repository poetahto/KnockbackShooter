using System;
using FishNet.Object.Synchronizing;
using UniRx;

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
}