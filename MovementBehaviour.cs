using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections;

public class MovementBehaviour : MonoBehaviour
{
	private TransformAccessArray transformAccessArray;
	private MovementBehaviourJob movementBehaviourJob;
	private JobHandle movementBehaviourHandle;

	[SerializeField]
	private GameObject prefabToSpawn;
	private Transform[] transforms = new Transform[10000];

	private Vector3 movementSpeed = new Vector3(5f, 0f, 0f);

	private void OnEnable()
	{
		for (int i = 0; i < 10000; i++)
		{
			GameObject tester = Instantiate(prefabToSpawn);
			transforms[i] = tester.transform;
		}

		transformAccessArray = new TransformAccessArray(transforms);
	}

	private void OnDisable()
	{
		transformAccessArray.Dispose();
	}

	private void Update()
	{
		movementBehaviourJob = new MovementBehaviourJob()
		{
			movementSpeed = movementSpeed,
			deltaTime = Time.deltaTime
		};

		movementBehaviourHandle = movementBehaviourJob.Schedule(transformAccessArray, movementBehaviourHandle);
	}

	private void LateUpdate()
	{
		movementBehaviourHandle.Complete();
	}
}

[BurstCompile]
public struct MovementBehaviourJob : IJobParallelForTransform
{
	public Vector3 movementSpeed;
	public float deltaTime;

	public void Execute(int index, TransformAccess transform)
	{
		transform.position += movementSpeed * deltaTime;
	}
}
