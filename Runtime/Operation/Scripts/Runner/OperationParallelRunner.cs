namespace PhEngine.Core.Operation
{
    public abstract class OperationParallelRunner<T> : OperationRunner<T> where T : Operation
    {
        public override void RunAll()
        {
            this.RunAsParallel(OnStopBehavior.Skip,operationList.ToArray());
        }
        
        public override void Pause()
        {
            operationList.ForEach(PauseOperation);
        }
        
        public override void Resume()
        {
            operationList.ForEach(ResumeOperation);
        }
        
        public override void ForceFinish()
        {
            operationList.ForEach(ForceFinishOperation);
        }
        
        public override void ForceCancel()
        {
            operationList.ForEach(ForceCancelOperation);
        }
    }
}