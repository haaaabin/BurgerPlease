using UnityEngine;

public class PakingPile : PileBase
{
	public void Awake()
	{
		_size = new Vector3(0.8f, 0.25f, 0.8f);
		_objectType = Define.EObjectType.PakingBox;

	}
}