namespace Cairn {
    public interface IFunction<TResult> {
        TResult Return();
    }

    public interface IFunction<T1, TResult> {
        bool Condition(T1 arg);
        TResult Return(T1 arg);
    }

    public interface IFunction<T1, T2, TResult> {
        bool Condition(T1 arg1, T2 arg2);
        TResult Return(T1 arg1, T2 arg2);
    }

    public interface IFunction<T1, T2, T3, TResult> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3);
        TResult Return(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IFunction<T1, T2, T3, T4, TResult> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        TResult Return(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IFunction<T1, T2, T3, T4, T5, TResult> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        TResult Return(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IFunction<T1, T2, T3, T4, T5, T6, TResult> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        TResult Return(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IFunction<T1, T2, T3, T4, T5, T6, T7, TResult> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        TResult Return(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
        TResult Return(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }
}
