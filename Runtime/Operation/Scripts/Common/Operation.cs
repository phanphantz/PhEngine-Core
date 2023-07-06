using System;
using System.Collections;
using UnityEngine;

#if UNITASK
using Cysharp.Threading.Tasks;
#endif

#if UNITY_EDITOR && EDITOR_COROUTINE
using Unity.EditorCoroutines.Editor;
#endif

namespace PhEngine.Core.Operation
{
    public class Operation : OperationCore
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
            var routine = Coroutine();
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

        IEnumerator Coroutine()
        {
            yield return StartDelay;
            InvokeOnStart();

            if (TryFinishOrKill())
                yield break;

            while (true)
            {
                PassTimeByDeltaTime();
                if (TryFinishOrKill())
                    yield break;

                InvokeOnUpdate();
                if (TryFinishOrKill())
                    yield break;

                RefreshProgress();
                if (TryFinishOrKill())
                    yield break;

                yield return UpdateDelay;
            }
        }

#if UNITASK
        public async UniTask Task()
        {
            await PreProcessTask();
            if (!TryStart())
                throw new OperationCanceledException();

            if (StartDelay != null)
                await StartDelayTask;

            InvokeOnStart();

            var result = await WaitUntilFinishTask();
            switch (result)
            {
                case EndingStatus.Ended:
                    NotifyFinish();
                    break;
                
                case EndingStatus.Cancelled:
                    NotifyCancel();
                    throw new OperationCanceledException();
            }
            await PostProcessTask();
        }

        protected virtual async UniTask PreProcessTask()
        {
            await UniTask.Yield();
        }
        
        protected virtual async UniTask PostProcessTask()
        {
            await UniTask.Yield();
        }

        async UniTask<EndingStatus> WaitUntilFinishTask()
        {
            while (true)
            {
                PassTimeByDeltaTime();
                var runningStatus = GetEndingStatus();
                if (runningStatus != EndingStatus.NotReached)
                    return runningStatus;

                InvokeOnUpdate();
                runningStatus = GetEndingStatus();
                if (runningStatus != EndingStatus.NotReached)
                    return runningStatus;

                RefreshProgress();
                runningStatus = GetEndingStatus();
                if (runningStatus != EndingStatus.NotReached)
                    return runningStatus;

                if (UpdateDelay != null)
                    await UpdateDelayTask;
                else
                    await UniTask.Yield();
            }
        }
#endif

        protected override void NotifyFinish()
        {
            base.NotifyFinish();
            isUseCoroutine = false;
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
    }
}