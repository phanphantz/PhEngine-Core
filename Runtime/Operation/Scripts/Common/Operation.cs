using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR && EDITOR_COROUTINE
using Unity.EditorCoroutines.Editor;
#endif

namespace PhEngine.Core.Operation
{
    public class Operation : BaseOperation
    {
        protected MonoBehaviour Host => host;
        MonoBehaviour host;
        Coroutine activeRoutine;
        bool isUseCoroutine;

#if UNITY_EDITOR && EDITOR_COROUTINE
        EditorCoroutine activeEditorRoutine;
#endif
        public Operation()
        {
        }

        public Operation(Action action) : base(action)
        {
        }

        protected override void RunInternally()
        {
            RunOn(Host);
        }

        public virtual void RunOn(MonoBehaviour target)
        {
            if (!TryStart())
                return;

            host = target;
            isUseCoroutine = true;
            var routine = Coroutine(CurrentRound);
            if (Application.isPlaying && host)
            {
                activeRoutine = host.StartCoroutine(routine);
            }
            else
            {
#if UNITY_EDITOR && EDITOR_COROUTINE
                if (host == null)
                    EditorCoroutineUtility.StartCoroutineOwnerless(routine);
                else
                    activeEditorRoutine = EditorCoroutineUtility.StartCoroutine(routine, host);
#else
                throw new ArgumentNullException(nameof(target));
#endif
            }
        }

        IEnumerator Coroutine(int assignedRound)
        {
            yield return StartDelay;
            InvokeOnStart();

            if (TryFinishOrKill(assignedRound))
                yield break;

            while (!TryFinishOrKill(assignedRound))
            {
                PassTimeByDeltaTime();
                if (TryFinishOrKill(assignedRound))
                    yield break;

                InvokeOnUpdate();
                if (TryFinishOrKill(assignedRound))
                    yield break;

                RefreshProgress();
                if (TryFinishOrKill(assignedRound))
                    yield break;

                yield return UpdateDelay;
            }
        }

        public async UniTask Task()
        {
            if (!TryStart())
                return;

            if (StartDelay != null)
                await StartDelay;

            InvokeOnStart();

            var result = await InternalTask();
            switch (result)
            {
                case Ending.Finish:
                {
                    NotifyFinish();
                    if (IsShouldRepeat())
                        await Task();
                    else
                        CurrentRound = 0;
                    break;
                }
                case Ending.Cancel:
                    NotifyCancel();
                    break;
            }
        }

        async UniTask<Ending> InternalTask()
        {
            while (true)
            {
                PassTimeByDeltaTime();
                var runningStatus = GetRunningStatus(CurrentRound);
                if (runningStatus != Ending.NotReached)
                    return runningStatus;

                InvokeOnUpdate();
                runningStatus = GetRunningStatus(CurrentRound);
                if (runningStatus != Ending.NotReached)
                    return runningStatus;

                RefreshProgress();
                runningStatus = GetRunningStatus(CurrentRound);
                if (runningStatus != Ending.NotReached)
                    return runningStatus;

                if (UpdateDelay != null)
                    await UpdateDelay;
                else
                    await UniTask.Yield();
            }
        }

        protected override void NotifyCancel()
        {
            if (isUseCoroutine)
                CancelCoroutine();

            base.NotifyCancel();
        }

        void CancelCoroutine()
        {
#if UNITY_EDITOR && EDITOR_COROUTINE
            if (activeEditorRoutine != null)
                EditorCoroutineUtility.StopCoroutine(activeEditorRoutine);

            activeEditorRoutine = null;
#endif
            if (host)
            {
                if (activeRoutine != null)
                    host.StopCoroutine(activeRoutine);

                activeRoutine = null;
                host = null;
            }

            isUseCoroutine = false;
        }

        public CustomYieldInstruction Wait()
        {
            this.Run();
            return new WaitUntil(() => IsFinished);
        }

        public CustomYieldInstruction WaitOn(MonoBehaviour target)
        {
            RunOn(target);
            return new WaitUntil(() => IsFinished);
        }

        public static Operation Create()
        {
            return new Operation();
        }

        public static Operation From(Action action)
        {
            return new Operation(action);
        }
    }
}