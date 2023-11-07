using System;
using TMPro;
using UniRx;

public static class UniRxTMPTools
{
    public static IDisposable SubscribeToText<T>(this IObservable<T> source, TMP_Text text)
    {
        return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
    }

    public static IDisposable SubscribeToText(this IObservable<string> source, TMP_Text text)
    {
        return source.SubscribeWithState(text, (x, t) => t.text = x);
    }
}