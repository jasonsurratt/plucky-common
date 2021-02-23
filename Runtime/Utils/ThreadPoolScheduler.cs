namespace Plucky.Common
{
    //public class ThreadPoolScheduler : MonoBehaviour
    //{
    //    static ThreadPoolScheduler _instance;
    //    public static ThreadPoolScheduler instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                GameObject go = new GameObject("ThreadPoolScheduler");
    //                _instance = go.AddComponent<ThreadPoolScheduler>();
    //            }
    //            return _instance;
    //        }
    //    }

    //    ConcurrentQueue<IEnumerator> jobs = new ConcurrentQueue<IEnumerator>();

    //    public void Schedule(

    //        ) => jobs.Enqueue(job);

    //    public void OnDestroy()
    //    {
    //        _instance = null;
    //    }

    //    public void Start()
    //    {
    //        StartCoroutine(RunJobs());
    //    }

    //    IEnumerator RunJobs()
    //    {
    //        while (true)
    //        {
    //            object result = null;
    //            if (jobs.Count > 0)
    //            {
    //                var job = jobs.Peek();

    //                // for now we just pass waits and such through -- this isn't necessarily ideal
    //                // but good enough for now.
    //                if (job.MoveNext())
    //                {
    //                    result = job.Current;
    //                }
    //                else
    //                {
    //                    jobs.Dequeue();
    //                }
    //            }
    //            yield return result;
    //        }
    //    }
    //}
}
