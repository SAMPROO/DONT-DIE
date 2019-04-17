#if NOT_DEFINED

public interface IJumpBonus
{
	float Value { get; }
}

[RequireComponent(typeof(Rigidbody))]
public class DivingBoardJumpBonus : MonoBehaviour, IJumpBonus
{
	private float jumpBonusValue;
	float IJumpBonus.Value => jumpBonusValue;

	private Rigidbody rigidbody;

	void FixedUpdate()
	{
		jumpBonusValue = Mathf.Max(0, rigidbody.velocity.y);
	}
}
#endif