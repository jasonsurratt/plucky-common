using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plucky.Common
{

    public class CoroutineJob
    {
        AsyncOperation asyncOp = null;
        IEnumerator it;

        public CoroutineJob(IEnumerator it)
        {
            this.it = it;
        }

        public bool Tick()
        {
            bool result = false;

            if (asyncOp != null)
            {
                result = true;
                if (asyncOp.isDone)
                {
                    asyncOp = null;
                }
            }
            else if (it.MoveNext())
            {
                result = true;

                switch (it.Current)
                {
                    case AsyncOperation aop:
                        asyncOp = aop;
                        break;
                }
            }

            return result;
        }
    }

    public class MainThreadScheduler : MonoBehaviour
    {
        float endOfLastFrameTime;

        int framesSinceLastJob = 0;

        static MainThreadScheduler _instance;
        public static MainThreadScheduler instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("MainThreadScheduler");
                    _instance = go.AddComponent<MainThreadScheduler>();
                }
                return _instance;
            }
        }

        Queue<CoroutineJob> jobs = new Queue<CoroutineJob>();

        public ExponentialMovingAverage jobsPerFrame = new ExponentialMovingAverage(30);

        List<CoroutineJob> jobsTmp = new List<CoroutineJob>();

        // what is the maximum amount of time to run background tasks.
        public float maxExecutionTime = 0.005f;

        /// maxFramesBetweenJobs is the maximum number of frames the scheduler will go between
        /// running at least one tick on one job.
        public int maxFramesBetweenJobs = 5;

        public int pendingJobs => jobs.Count;

        public ExponentialMovingAverage timePerFrame = new ExponentialMovingAverage(30);
        public ExponentialMovingAverage timePerFrameExecuting = new ExponentialMovingAverage(30);

        public void Schedule(IEnumerator job) => jobs.Enqueue(new CoroutineJob(job));

        public void OnDestroy()
        {
            _instance = null;
        }

        public void LateUpdate()
        {
            float startTime = Time.realtimeSinceStartup;
            float lastFrameLength = endOfLastFrameTime - startTime;

            timePerFrame.Add(lastFrameLength);

            float timeAllotted = Mathf.Min(maxExecutionTime, (float)timePerFrame.value - lastFrameLength);
            float endTime = startTime + timeAllotted;

            framesSinceLastJob++;

            int runCount = 0;
            jobsTmp.Clear();
            while (jobs.Count > 0 &&
                (Time.realtimeSinceStartup < endTime || framesSinceLastJob >= maxFramesBetweenJobs))
            {
                var job = jobs.Dequeue();
                runCount++;
                framesSinceLastJob = 0;

                try
                {
                    // Go through each job one at a time.
                    if (job.Tick())
                    {
                        jobsTmp.Add(job);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error running job: {e}");
                }
            }

            for (int i = 0; i < jobsTmp.Count; i++)
            {
                jobs.Enqueue(jobsTmp[i]);
            }

            timePerFrame.Add(Time.realtimeSinceStartup - startTime);
            jobsPerFrame.Add(runCount);

            endOfLastFrameTime = Time.realtimeSinceStartup;
        }
    }
}
