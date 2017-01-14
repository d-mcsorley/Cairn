namespace Cairn {
    public interface IAction {
        void Execute();
    }

    public interface IAction<T1> {
        bool Condition(T1 arg);
        void Execute(T1 arg);
    }

    public interface IAction<T1, T2> {
        bool Condition(T1 arg1, T2 arg2);
        void Execute(T1 arg1, T2 arg2);
    }

    public interface IAction<T1, T2, T3> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3);
        void Execute(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IAction<T1, T2, T3, T4> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IAction<T1, T2, T3, T4, T5> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IAction<T1, T2, T3, T4, T5, T6> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IAction<T1, T2, T3, T4, T5, T6, T7> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8> {
        bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
        void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }
}
