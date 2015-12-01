// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public class AIAction
    {
        public Enemy obj;
        public enum Actions
        {
            Move,Attack
        }
        public Actions action;
        public Vector3 pos;

        public AIAction(Enemy o,Actions a,Vector3 p)
        {
            obj = o;
            action = a;
            pos = p;
        }
    }
	public abstract class AIBase
	{
        public int Count
        {
            set { }
            get { return self.Count; }
        }
        public int mode; //0 = PvP, 1 = PvE, 2 = EvE
        public List<MovingObject> other;
        public List<Enemy> self;
        protected List<AIAction> actions;
        public List<AIAction> acts
        {
            set { }
            get { return actions; }
        }
        public virtual void onTurn()
        {
            actions = new List<AIAction>();
        }
        public void init(List<Enemy> s,List<MovingObject> o)
        {
            self = s;
            other = o;
        }
	}
}

