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
	[SerializeField]
	private int objectsCount = 10000;
	private Transform[] transforms;

	private Vector3 movementSpeed = new Vector3(5f, 0f, 0f);

	private void Awake()
	{
		transforms = new Transform[objectsCount];

		for (int i = 0; i < objectsCount; i++)
			transforms[i] = Instantiate(prefabToSpawn, transform).transform;
	}

	private void OnEnable()
	{
		for (int i = 0; i < objectsCount; i++)
			transforms[i].gameObject.SetActive(true);

		transformAccessArray = new TransformAccessArray(transforms);
	}

	private void OnDisable()
	{
		for (int i = 0; i < objectsCount; i++)
			transforms[i].gameObject.SetActive(false);

		transformAccessArray.Dispose();
	}

	private void Update()
	{
		movementBehaviourJob = new MovementBehaviourJob()
		{
			movementSpeed = movementSpeed,
			deltaTime = Time.deltaTime
		};

		movementBehaviourHandle = movementBehaviourJob.Schedule(transformAccessArray);
		JobHandle.ScheduleBatchedJobs();
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
		transform.localPosition += movementSpeed * deltaTime;
	}
}
