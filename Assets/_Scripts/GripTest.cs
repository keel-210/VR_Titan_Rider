using UnityEngine;

public class GripTest : MonoBehaviour
{
	[SerializeField] Vector3 offset;
	[SerializeField] BoneDirections boneDirections;
	[SerializeField] Mode mode;
	Rigidbody rigidbody;
	Vector3 pseudX, pseudY, pseudZ, localGripPos;
	void Start ()
	{
		rigidbody = GetComponent<Rigidbody> ();
		localGripPos = transform.localPosition;

	}
	void Update ()
	{
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		switch (mode)
		{
			case Mode.Local:
				transform.localPosition += offset;
				break;
			case Mode.Pseud_Parent:
				Vector3 pseudX = Vector3.zero, pseudY = Vector3.zero, pseudZ = Vector3.zero;
				if (boneDirections.GetPseuds (transform.parent, out pseudX, out pseudY, out pseudZ))
				{
					transform.localPosition += offset.x * pseudX + offset.y * pseudY + offset.z * pseudZ;
				}
				else
				{
					Debug.Log ("Failed To Get Pseud BoneDirection");
				}
				break;
			case Mode.Tra_up:
				transform.localPosition = localGripPos + offset.y * transform.up;
				break;
			case Mode.Pseud_Up:
				if (boneDirections.GetPseuds (transform.parent, out pseudX, out pseudY, out pseudZ))
				{
					transform.localPosition = localGripPos + offset.x * pseudX + offset.y * pseudY + offset.z * pseudZ;
				}
				else
				{
					Debug.Log ("Failed To Get Pseud BoneDirection");
				}
				break;
		}
	}
	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, transform.position + pseudX * 10);
		Gizmos.color = Color.green;
		Gizmos.DrawLine (transform.position, transform.position + pseudY * 10);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (transform.position, transform.position + pseudZ * 10);
	}
	enum Mode
	{
		Local,
		Pseud_Parent,
		Tra_up,
		Pseud_Up,
	}
}