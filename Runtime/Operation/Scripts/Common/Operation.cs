using System;
using System.Collections;
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
        
#if UNITY_EDITOR && EDITOR_COROUTINE
        EditorCoroutine activeEditorRoutine;
#endif
        bool isRunningAsExternalCoroutine;
        
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
            var routine = InternalCoroutine(CurrentRound);
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
        
        protected override void NotifyCancel()
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
            base.NotifyCancel();
        }

        protected override bool TryStart()
        {
            isRunningAsExternalCoroutine = false;
            return base.TryStart();
        }
        
        protected override void TryRepeat()
        {
            if (isRunningAsExternalCoroutine)
                return;

            base.TryRepeat();
        }
        
        public IEnumerator Coroutine()
        {
            while (IsShouldRepeat() || !isRunningAsExternalCoroutine)
            {
                if (!TryStart())
                    yield break;

                isRunningAsExternalCoroutine = true;
                yield return InternalCoroutine(CurrentRound);
            }
            CurrentRound = 0;
        }
        
        IEnumerator InternalCoroutine(int assignedRound)
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