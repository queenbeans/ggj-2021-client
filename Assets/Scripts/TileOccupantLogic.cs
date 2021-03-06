using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel; // leantween

namespace Dossamer.Ggj2021 {
	public class TileOccupantLogic : MonoBehaviour
	{
		[SerializeField]
		bool isTile3D = false;

		public Vector3 targetPosition;

		public float moveSpeed = 4.0f; // four tiles per second
		public Queue<Vector3> movementQueue = new Queue<Vector3>();


		private bool isMoving = false;
		private LTDescr myTween;

		private Vector3 lastPosition;

		public bool getIsMoving()
		{
			return isMoving;
		}

		// Start is called before the first frame update
		void Start()
		{
			targetPosition = transform.position;
			lastPosition = transform.position;

			if (isTile3D)
			{
				transform.Rotate(Vector3.up, Random.Range(0f, 360f));
			}
		}

		public void setNewTarget(Vector3 finalPosition, Queue<Vector3> queue) {
			targetPosition = finalPosition;
			movementQueue = queue;
		}


		// Update is called once per frame
		void Update()
		{
			if (transform.position != targetPosition && !isMoving)
			{
				// make sure the path to target is valid
				if (movementQueue.Count > 0)
				{
					isMoving = true;
					LTDescr myTween = tweenToNewPosition(movementQueue.Dequeue());
				} else
				{
					// otherwise abort
					targetPosition = transform.position;
				}
			}

			if (!isTile3D)
			{
				transform.LookAt(CustomClient.Instance.referenceCamera.transform);
			}
		}

		LTDescr tweenToNewPosition(Vector3 position)
		{
			

			if (isTile3D)
			{
				// unfortunately there's no easy way to deep copy a transform
				Quaternion oldRotation = transform.rotation;
				transform.LookAt(position);
				Quaternion newRotation = transform.rotation;
				transform.rotation = oldRotation;

				LeanTween.rotate(gameObject, newRotation.eulerAngles, moveSpeed).setEase(LeanTweenType.easeOutSine);
			}
			return LeanTween.move(gameObject, position, (position - transform.position).magnitude / moveSpeed).setEase(LeanTweenType.easeOutQuad)
							.setOnComplete(() => {
								if (movementQueue.Count > 0)
								{
									myTween = tweenToNewPosition(movementQueue.Dequeue());
								}
								else
								{
									isMoving = false;
									CustomClient.Instance.UpdateIsPlayerMoving(isMoving);
								}
							});
		}
	}
}