public struct PlayerHandle
{
	public readonly int index;

	public PlayerHandle (int index)
	{
		this.index = index;
	}

	public static implicit operator int (PlayerHandle handle)
	{
		return handle.index;
	}
}