﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using CriticalPathMethod;
using System.Collections.Generic;

namespace ComputerEngineering
{
    class CPM
    {

        private static void Main(string[] args)
        {
            // Array to store the activities that'll be evaluated.
            var activities = GetActivities();
            Output(activities.Shuffle().CriticalPath(p => p.Predecessors, l => (long)l.Duration));
        }

        private static void Output(IEnumerable<Activity> list)
        {
            Console.Write("\n          Critical Path: ");
            var totalDuration = 0L;
            foreach (Activity activity in list)
            {
                if (activity.Id == "END") continue;
                Console.Write("{0} ", activity.Id);
                totalDuration += activity.Duration;
            }
            Console.Write("\n\n         Total duration: {0}\n\n", totalDuration);
        }

        /// <summary>
        /// Gets the activities that'll be evaluated by the critical path method.
        /// </summary>
        /// <param name="list">Array to store the activities that'll be evaluated.</param>
        /// <returns>list</returns>
        private static IEnumerable<Activity> GetActivities()
        {
            var list = new List<Activity>();
            var input = System.IO.File.ReadAllLines("input.txt");
            var ad = new Dictionary<string, Activity>();
            var deferredList = new Dictionary<Activity, List<string>>();
            Console.Write("\n       Number of activities: " + input.Length);

            int inx = 0;
            foreach (var line in input)
            {
                var activity = new Activity();
                var elements = line.Split(' ');
                Console.WriteLine("\n                Activity {0}\n", inx + 1);

                activity.Id = elements[0];
                Console.WriteLine("\n                     ID: " + activity.Id);
                ad.Add(activity.Id, activity);
                activity.Description = elements[1];
                Console.WriteLine("            Description: " + activity.Description);

                activity.Duration = int.Parse(elements[2]);
                Console.WriteLine("               Duration: " + activity.Duration);

                int np = int.Parse(elements[3]);
                Console.WriteLine(" Number of predecessors: ", np);

                if (np != 0)
                {
                    var allIds = new List<string>();
                    for (int j = 0; j < np; j++)
                    {
                        allIds.Add(elements[4 + j]);
                        Console.WriteLine("    #{0} predecessor's ID: " + elements[4 + j], j + 1);
                    }

                    if (allIds.Any(i => !ad.ContainsKey(i)))
                    {
                        // Defer processing on this one
                        deferredList.Add(activity, allIds);
                    }
                    else
                    {
                        foreach (var id in allIds)
                        {
                            var aux = ad[id];

                            activity.Predecessors.Add(aux);
                        }
                    }
                }
                list.Add(activity);
            }

            while (deferredList.Count > 0)
            {
                var processedActivities = new List<Activity>();
                foreach (var activity in deferredList)
                {
                    if (activity.Value.Where(ad.ContainsKey).Count() == activity.Value.Count)
                    {
                        // All dependencies are now loaded
                        foreach (var id in activity.Value)
                        {
                            var aux = ad[id];

                            activity.Key.Predecessors.Add(aux);
                        }
                        processedActivities.Add(activity.Key);
                    }
                }
                foreach (var activity in processedActivities)
                {
                    deferredList.Remove(activity);
                }
            }

            return GetFreeEndActivities(list);
        }

        /// <summary>
        /// Adds a zero duration activity with free end activities as predecessors.
        /// </summary>
        /// <param name="list">Array to store the activities that'll be evaluated.</param>
        /// <returns>list</returns>
        private static List<Activity> GetFreeEndActivities(List<Activity> list)
        {
            var endActivity = new Activity() { Id = "END", Description = "End Activity", Duration = 0 };
            foreach (var activity in list)
            {
                var foundSuccessors = false;
                foreach (var ac in list)
                {
                    if (ac.Predecessors.Any(a => a.Id == activity.Id))
                    {
                        foundSuccessors = true;
                    }
                }
                if (!foundSuccessors)
                {
                    endActivity.Predecessors.Add(activity);
                }
            }
            list.Add(endActivity);
            return list;
        }
    }
}
