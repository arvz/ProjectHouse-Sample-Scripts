using Cysharp.Threading.Tasks;

/// <summary>
/// An entity that can interact with InteractHandler(s)
/// </summary>
public interface IInteractor
{
   public UniTask PreInteract(InteractHandler interactHandler);

   public UniTask PlayAnimation(CharacterAnimationState characterAnimationStateType);

   public void FinishInteraction();

}
