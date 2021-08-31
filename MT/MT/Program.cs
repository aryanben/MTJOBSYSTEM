using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace MT
{
    class Program
    {
        static void Main(string[] args)
        {
            ExampleJobo exJobo = new ExampleJobo();
            JobSystemo JS = new JobSystemo();
            int jobId = JS.TryAdd(exJobo);
            if (jobId>0)
            {
                Console.WriteLine("jobo submitted my man");
            }
            while (true)
            {
                Jobo j = null;

                if (JS.TryGetJob(jobId,out j))
                {
                    ExampleJobo ej = (ExampleJobo)j;
                    Console.WriteLine("results is"+ j.result + "  --- "+ej.result);
                    break;
                }
            }
        }
    }

    public class Jobo
    {
        static int globalId = 0;

        public enum State { Started, Done };
        public State state;

        public Object result
        {
            get;
            protected set;
        }

        int id;
        public int ID
        {
            get
            {
                return id;
            }
        }
        public Jobo()
        {
            globalId++;
            id = globalId;
        }

        public virtual void Work(Object stateInfo)
        {
            state = State.Started;
        }
    }
    public class JobSystemo
    {
        ConcurrentDictionary<int, Jobo> dic = new ConcurrentDictionary<int, Jobo>();

        public int TryAdd(Jobo job)
        {
            if (dic.TryAdd(job.ID, job))
            {
                if (ThreadPool.QueueUserWorkItem(job.Work))
                {
                    return job.ID;

                }

            }
            return -1;
        }

        public bool TryGetJob(int id, out Jobo job)
        {
            Jobo temp = new Jobo();

            if (dic.TryGetValue(id, out temp))
            {
                if (temp != null)
                {
                    if (temp.state == Jobo.State.Done)
                    {
                        job = temp;
                        return true;
                    }
                }

            }
            job = null;
            return false;
        }
    }
    public class ExampleJobo : Jobo
    {
        public override void Work(object stateInfo)
        {
            base.Work(stateInfo);
            int ex=1;
            for (int i = 0; i < 100; i++)
            {
                
                Console.WriteLine("hello"+i);
                ex = ex + i;
                

            }
            result = ex+" lol";
            state = State.Done;
        }
    }

}

