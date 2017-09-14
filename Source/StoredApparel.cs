﻿using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
using Verse.AI;
using System.Text;

namespace ChangeDresser
{
    class StoredApparel
    {
        private static int ID = 0;

        private readonly int UniqueId;
        private Dictionary<Def, LinkedList<Apparel>> StoredApparelLookup = new Dictionary<Def, LinkedList<Apparel>>();
        public int Count { get; private set; }

        public StoredApparel()
        {
            this.UniqueId = ID;
            ++ID;
            this.Count = 0;
        }

        public IEnumerable<Apparel> Apparel
        {
            get
            {
                List<Apparel> l = new List<Apparel>(this.Count);
                foreach (LinkedList<Apparel> ll in this.StoredApparelLookup.Values)
                {
                    foreach (Apparel a in ll)
                    {
                        l.Add(a);
                    }
                }
                return l;
            }
        }

        public void AddApparel(Apparel apparel)
        {
            if (apparel != null)
            {
                LinkedList<Apparel> l;
                if (!this.StoredApparelLookup.TryGetValue(apparel.def, out l))
                {
                    l = new LinkedList<Apparel>();
                    this.StoredApparelLookup.Add(apparel.def, l);
                }
                this.AddApparelToLinkedList(apparel, l);
            }
        }

        private void AddApparelToLinkedList(Apparel apparel, LinkedList<Apparel> l)
        {
#if TRACE
            Log.Message("Start StoredApparel.AddApparelToLinkedList");
            Log.Warning("Apparel: " + apparel.Label);
            StringBuilder sb = new StringBuilder("LinkedList: ");
            foreach (Apparel a in l)
            {
                sb.Append(a.LabelShort);
                sb.Append(", ");
            }
            Log.Warning(sb.ToString());
#endif
            QualityCategory q;
            if (!apparel.TryGetQuality(out q))
            {
#if TRACE
                Log.Message("AddLast - quality not found");
#endif
                l.AddLast(apparel);
            }
            else
            {
#if TRACE
                Log.Message("HP: " + apparel.HitPoints + " HPMax: " + apparel.MaxHitPoints);
#endif
                int hpPercent = apparel.HitPoints / apparel.MaxHitPoints;
                for (LinkedListNode<Apparel> n = l.First; n != null; n = n.Next)
                {
                    QualityCategory nq;
                    if (!n.Value.TryGetQuality(out nq) ||
                        q > nq ||
                        (q == nq && hpPercent >= (n.Value.HitPoints / n.Value.MaxHitPoints)))
                    {
                        l.AddBefore(n, apparel);
                        return;
                    }
                }
                l.AddLast(apparel);
            }
            ++this.Count;
#if TRACE
            Log.Message("End StoredApparel.AddApparelToLinkedList");
#endif
        }

        public bool Contains(Apparel apparel)
        {
            LinkedList<Apparel> l;
            if (this.StoredApparelLookup.TryGetValue(apparel.def, out l))
            {
                return l.Contains(apparel);
            }
            return false;
        }

        public void Clear()
        {
            foreach (LinkedList<Apparel> l in this.StoredApparelLookup.Values)
            {
                l.Clear();
            }
            this.StoredApparelLookup.Clear();
        }

        public bool TryRemoveApparel(ThingDef def, out Apparel apparel)
        {
            LinkedList<Apparel> l;
            if (this.StoredApparelLookup.TryGetValue(def, out l))
            {
                if (l.Count > 0)
                {
                    apparel = l.First.Value;
                    l.RemoveFirst();
                    --this.Count;
                    return true;
                }
            }
            apparel = null;
            return false;
        }

        public bool TryRemoveBestApparel(ThingDef def, out Apparel apparel)
        {
            LinkedList<Apparel> l;
            if (this.StoredApparelLookup.TryGetValue(def, out l))
            {
                if (l.Count > 0)
                {
                    apparel = l.First.Value;
                    l.RemoveFirst();
                    --this.Count;
                    return true;
                }
            }
            apparel = null;
            return false;
        }

        public bool RemoveApparel(Apparel apparel)
        {
            LinkedList<Apparel> l;
            if (this.StoredApparelLookup.TryGetValue(apparel.def, out l))
            {
                if (l.Remove(apparel))
                {
                    --this.Count;
                    return true;
                }
            }
            return false;
        }

        internal bool TryRemoveBestApparel(ThingDef def, ThingFilter filter, out Apparel apparel)
        {
#if DEBUG
            Log.Message(Environment.NewLine + "Start StoredApparel.TryRemoveBestApperal Def: " + def.label);
#endif
            LinkedList<Apparel> l;
            if (this.StoredApparelLookup.TryGetValue(def, out l))
            {
#if DEBUG
                Log.Warning("Apparel List found Count: " + l.Count);
#endif
                for (LinkedListNode<Apparel> n = l.First; n != null; n = n.Next)
                {
#if DEBUG
                    Log.Warning("Apparel " + n.Value.Label);
#endif
                    try
                    {
                        if (filter.Allows(n.Value))
                        {
                            l.Remove(n);
                            apparel = n.Value;
                            --this.Count;
#if DEBUG
                            Log.Warning("Start StoredApparel.TryRemoveBestApperal Return: True Apparel:" + apparel.LabelShort + Environment.NewLine);
#endif
                            return true;
                        }
#if DEBUG
                        else
                            Log.Warning("Filter rejected");
#endif
                    }
                    catch
                    {
                        Log.Error("catch");
                    }
                }
            }
            apparel = null;
#if DEBUG
            Log.Message("End StoredApparel.TryRemoveBestApperal Return: False" + Environment.NewLine);
#endif
            return false;
        }

        public List<Apparel> RemoveFilteredApparel(ThingFilter filter)
        {
            List<Apparel> removed = new List<Apparel>(0);
            foreach (LinkedList<Apparel> ll in this.StoredApparelLookup.Values)
            {
                
            }
            return removed;
        }

        public override bool Equals(object obj)
        {
            return obj != null && this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.UniqueId;
        }

        public Apparel FindBetterApparel(ref float baseApparelScore, Pawn pawn, Outfit currentOutfit, Building dresser)
        {
            Apparel betterApparel = null;
            foreach (LinkedList<Apparel> ll in this.StoredApparelLookup.Values)
            {
                foreach (Apparel apparel in ll)
                {
                    if (currentOutfit.filter.Allows(apparel))
                    {
                        if (!apparel.IsForbidden(pawn))
                        {
                            float newApparelScore = JobGiver_OptimizeApparel.ApparelScoreGain(pawn, apparel);
                            if (newApparelScore >= 0.05f && newApparelScore >= baseApparelScore)
                            {
                                if (ApparelUtility.HasPartsToWear(pawn, apparel.def))
                                {
                                    if (ReservationUtility.CanReserveAndReach(pawn, dresser, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1))
                                    {
                                        betterApparel = apparel;
                                        baseApparelScore = newApparelScore;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return betterApparel;
        }
    }
}
