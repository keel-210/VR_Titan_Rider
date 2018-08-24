using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoneDirections : MonoBehaviour
{
	//このオブジェクト自身のアニメーター
	private Animator animator;

	//初期状態の回転情報キャッシュ
	private Dictionary<HumanBodyBones, Quaternion> initialRotations;
	private Dictionary<HumanBodyBones, Vector3> pseudXaxis;
	private Dictionary<HumanBodyBones, Vector3> pseudYaxis;
	private Dictionary<HumanBodyBones, Vector3> pseudZaxis;

	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator> ();
		InitializeLocalRotations ();
	}
	public bool GetNowPseuds (Transform tra, out Vector3 pseudX, out Vector3 pseudY, out Vector3 pseudZ)
	{
		var rootT = animator.GetBoneTransform (HumanBodyBones.Hips).root;
		var t = tra;

		pseudX = Vector3.zero;
		pseudY = Vector3.zero;
		pseudZ = Vector3.zero;
		pseudX = new Vector3 (
			Vector3.Dot (t.right, rootT.right),
			Vector3.Dot (t.up, rootT.right),
			Vector3.Dot (t.forward, rootT.right)
		);
		pseudY = new Vector3 (
			Vector3.Dot (t.right, rootT.up),
			Vector3.Dot (t.up, rootT.up),
			Vector3.Dot (t.forward, rootT.up)
		);
		pseudZ = new Vector3 (
			Vector3.Dot (t.right, rootT.forward),
			Vector3.Dot (t.up, rootT.forward),
			Vector3.Dot (t.forward, rootT.forward)
		);
		return pseudX != Vector3.zero && pseudY != Vector3.zero && pseudZ != Vector3.zero;
	}
	public bool GetPseuds (Transform tra, out Vector3 pseudX, out Vector3 pseudY, out Vector3 pseudZ)
	{
		pseudX = Vector3.zero;
		pseudY = Vector3.zero;
		pseudZ = Vector3.zero;
		foreach (var bone in targetBones)
		{
			if (tra == animator.GetBoneTransform (bone))
			{
				pseudX = pseudXaxis[bone];
				pseudY = pseudYaxis[bone];
				pseudZ = pseudZaxis[bone];
				return true;
			}
		}
		return false;
	}
	private void CopyRotations (Animator src, Animator dest)
	{
		foreach (var bone in targetBones)
		{
			//ボーンがデフォでWorldのXYZに沿ってる場合の回転を表すパラメタをまず拾う
			float angle;
			Vector3 axis;
			src.GetBoneTransform (bone).localRotation.ToAngleAxis (out angle, out axis);

			Vector3 axisInLocalCoordinate = axis.x * pseudXaxis[bone] + axis.y * pseudYaxis[bone] + axis.z * pseudZaxis[bone];

			Quaternion modifiedRotation = Quaternion.AngleAxis (angle, axisInLocalCoordinate);

			dest.GetBoneTransform (bone).localRotation =
				initialRotations[bone] *
				modifiedRotation;
		}
	}

	private void InitializeLocalRotations ()
	{
		initialRotations = targetBones.ToDictionary (
			b => b,
			b => animator.GetBoneTransform (b).localRotation
		);

		var rootT = animator.GetBoneTransform (HumanBodyBones.Hips).root;

		pseudXaxis = targetBones.ToDictionary (
			b => b,
			b =>
			{
				var t = animator.GetBoneTransform (b);
				return new Vector3 (
					Vector3.Dot (t.right, rootT.right),
					Vector3.Dot (t.up, rootT.right),
					Vector3.Dot (t.forward, rootT.right)
				);
			});

		pseudYaxis = targetBones.ToDictionary (
			b => b,
			b =>
			{
				var t = animator.GetBoneTransform (b);
				return new Vector3 (
					Vector3.Dot (t.right, rootT.up),
					Vector3.Dot (t.up, rootT.up),
					Vector3.Dot (t.forward, rootT.up)
				);
			});

		pseudZaxis = targetBones.ToDictionary (
			b => b,
			b =>
			{
				var t = animator.GetBoneTransform (b);
				return new Vector3 (
					Vector3.Dot (t.right, rootT.forward),
					Vector3.Dot (t.up, rootT.forward),
					Vector3.Dot (t.forward, rootT.forward)
				);
			});
	}

	//コピー対象になるボーン一覧(もちろん足したり削ったりしてOK)
	private static HumanBodyBones[] targetBones = new []
	{
		HumanBodyBones.Hips,
			HumanBodyBones.RightUpperLeg,
			HumanBodyBones.RightLowerLeg,
			HumanBodyBones.RightFoot,
			HumanBodyBones.LeftUpperLeg,
			HumanBodyBones.LeftLowerLeg,
			HumanBodyBones.LeftFoot,
			HumanBodyBones.Spine,
			HumanBodyBones.Chest,
			HumanBodyBones.Neck,
			HumanBodyBones.Head,
			HumanBodyBones.RightShoulder,
			HumanBodyBones.RightUpperArm,
			HumanBodyBones.RightLowerArm,
			HumanBodyBones.LeftShoulder,
			HumanBodyBones.LeftUpperArm,
			HumanBodyBones.LeftLowerArm,
	};
}